using System.Formats.Nrbf;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace App.Parser.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
    }

    [HttpPost("dump")]
    public IActionResult Dump([FromBody] BinaryFormatterRequest request)
    {
        try
        {
            if (!TryHexToBytes(request.Data, out byte[] bytes))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid hex data."
                });
            }

            using var stream = new MemoryStream(bytes, writable: false);

            SerializationRecord rootRecord = NrbfDecoder.Decode(stream);

            object? data = DumpValue(
                value: rootRecord,
                visitedRecordIds: new HashSet<SerializationRecordId>(),
                depth: 0);

            return Ok(new
            {
                success = true,
                data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BinaryFormatter payload dump error.");

            return BadRequest(new
            {
                success = false,
                error = ex.Message,
                exceptionType = ex.GetType().FullName
            });
        }
    }

    private static object? DumpValue(
        object? value,
        HashSet<SerializationRecordId> visitedRecordIds,
        int depth)
    {
        if (value is null)
            return null;

        if (depth > 80)
            return "<max-depth>";

        if (IsSimpleValue(value))
            return value;

        if (value is byte[] byteArray)
            return "0x" + Convert.ToHexString(byteArray);

        if (value is PrimitiveTypeRecord primitiveRecord)
            return primitiveRecord.Value;

        if (value is ClassRecord classRecord)
            return DumpClassRecord(classRecord, visitedRecordIds, depth);

        if (value is ArrayRecord arrayRecord)
            return DumpArrayRecord(arrayRecord, visitedRecordIds, depth);

        if (value is Array runtimeArray)
            return DumpRuntimeArray(runtimeArray, visitedRecordIds, depth);

        if (value is SerializationRecord serializationRecord)
        {
            return new Dictionary<string, object?>
            {
                ["$recordType"] = serializationRecord.RecordType.ToString(),
                ["$runtimeRecordType"] = serializationRecord.GetType().FullName,
                ["$type"] = serializationRecord.TypeName.AssemblyQualifiedName
            };
        }

        return value.ToString();
    }

    private static object DumpClassRecord(
        ClassRecord classRecord,
        HashSet<SerializationRecordId> visitedRecordIds,
        int depth)
    {
        if (!visitedRecordIds.Add(classRecord.Id))
        {
            return new Dictionary<string, object?>
            {
                ["$ref"] = classRecord.TypeName.AssemblyQualifiedName
            };
        }

        string[] memberNames = classRecord.MemberNames.ToArray();

        var result = new Dictionary<string, object?>
        {
            ["$recordType"] = classRecord.RecordType.ToString(),
            ["$runtimeRecordType"] = classRecord.GetType().FullName,
            ["$type"] = classRecord.TypeName.AssemblyQualifiedName,
            ["$memberCount"] = memberNames.Length
        };

        foreach (string memberName in memberNames)
        {
            string displayName = NormalizeMemberName(memberName);
            displayName = MakeUniqueKey(result, displayName);

            try
            {
                object? rawValue = classRecord.GetRawValue(memberName);

                result[displayName] = DumpValue(
                    rawValue,
                    visitedRecordIds,
                    depth + 1);
            }
            catch (Exception ex)
            {
                result[displayName] = $"<read-error: {ex.GetType().Name} - {ex.Message}>";
            }
        }

        return result;
    }

    private static object DumpArrayRecord(
        ArrayRecord arrayRecord,
        HashSet<SerializationRecordId> visitedRecordIds,
        int depth)
    {
        if (!visitedRecordIds.Add(arrayRecord.Id))
        {
            return new Dictionary<string, object?>
            {
                ["$ref"] = arrayRecord.TypeName.AssemblyQualifiedName
            };
        }

        int[] lengths = arrayRecord.Lengths.ToArray();

        long totalLength = 1;

        foreach (int length in lengths)
        {
            totalLength *= length;
        }

        var result = new Dictionary<string, object?>
        {
            ["$recordType"] = arrayRecord.RecordType.ToString(),
            ["$runtimeRecordType"] = arrayRecord.GetType().FullName,
            ["$type"] = arrayRecord.TypeName.AssemblyQualifiedName,
            ["$rank"] = arrayRecord.Rank,
            ["$lengths"] = lengths
        };

        const int maxArrayItems = 5000;

        if (totalLength > maxArrayItems)
        {
            result["$items"] = $"<array too large: {totalLength} items>";
            return result;
        }

        Array? array = TryGetArray(arrayRecord);

        if (array is null)
        {
            result["$items"] = "<array values could not be read>";
            return result;
        }

        result["$items"] = DumpRuntimeArray(
            array,
            visitedRecordIds,
            depth + 1);

        return result;
    }

    private static object DumpRuntimeArray(
        Array array,
        HashSet<SerializationRecordId> visitedRecordIds,
        int depth)
    {
        if (array is byte[] byteArray)
            return "0x" + Convert.ToHexString(byteArray);

        var list = new List<object?>();

        foreach (object? item in array)
        {
            list.Add(DumpValue(
                item,
                visitedRecordIds,
                depth + 1));
        }

        return list;
    }

    private static Array? TryGetArray(ArrayRecord arrayRecord)
    {
        try
        {
            Type runtimeType = arrayRecord.GetType();

            MethodInfo? getArrayBoolMethod = runtimeType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(method =>
                {
                    if (method.Name != "GetArray")
                        return false;

                    ParameterInfo[] parameters = method.GetParameters();

                    return parameters.Length == 1 &&
                           parameters[0].ParameterType == typeof(bool);
                });

            if (getArrayBoolMethod is not null)
            {
                return getArrayBoolMethod.Invoke(
                    arrayRecord,
                    new object[] { true }) as Array;
            }

            MethodInfo? getArrayEmptyMethod = runtimeType.GetMethod(
                "GetArray",
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);

            if (getArrayEmptyMethod is not null)
            {
                return getArrayEmptyMethod.Invoke(arrayRecord, null) as Array;
            }

            MethodInfo? getArrayWithTypeMethod = typeof(ArrayRecord).GetMethod(
                "GetArray",
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types: new[] { typeof(Type), typeof(bool) },
                modifiers: null);

            if (getArrayWithTypeMethod is null)
                return null;

            Type[] candidateArrayTypes =
            {
                ResolveKnownArrayType(arrayRecord.TypeName.AssemblyQualifiedName) ?? typeof(object[]),
                typeof(SerializationRecord[]),
                typeof(object[])
            };

            foreach (Type candidateArrayType in candidateArrayTypes.Distinct())
            {
                try
                {
                    Array? array = getArrayWithTypeMethod.Invoke(
                        arrayRecord,
                        new object[] { candidateArrayType, true }) as Array;

                    if (array is not null)
                        return array;
                }
                catch
                {
                    // Diğer candidate type'ı dene.
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static Type? ResolveKnownArrayType(string? assemblyQualifiedName)
    {
        if (string.IsNullOrWhiteSpace(assemblyQualifiedName))
            return null;

        if (assemblyQualifiedName.Contains("System.Byte[]"))
            return typeof(byte[]);

        if (assemblyQualifiedName.Contains("System.String[]"))
            return typeof(string[]);

        if (assemblyQualifiedName.Contains("System.Int16[]"))
            return typeof(short[]);

        if (assemblyQualifiedName.Contains("System.Int32[]"))
            return typeof(int[]);

        if (assemblyQualifiedName.Contains("System.Int64[]"))
            return typeof(long[]);

        if (assemblyQualifiedName.Contains("System.Decimal[]"))
            return typeof(decimal[]);

        if (assemblyQualifiedName.Contains("System.Boolean[]"))
            return typeof(bool[]);

        if (assemblyQualifiedName.Contains("System.DateTime[]"))
            return typeof(DateTime[]);

        return Type.GetType(assemblyQualifiedName, throwOnError: false);
    }

    private static bool IsSimpleValue(object value)
    {
        return value is string
            or bool
            or byte
            or sbyte
            or short
            or ushort
            or int
            or uint
            or long
            or ulong
            or float
            or double
            or decimal
            or char
            or DateTime
            or TimeSpan
            or Guid;
    }

    private static string NormalizeMemberName(string memberName)
    {
        int startIndex = memberName.LastIndexOf('<');
        int endIndex = memberName.IndexOf(">k__BackingField", StringComparison.Ordinal);

        if (startIndex >= 0 && endIndex > startIndex)
        {
            return memberName.Substring(
                startIndex + 1,
                endIndex - startIndex - 1);
        }

        if (memberName.StartsWith("_") && memberName.Length > 1)
            return memberName[1..];

        return memberName;
    }

    private static string MakeUniqueKey(
        Dictionary<string, object?> dictionary,
        string key)
    {
        if (!dictionary.ContainsKey(key))
            return key;

        int counter = 2;

        while (dictionary.ContainsKey($"{key}_{counter}"))
        {
            counter++;
        }

        return $"{key}_{counter}";
    }

    private static bool TryHexToBytes(string? hex, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();

        try
        {
            if (string.IsNullOrWhiteSpace(hex))
                return false;

            hex = hex.Trim();

            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex[2..];

            hex = hex
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");

            if (hex.Length == 0)
                return false;

            if (hex.Length % 2 != 0)
                return false;

            bytes = Convert.FromHexString(hex);
            return true;
        }
        catch
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }
}

public sealed class BinaryFormatterRequest
{
    public string Data { get; set; } = string.Empty;
}