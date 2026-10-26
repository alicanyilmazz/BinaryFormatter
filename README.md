public static class ValidationRegexPatterns
{
    #region ASCII Patterns

    // ASCII letters and digits
    public const string AsciiLettersAndDigits =
        @"\A[A-Za-z0-9]+\z";

    // ASCII letters, digits, spaces and hyphens (-)
    public const string AsciiLettersDigitsSpacesAndHyphens =
        @"\A[A-Za-z0-9 \-]+\z";

    // ASCII letters, digits and backslashes (\)
    public const string AsciiLettersDigitsAndBackslashes =
        @"\A[A-Za-z0-9\\]+\z";

    // ASCII letters, digits, backslashes (\), hyphens (-) and underscores (_)
    public const string AsciiLettersDigitsBackslashesHyphensAndUnderscores =
        @"\A[A-Za-z0-9\\_-]+\z";

    #endregion


    #region Unicode Patterns

    // Unicode letters, digits, spaces, hyphens (-) and underscores (_)
    public const string LettersDigitsSpacesHyphensAndUnderscores =
        @"\A[\p{L}\p{M}0-9 _\-]+\z";

    #endregion


    #region Numeric Patterns

    // Digits, spaces and plus signs (+)
    public const string DigitsSpacesAndPlusSigns =
        @"\A[0-9 +]+\z";

    // Digits, spaces, plus signs (+) and hyphens (-)
    public const string DigitsSpacesPlusSignsAndHyphens =
        @"\A[0-9 +\-]+\z";

    #endregion
}
