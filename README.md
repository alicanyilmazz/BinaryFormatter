public static class ValidationRegexPatterns
{
    #region Basic Patterns

    // Letters only (Unicode supported)
    public const string LettersOnly =
        @"\A[\p{L}\p{M}]+\z";

    // Digits only
    public const string DigitsOnly =
        @"\A[0-9]+\z";

    // Spaces only
    public const string SpacesOnly =
        @"\A[ ]+\z";

    // Letters and digits
    public const string LettersAndDigits =
        @"\A[\p{L}\p{M}0-9]+\z";

    // Letters and spaces
    public const string LettersAndSpaces =
        @"\A[\p{L}\p{M} ]+\z";

    // Digits and spaces
    public const string DigitsAndSpaces =
        @"\A[0-9 ]+\z";

    // Letters, digits and spaces
    public const string LettersDigitsAndSpaces =
        @"\A[\p{L}\p{M}0-9 ]+\z";

    #endregion


    #region ASCII Patterns

    // ASCII letters only
    public const string AsciiLettersOnly =
        @"\A[A-Za-z]+\z";

    // ASCII letters and digits
    public const string AsciiLettersAndDigits =
        @"\A[A-Za-z0-9]+\z";

    // ASCII letters and spaces
    public const string AsciiLettersAndSpaces =
        @"\A[A-Za-z ]+\z";

    // ASCII letters, digits and spaces
    public const string AsciiLettersDigitsAndSpaces =
        @"\A[A-Za-z0-9 ]+\z";

    #endregion


    #region Hyphen Patterns

    // Letters and hyphens
    public const string LettersAndHyphens =
        @"\A[\p{L}\p{M}\-]+\z";

    // Digits and hyphens
    public const string DigitsAndHyphens =
        @"\A[0-9\-]+\z";

    // Letters, digits and hyphens
    public const string LettersDigitsAndHyphens =
        @"\A[\p{L}\p{M}0-9\-]+\z";

    // Letters, spaces and hyphens
    public const string LettersSpacesAndHyphens =
        @"\A[\p{L}\p{M} \-]+\z";

    // Letters, digits, spaces and hyphens
    public const string LettersDigitsSpacesAndHyphens =
        @"\A[\p{L}\p{M}0-9 \-]+\z";

    #endregion


    #region Underscore Patterns

    // Letters and underscores
    public const string LettersAndUnderscores =
        @"\A[\p{L}\p{M}_]+\z";

    // Digits and underscores
    public const string DigitsAndUnderscores =
        @"\A[0-9_]+\z";

    // Letters, digits and underscores
    public const string LettersDigitsAndUnderscores =
        @"\A[\p{L}\p{M}0-9_]+\z";

    // Letters, digits, spaces and underscores
    public const string LettersDigitsSpacesAndUnderscores =
        @"\A[\p{L}\p{M}0-9 _]+\z";

    #endregion


    #region Dot Patterns

    // Letters and dots
    public const string LettersAndDots =
        @"\A[\p{L}\p{M}.]+\z";

    // Digits and dots
    public const string DigitsAndDots =
        @"\A[0-9.]+\z";

    // Letters, digits and dots
    public const string LettersDigitsAndDots =
        @"\A[\p{L}\p{M}0-9.]+\z";

    // Letters, digits, spaces and dots
    public const string LettersDigitsSpacesAndDots =
        @"\A[\p{L}\p{M}0-9 .]+\z";

    #endregion


    #region Combined Patterns

    // Letters, digits, hyphens and underscores
    public const string LettersDigitsHyphensAndUnderscores =
        @"\A[\p{L}\p{M}0-9_\-]+\z";

    // Letters, digits, spaces, hyphens and underscores
    public const string LettersDigitsSpacesHyphensAndUnderscores =
        @"\A[\p{L}\p{M}0-9 _\-]+\z";

    // Letters, digits, dots and underscores
    public const string LettersDigitsDotsAndUnderscores =
        @"\A[\p{L}\p{M}0-9._]+\z";

    // Letters, digits, dots and hyphens
    public const string LettersDigitsDotsAndHyphens =
        @"\A[\p{L}\p{M}0-9.\-]+\z";

    // Letters, digits, dots, hyphens and underscores
    public const string LettersDigitsDotsHyphensAndUnderscores =
        @"\A[\p{L}\p{M}0-9._\-]+\z";

    // Letters, digits, spaces, dots, hyphens and underscores
    public const string LettersDigitsSpacesDotsHyphensAndUnderscores =
        @"\A[\p{L}\p{M}0-9 ._\-]+\z";

    #endregion


    #region Additional Character Combinations

    // Letters, spaces and apostrophes
    public const string LettersSpacesAndApostrophes =
        @"\A[\p{L}\p{M} ']+\z";

    // Letters, spaces, hyphens and apostrophes
    public const string LettersSpacesHyphensAndApostrophes =
        @"\A[\p{L}\p{M} '\-]+\z";

    // Letters, digits and slashes
    public const string LettersDigitsAndSlashes =
        @"\A[\p{L}\p{M}0-9/]+\z";

    // Letters, digits, spaces and slashes
    public const string LettersDigitsSpacesAndSlashes =
        @"\A[\p{L}\p{M}0-9 /]+\z";

    // Letters, digits and @ character
    public const string LettersDigitsAndAtSigns =
        @"\A[\p{L}\p{M}0-9@]+\z";

    // Letters, digits, dots and @ character
    public const string LettersDigitsDotsAndAtSigns =
        @"\A[\p{L}\p{M}0-9.@]+\z";

    // Letters, digits, spaces, commas and dots
    public const string LettersDigitsSpacesCommasAndDots =
        @"\A[\p{L}\p{M}0-9 ,\.]+\z";

    #endregion


    #region Numeric Format Patterns

    // Optional leading plus followed by digits
    public const string DigitsWithOptionalLeadingPlus =
        @"\A\+?[0-9]+\z";

    // Optional leading minus followed by digits
    public const string DigitsWithOptionalLeadingMinus =
        @"\A-?[0-9]+\z";

    // Optional leading plus or minus followed by digits
    public const string DigitsWithOptionalLeadingSign =
        @"\A[+\-]?[0-9]+\z";

    // Unsigned decimal with dot separator
    public const string UnsignedDecimal =
        @"\A[0-9]+(?:\.[0-9]+)?\z";

    // Signed decimal with dot separator
    public const string SignedDecimal =
        @"\A[+\-]?[0-9]+(?:\.[0-9]+)?\z";

    #endregion


    #region Common Formats

    // Common email address format
    public const string EmailAddress =
        @"\A[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(?:\.[A-Za-z0-9\-]+)+\z";

    // Hexadecimal characters
    public const string Hexadecimal =
        @"\A[0-9A-Fa-f]+\z";

    #endregion
}


