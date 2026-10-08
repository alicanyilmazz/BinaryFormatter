using System;
using System.Collections.Generic;
using System.Text;

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
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder = CharacterOrder.Keep,
        UnmappedCharacterBehavior unmappedCharacterBehavior = UnmappedCharacterBehavior.Keep);
}

public class CharacterMappingConverter : ICharacterMappingConverter
{
    public string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        CharacterOrder characterOrder = CharacterOrder.Keep,
        UnmappedCharacterBehavior unmappedCharacterBehavior = UnmappedCharacterBehavior.Keep)
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
        string mappedValue;

        if (characterMap.TryGetValue(character, out mappedValue))
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
                    string.Format(
                        "Character mapping not found. Character: '{0}', Hex: 0x{1:X4}",
                        character,
                        (int)character));

            default:
                result.Append(character);
                break;
        }
    }
}
