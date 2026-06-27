using System.Formats.Nrbf;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace App.Parser.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
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

            using var stream = new MemoryStream(bytes);

            SerializationRecord record = NrbfDecoder.Decode(stream);

            object? result = DumpValue(record, new HashSet<string>(), 0);

            return Ok(new
            {
                success = true,
                data = result
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

        if (value is string ||
            value is bool ||
            value is byte ||
            value is sbyte ||
            value is short ||
            value is ushort ||
            value is int ||
            value is uint ||
            value is long ||
            value is ulong ||
            value is float ||
            value is double ||
            value is decimal ||
            value is char ||
            value is DateTime ||
            value is TimeSpan ||
            value is Guid)
        {
            return value;
        }

        if (value is byte[] byteArray)
        {
            return "0x" + Convert.ToHexString(byteArray);
        }

        if (value is PrimitiveTypeRecord primitiveRecord)
        {
            return primitiveRecord.Value;
        }

        if (value is ClassRecord classRecord)
        {
            return DumpClassRecord(classRecord, visitedRecords, depth);
        }

        if (value is ArrayRecord arrayRecord)
        {
            return DumpArrayRecord(arrayRecord, visitedRecords, depth);
        }

        if (value is Array runtimeArray)
        {
            return DumpRuntimeArray(runtimeArray, visitedRecords, depth);
        }

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

        var result = new Dictionary<string, object?>
        {
            ["$id"] = recordId,
            ["$recordType"] = classRecord.RecordType.ToString(),
            ["$type"] = classRecord.TypeName.AssemblyQualifiedName
        };

        foreach (string memberName in classRecord.MemberNames)
        {
            string displayName = NormalizeMemberName(memberName);

            try
            {
                object? rawValue = classRecord.GetRawValue(memberName);
                result[displayName] = DumpValue(rawValue, visitedRecords, depth + 1);
            }
            catch (Exception ex)
            {
                result[displayName] = $"<read-error: {ex.GetType().Name}>";
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
        {
            return "0x" + Convert.ToHexString(byteArray);
        }

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
            Type type = arrayRecord.GetType();

            // SZArrayRecord<T> için: GetArray()
            MethodInfo? parameterlessGetArray = type.GetMethod(
                "GetArray",
                Type.EmptyTypes);

            if (parameterlessGetArray is not null)
            {
                return parameterlessGetArray.Invoke(arrayRecord, null) as Array;
            }

            // ArrayRecord için: GetArray(Type expectedArrayType, bool allowNulls)
            MethodInfo? typedGetArray = type.GetMethod(
                "GetArray",
                new[] { typeof(Type), typeof(bool) });

            if (typedGetArray is not null)
            {
                return typedGetArray.Invoke(
                    arrayRecord,
                    new object[] { typeof(object[]), true }) as Array;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static string NormalizeMemberName(string memberName)
    {
        // <AccountNumber>k__BackingField -> AccountNumber
        if (memberName.StartsWith("<") &&
            memberName.Contains(">k__BackingField"))
        {
            int endIndex = memberName.IndexOf('>');
            return memberName.Substring(1, endIndex - 1);
        }

        // _accountNumber -> accountNumber
        if (memberName.StartsWith("_") && memberName.Length > 1)
        {
            return memberName[1..];
        }

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
            {
                hex = hex[2..];
            }

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

public class BinaryFormatterRequest
{
    public string Data { get; set; } = string.Empty;
}