[Flags]
public enum AllowedCharacterSet
{
    None = 0,

    Letters = 1 << 0,

    Digits = 1 << 1,

    Spaces = 1 << 2,

    Hyphens = 1 << 3,

    Underscores = 1 << 4,

    Dots = 1 << 5,

    AtSigns = 1 << 6,

    PlusSigns = 1 << 7,

    Slashes = 1 << 8,

    Apostrophes = 1 << 9,

    Commas = 1 << 10,

    Colons = 1 << 11
}

public static string Build(AllowedCharacterSet allowed)
{
    if (allowed == AllowedCharacterSet.None)
        throw new ArgumentException(
            "At least one character set must be specified.",
            nameof(allowed));

    const AllowedCharacterSet supported =
        AllowedCharacterSet.Letters |
        AllowedCharacterSet.Digits |
        AllowedCharacterSet.Spaces |
        AllowedCharacterSet.Hyphens |
        AllowedCharacterSet.Underscores |
        AllowedCharacterSet.Dots |
        AllowedCharacterSet.AtSigns |
        AllowedCharacterSet.PlusSigns |
        AllowedCharacterSet.Slashes |
        AllowedCharacterSet.Apostrophes |
        AllowedCharacterSet.Commas |
        AllowedCharacterSet.Colons;

    if ((allowed & ~supported) != AllowedCharacterSet.None)
        throw new ArgumentOutOfRangeException(nameof(allowed));

    var chars = new StringBuilder();

    if ((allowed & AllowedCharacterSet.Letters) != 0)
        chars.Append(@"\p{L}\p{M}");

    if ((allowed & AllowedCharacterSet.Digits) != 0)
        chars.Append("0-9");

    if ((allowed & AllowedCharacterSet.Spaces) != 0)
        chars.Append(" ");

    if ((allowed & AllowedCharacterSet.Hyphens) != 0)
        chars.Append(@"\-");

    if ((allowed & AllowedCharacterSet.Underscores) != 0)
        chars.Append("_");

    if ((allowed & AllowedCharacterSet.Dots) != 0)
        chars.Append(@"\.");

    if ((allowed & AllowedCharacterSet.AtSigns) != 0)
        chars.Append("@");

    if ((allowed & AllowedCharacterSet.PlusSigns) != 0)
        chars.Append(@"\+");

    if ((allowed & AllowedCharacterSet.Slashes) != 0)
        chars.Append("/");

    if ((allowed & AllowedCharacterSet.Apostrophes) != 0)
        chars.Append("'");

    if ((allowed & AllowedCharacterSet.Commas) != 0)
        chars.Append(",");

    if ((allowed & AllowedCharacterSet.Colons) != 0)
        chars.Append(":");

    return @"\A[" + chars + @"]+\z";
}
