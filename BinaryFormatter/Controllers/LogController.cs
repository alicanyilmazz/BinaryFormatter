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
                visitedRecords: new HashSet<string>(),
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
        HashSet<string> visitedRecords,
        int depth)
    {
        if (value is null)
            return null;

        if (depth > 50)
            return "<max-depth>";

        if (IsSimpleValue(value))
            return value;

        if (value is byte[] byteArray)
            return "0x" + Convert.ToHexString(byteArray);

        if (value is PrimitiveTypeRecord primitiveRecord)
            return primitiveRecord.Value;

        if (value is ClassRecord classRecord)
            return DumpClassRecord(classRecord, visitedRecords, depth);

        if (value is ArrayRecord arrayRecord)
            return DumpArrayRecord(arrayRecord, visitedRecords, depth);

        if (value is Array runtimeArray)
            return DumpRuntimeArray(runtimeArray, visitedRecords, depth);

        if (value is SerializationRecord serializationRecord)
        {
            return new Dictionary<string, object?>
            {
                ["$id"] = serializationRecord.Id.ToString(),
                ["$recordType"] = serializationRecord.RecordType.ToString(),
                ["$type"] = serializationRecord.TypeName.AssemblyQualifiedName
            };
        }

        return value.ToString();
    }

    private static object DumpClassRecord(
        ClassRecord classRecord,
        HashSet<string> visitedRecords,
        int depth)
    {
        string recordId = classRecord.Id.ToString();

        if (!string.IsNullOrWhiteSpace(recordId))
        {
            if (!visitedRecords.Add(recordId))
            {
                return new Dictionary<string, object?>
                {
                    ["$ref"] = recordId,
                    ["$type"] = classRecord.TypeName.AssemblyQualifiedName
                };
            }
        }

        string[] memberNames = classRecord.MemberNames.ToArray();

        var result = new Dictionary<string, object?>
        {
            ["$id"] = recordId,
            ["$runtimeRecordType"] = classRecord.GetType().FullName,
            ["$recordType"] = classRecord.RecordType.ToString(),
            ["$type"] = classRecord.TypeName.AssemblyQualifiedName,
            ["$memberCount"] = memberNames.Length
        };

        foreach (string memberName in memberNames)
        {
            string displayName = NormalizeMemberName(memberName);

            try
            {
                object? rawValue = classRecord.GetRawValue(memberName);

                result[displayName] = DumpValue(
                    rawValue,
                    visitedRecords,
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
        HashSet<string> visitedRecords,
        int depth)
    {
        int[] lengths = arrayRecord.Lengths.ToArray();

        long totalLength = 1;

        foreach (int length in lengths)
        {
            totalLength *= length;
        }

        var result = new Dictionary<string, object?>
        {
            ["$id"] = arrayRecord.Id.ToString(),
            ["$recordType"] = arrayRecord.RecordType.ToString(),
            ["$type"] = arrayRecord.TypeName.AssemblyQualifiedName,
            ["$rank"] = arrayRecord.Rank,
            ["$lengths"] = lengths
        };

        const int maxArrayItems = 1000;

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

        result["$items"] = DumpRuntimeArray(array, visitedRecords, depth + 1);

        return result;
    }

    private static object DumpRuntimeArray(
        Array array,
        HashSet<string> visitedRecords,
        int depth)
    {
        if (array is byte[] byteArray)
            return "0x" + Convert.ToHexString(byteArray);

        var list = new List<object?>();

        foreach (object? item in array)
        {
            list.Add(DumpValue(item, visitedRecords, depth + 1));
        }

        return list;
    }

    private static Array? TryGetArray(ArrayRecord arrayRecord)
    {
        try
        {
            Type runtimeType = arrayRecord.GetType();

            // SZArrayRecord<T> için: GetArray(bool allowNulls = true)
            MethodInfo? szArrayGetArray = runtimeType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(method =>
                {
                    if (method.Name != "GetArray")
                        return false;

                    ParameterInfo[] parameters = method.GetParameters();

                    return parameters.Length == 1 &&
                           parameters[0].ParameterType == typeof(bool);
                });

            if (szArrayGetArray is not null)
            {
                return szArrayGetArray.Invoke(
                    arrayRecord,
                    new object[] { true }) as Array;
            }

            // Bazı sürümlerde parametresiz olabilir.
            MethodInfo? parameterlessGetArray = runtimeType.GetMethod(
                "GetArray",
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);

            if (parameterlessGetArray is not null)
            {
                return parameterlessGetArray.Invoke(arrayRecord, null) as Array;
            }

            // Multi-dimensional veya jagged array için expected array type gerekir.
            Type? expectedArrayType = ResolveKnownArrayType(
                arrayRecord.TypeName.AssemblyQualifiedName);

            if (expectedArrayType is null)
                return null;

            MethodInfo? getArrayWithType = typeof(ArrayRecord).GetMethod(
                "GetArray",
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types: new[] { typeof(Type), typeof(bool) },
                modifiers: null);

            if (getArrayWithType is null)
                return null;

            return getArrayWithType.Invoke(
                arrayRecord,
                new object[] { expectedArrayType, true }) as Array;
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
        if (memberName.StartsWith("<") &&
            memberName.Contains(">k__BackingField"))
        {
            int endIndex = memberName.IndexOf('>');
            return memberName.Substring(1, endIndex - 1);
        }

        if (memberName.StartsWith("_") && memberName.Length > 1)
            return memberName[1..];

        return memberName;
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