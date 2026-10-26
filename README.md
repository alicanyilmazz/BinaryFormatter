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

    // ASCII letters, digits and hyphens
    public const string AsciiLettersDigitsAndHyphens =
        @"\A[A-Za-z0-9\-]+\z";

    // ASCII letters, digits, spaces and hyphens
    public const string AsciiLettersDigitsSpacesAndHyphens =
        @"\A[A-Za-z0-9 \-]+\z";

    // ASCII letters, digits and backslashes
    public const string AsciiLettersDigitsAndBackslashes =
        @"\A[A-Za-z0-9\\]+\z";

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


    #region Additional Character Patterns

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

    // Letters, digits and @ signs
    public const string LettersDigitsAndAtSigns =
        @"\A[\p{L}\p{M}0-9@]+\z";

    // Letters, digits, dots and @ signs
    public const string LettersDigitsDotsAndAtSigns =
        @"\A[\p{L}\p{M}0-9.@]+\z";

    // Letters, digits, spaces, commas and dots
    public const string LettersDigitsSpacesCommasAndDots =
        @"\A[\p{L}\p{M}0-9 ,.]+\z";

    #endregion


    #region Numeric Patterns

    // Optional leading plus followed by digits
    public const string DigitsWithOptionalLeadingPlus =
        @"\A\+?[0-9]+\z";

    // Optional leading minus followed by digits
    public const string DigitsWithOptionalLeadingMinus =
        @"\A-?[0-9]+\z";

    // Optional leading plus or minus followed by digits
    public const string DigitsWithOptionalLeadingSign =
        @"\A[+\-]?[0-9]+\z";

    // Digits, spaces and plus signs
    public const string DigitsSpacesAndPlusSigns =
        @"\A[0-9 +]+\z";

    // Digits, spaces, plus signs and hyphens
    public const string DigitsSpacesPlusSignsAndHyphens =
        @"\A[0-9 +\-]+\z";

    // Unsigned decimal
    public const string UnsignedDecimal =
        @"\A[0-9]+(?:\.[0-9]+)?\z";

    // Signed decimal
    public const string SignedDecimal =
        @"\A[+\-]?[0-9]+(?:\.[0-9]+)?\z";

    #endregion


    #region Common Format Patterns

    // Common email address format
    public const string EmailAddress =
        @"\A[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(?:\.[A-Za-z0-9\-]+)+\z";

    // Hexadecimal characters
    public const string Hexadecimal =
        @"\A[0-9A-Fa-f]+\z";

    // Optional leading plus and 10-15 digits
    public const string PhoneNumber =
        @"\A\+?[0-9]{10,15}\z";

    #endregion
}
