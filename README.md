public class CharacterMappingOptions
{
    public CharacterOrder CharacterOrder { get; set; }

    public UnmappedCharacterBehavior UnmappedCharacterBehavior { get; set; }

    public TextSplitMode SplitMode { get; set; }

    public string SplitPattern { get; set; }

    public TextGroupOrder GroupOrder { get; set; }
}

using System.Collections.Generic;

public interface ICharacterMappingConverter
{
    string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap);

    string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder);

    string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder,
        UnmappedCharacterBehavior unmappedCharacterBehavior);

    string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterMappingOptions options);
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class CharacterMappingConverter : ICharacterMappingConverter
{
    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap)
    {
        return Convert(
            data,
            characterMap,
            new CharacterMappingOptions());
    }

    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder)
    {
        return Convert(
            data,
            characterMap,
            new CharacterMappingOptions
            {
                CharacterOrder = characterOrder
            });
    }

    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder,
        UnmappedCharacterBehavior unmappedCharacterBehavior)
    {
        return Convert(
            data,
            characterMap,
            new CharacterMappingOptions
            {
                CharacterOrder = characterOrder,
                UnmappedCharacterBehavior = unmappedCharacterBehavior
            });
    }

    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterMappingOptions options)
    {
        try
        {
            if (string.IsNullOrEmpty(data))
                return data;

            if (characterMap == null)
                throw new ArgumentNullException(nameof(characterMap));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.SplitMode == TextSplitMode.None)
            {
                return ConvertCharacters(
                    data,
                    characterMap,
                    options.CharacterOrder,
                    options.UnmappedCharacterBehavior);
            }

            return ConvertGroups(
                data,
                characterMap,
                options);
        }
        catch (Exception ex)
        {
            AtmLogManager.Log.Error(
                "Error while converting characters",
                ex);

            return data;
        }
    }

    private static string ConvertGroups(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterMappingOptions options)
    {
        if (string.IsNullOrEmpty(options.SplitPattern))
        {
            throw new ArgumentException(
                "SplitPattern cannot be null or empty.",
                nameof(options.SplitPattern));
        }

        string pattern;

        switch (options.SplitMode)
        {
            case TextSplitMode.Literal:
                pattern = Regex.Escape(options.SplitPattern);
                break;

            case TextSplitMode.Regex:
                pattern = options.SplitPattern;
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(options.SplitMode));
        }

        var regex = new Regex(
            pattern,
            RegexOptions.None,
            TimeSpan.FromMilliseconds(500));

        var groups = new List<string>();
        var separators = new List<string>();

        int currentIndex = 0;

        foreach (Match match in regex.Matches(data))
        {
            if (match.Length == 0)
            {
                throw new ArgumentException(
                    "SplitPattern cannot match an empty string.");
            }

            groups.Add(
                data.Substring(
                    currentIndex,
                    match.Index - currentIndex));

            separators.Add(match.Value);

            currentIndex = match.Index + match.Length;
        }

        groups.Add(data.Substring(currentIndex));

        switch (options.GroupOrder)
        {
            case TextGroupOrder.Keep:
                break;

            case TextGroupOrder.Reverse:
                ReverseGroups(groups);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(options.GroupOrder));
        }

        var result = new StringBuilder();

        for (int i = 0; i < groups.Count; i++)
        {
            string convertedGroup = ConvertCharacters(
                groups[i],
                characterMap,
                options.CharacterOrder,
                options.UnmappedCharacterBehavior);

            result.Append(convertedGroup);

            if (i < separators.Count)
            {
                result.Append(separators[i]);
            }
        }

        return result.ToString();
    }

    private static void ReverseGroups(List<string> groups)
    {
        var indexes = new List<int>();

        for (int i = 0; i < groups.Count; i++)
        {
            if (!string.IsNullOrEmpty(groups[i]))
            {
                indexes.Add(i);
            }
        }

        int left = 0;
        int right = indexes.Count - 1;

        while (left < right)
        {
            int leftIndex = indexes[left];
            int rightIndex = indexes[right];

            string temp = groups[leftIndex];

            groups[leftIndex] = groups[rightIndex];
            groups[rightIndex] = temp;

            left++;
            right--;
        }
    }

    private static string ConvertCharacters(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder,
        UnmappedCharacterBehavior unmappedCharacterBehavior)
    {
        var result = new StringBuilder(data.Length);

        switch (characterOrder)
        {
            case CharacterOrder.Keep:

                foreach (char character in data)
                {
                    AppendMappedCharacter(
                        result,
                        character,
                        characterMap,
                        unmappedCharacterBehavior);
                }

                break;

            case CharacterOrder.Reverse:

                for (int i = data.Length - 1; i >= 0; i--)
                {
                    AppendMappedCharacter(
                        result,
                        data[i],
                        characterMap,
                        unmappedCharacterBehavior);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(characterOrder));
        }

        return result.ToString();
    }

    private static void AppendMappedCharacter(
        StringBuilder result,
        char character,
        IReadOnlyDictionary<char, string> characterMap,
        UnmappedCharacterBehavior unmappedCharacterBehavior)
    {
        if (characterMap.TryGetValue(
            character,
            out string mappedValue))
        {
            result.Append(mappedValue);
            return;
        }

        switch (unmappedCharacterBehavior)
        {
            case UnmappedCharacterBehavior.Keep:

                result.Append(character);
                break;

            case UnmappedCharacterBehavior.Remove:

                break;

            default:

                throw new ArgumentOutOfRangeException(
                    nameof(unmappedCharacterBehavior));
        }
    }
}
