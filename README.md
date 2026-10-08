public class MappedTextConverter : ITextConverter
{
    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        TextConversionOptions options = null)
    {
        if (string.IsNullOrEmpty(data))
            return data;

        if (characterMap == null)
            throw new ArgumentNullException(nameof(characterMap));

        if (options == null)
            options = new TextConversionOptions();

        List<TextRun> runs = CreateRuns(data, options);

        /*
         * Örneğin:
         *
         * ABC 20-02-26 DEF
         *
         * Reverse + Numeric Preserve:
         *
         * Run'lar:
         * [ABC ]
         * [20-02-26] -> Preserve
         * [ DEF]
         *
         * Run sırası ters çevrilir ama numeric run'ın
         * kendi karakter sırası korunur.
         */
        if (options.OrderMode == TextOrderMode.Reverse)
        {
            runs.Reverse();
        }

        var result = new StringBuilder();

        foreach (TextRun run in runs)
        {
            if (options.OrderMode == TextOrderMode.Reverse &&
                !run.PreserveOrder)
            {
                AppendReverseMapped(
                    result,
                    run.Value,
                    characterMap,
                    options);
            }
            else
            {
                AppendMapped(
                    result,
                    run.Value,
                    characterMap,
                    options);
            }
        }

        return result.ToString();
    }

    private static List<TextRun> CreateRuns(
        string data,
        TextConversionOptions options)
    {
        var runs = new List<TextRun>();

        /*
         * Hiçbir özel run korunmayacaksa
         * tüm text tek run olarak yeterli.
         */
        if (options.PreserveRuns == PreserveRunType.None)
        {
            runs.Add(new TextRun
            {
                Value = data,
                PreserveOrder = false
            });

            return runs;
        }

        var normalRun = new StringBuilder();

        int index = 0;

        while (index < data.Length)
        {
            /*
             * Numeric preservation aktifse
             * sayı/date/amount gibi bir alan yakalamaya çalış.
             */
            if (HasOption(
                    options.PreserveRuns,
                    PreserveRunType.Numeric) &&
                IsNumericRunStart(data, index))
            {
                AddNormalRunIfExists(runs, normalRun);

                int endIndex = ReadNumericRun(
                    data,
                    index,
                    options.NumericSeparators);

                string numericValue =
                    data.Substring(index, endIndex - index);

                runs.Add(new TextRun
                {
                    Value = numericValue,
                    PreserveOrder = true
                });

                index = endIndex;
                continue;
            }

            normalRun.Append(data[index]);
            index++;
        }

        AddNormalRunIfExists(runs, normalRun);

        return runs;
    }

    private static bool IsNumericRunStart(
        string data,
        int index)
    {
        if (index >= data.Length)
            return false;

        char character = data[index];

        if (char.IsDigit(character))
            return true;

        /*
         * Negatif veya pozitif sayı:
         *
         * -130.00
         * +130.00
         */
        if ((character == '-' || character == '+') &&
            index + 1 < data.Length &&
            char.IsDigit(data[index + 1]))
        {
            return true;
        }

        return false;
    }

    private static int ReadNumericRun(
        string data,
        int startIndex,
        string numericSeparators)
    {
        int index = startIndex;

        /*
         * Başında + veya - varsa dahil et.
         */
        if (index < data.Length &&
            (data[index] == '+' || data[index] == '-'))
        {
            index++;
        }

        while (index < data.Length)
        {
            char character = data[index];

            if (char.IsDigit(character))
            {
                index++;
                continue;
            }

            if (IsNumericSeparator(
                    character,
                    numericSeparators) &&
                index + 1 < data.Length &&
                char.IsDigit(data[index + 1]))
            {
                index++;
                continue;
            }

            break;
        }

        return index;
    }

    private static bool IsNumericSeparator(
        char character,
        string numericSeparators)
    {
        if (string.IsNullOrEmpty(numericSeparators))
            return false;

        return numericSeparators.IndexOf(character) >= 0;
    }

    private static void AddNormalRunIfExists(
        List<TextRun> runs,
        StringBuilder normalRun)
    {
        if (normalRun.Length == 0)
            return;

        runs.Add(new TextRun
        {
            Value = normalRun.ToString(),
            PreserveOrder = false
        });

        normalRun.Clear();
    }

    private static void AppendMapped(
        StringBuilder result,
        string value,
        IReadOnlyDictionary<char, string> characterMap,
        TextConversionOptions options)
    {
        foreach (char character in value)
        {
            AppendCharacter(
                result,
                character,
                characterMap,
                options);
        }
    }

    private static void AppendReverseMapped(
        StringBuilder result,
        string value,
        IReadOnlyDictionary<char, string> characterMap,
        TextConversionOptions options)
    {
        for (int i = value.Length - 1; i >= 0; i--)
        {
            AppendCharacter(
                result,
                value[i],
                characterMap,
                options);
        }
    }

    private static void AppendCharacter(
        StringBuilder result,
        char character,
        IReadOnlyDictionary<char, string> characterMap,
        TextConversionOptions options)
    {
        string mappedValue;

        if (characterMap.TryGetValue(
                character,
                out mappedValue))
        {
            result.Append(mappedValue);
            return;
        }

        switch (options.UnmappedCharacterBehavior)
        {
            case UnmappedCharacterBehavior.Keep:
                result.Append(character);
                break;

            case UnmappedCharacterBehavior.Remove:
                break;

            case UnmappedCharacterBehavior.ThrowException:
                throw new InvalidOperationException(
                    string.Format(
                        "Character mapping not found. Character: '{0}', Hex: 0x{1:X4}",
                        character,
                        (int)character));

            default:
                result.Append(character);
                break;
        }
    }

    private static bool HasOption(
        PreserveRunType value,
        PreserveRunType option)
    {
        return (value & option) == option;
    }

    private sealed class TextRun
    {
        public string Value { get; set; }

        public bool PreserveOrder { get; set; }
    }
}
