public enum CharacterOrder
{
    Keep = 0,
    Reverse = 1
}

public enum UnmappedCharacterBehavior
{
    Keep = 0,
    Remove = 1,
    ThrowException = 2
}

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
}

public class CharacterMappingConverter : ICharacterMappingConverter
{
    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap)
    {
        return Convert(
            data,
            characterMap,
            CharacterOrder.Keep,
            UnmappedCharacterBehavior.Keep);
    }

    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder)
    {
        return Convert(
            data,
            characterMap,
            characterOrder,
            UnmappedCharacterBehavior.Keep);
    }

    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder,
        UnmappedCharacterBehavior unmappedCharacterBehavior)
    {
        if (string.IsNullOrEmpty(data))
            return data;

        if (characterMap == null)
            throw new ArgumentNullException(nameof(characterMap));

        var result = new StringBuilder();

        if (characterOrder == CharacterOrder.Reverse)
        {
            for (int i = data.Length - 1; i >= 0; i--)
            {
                AppendMappedCharacter(
                    result,
                    data[i],
                    characterMap,
                    unmappedCharacterBehavior);
            }
        }
        else
        {
            foreach (char character in data)
            {
                AppendMappedCharacter(
                    result,
                    character,
                    characterMap,
                    unmappedCharacterBehavior);
            }
        }

        return result.ToString();
    }

    private static void AppendMappedCharacter(
        StringBuilder result,
        char character,
        IReadOnlyDictionary<char, string> characterMap,
        UnmappedCharacterBehavior unmappedCharacterBehavior)
    {
        if (characterMap.TryGetValue(character, out string mappedValue))
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

            case UnmappedCharacterBehavior.ThrowException:
                throw new InvalidOperationException(
                    $"Character mapping not found. Character: '{character}', Hex: 0x{(int)character:X4}");

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(unmappedCharacterBehavior),
                    unmappedCharacterBehavior,
                    null);
        }
    }
}
