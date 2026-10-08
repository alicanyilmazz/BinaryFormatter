public interface ITextConverter
{
    string Convert(
        string data,
        IReadOnlyDictionary<char, string> characterMap,
        TextConversionOptions options = null);
}

using System;
using System.Collections.Generic;
using System.Text;

public enum TextOrderMode
{
    Keep = 0,
    Reverse = 1
}

[Flags]
public enum PreserveRunType
{
    None = 0,
    Numeric = 1
}

public enum UnmappedCharacterBehavior
{
    Keep = 0,
    Remove = 1,
    ThrowException = 2
}

public class TextConversionOptions
{
    /// <summary>
    /// Gelen datanın karakter sırası korunacak mı,
    /// yoksa ters çevrilecek mi?
    /// </summary>
    public TextOrderMode OrderMode { get; set; }

    /// <summary>
    /// Reverse sırasında kendi iç sırası korunması gereken
    /// veri tipleri.
    ///
    /// Örn:
    /// 20-02-26
    /// 130.00
    ///
    /// Numeric seçilirse bunların kendi iç sırası bozulmaz.
    /// </summary>
    public PreserveRunType PreserveRuns { get; set; }

    /// <summary>
    /// Numeric run içerisinde kabul edilecek ayraçlar.
    /// İstenirse dışarıdan değiştirilebilir.
    /// </summary>
    public string NumericSeparators { get; set; }

    /// <summary>
    /// CharacterMap içerisinde bulunmayan bir karakter
    /// geldiğinde ne yapılacağı.
    /// </summary>
    public UnmappedCharacterBehavior UnmappedCharacterBehavior { get; set; }

    public TextConversionOptions()
    {
        OrderMode = TextOrderMode.Keep;
        PreserveRuns = PreserveRunType.None;
        NumericSeparators = ".,-/:";
        UnmappedCharacterBehavior = UnmappedCharacterBehavior.Keep;
    }
}
