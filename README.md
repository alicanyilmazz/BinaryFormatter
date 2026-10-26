public static class ValidationRegexPatterns
{
    #region ASCII Letter Patterns

    // ASCII letters only
    // Empty string is NOT allowed
    public const string AsciiLettersOnly =
        @"\A[A-Za-z]+\z";

    // ASCII letters only
    // Empty string is allowed
    public const string AsciiLettersOnlyAllowEmpty =
        @"\A[A-Za-z]*\z";


    // ASCII letters and backslashes (\)
    // Empty string is NOT allowed
    public const string AsciiLettersAndBackslashes =
        @"\A[A-Za-z\\]+\z";

    // ASCII letters and backslashes (\)
    // Empty string is allowed
    public const string AsciiLettersAndBackslashesAllowEmpty =
        @"\A[A-Za-z\\]*\z";

    #endregion


    #region ASCII Letters And Digits Patterns

    // ASCII letters and digits
    // Empty string is NOT allowed
    public const string AsciiLettersAndDigits =
        @"\A[A-Za-z0-9]+\z";


    // ASCII letters, digits, spaces and hyphens (-)
    // Empty string is NOT allowed
    public const string AsciiLettersDigitsSpacesAndHyphens =
        @"\A[A-Za-z0-9 \-]+\z";


    // ASCII letters, digits and backslashes (\)
    // Empty string is NOT allowed
    public const string AsciiLettersDigitsAndBackslashes =
        @"\A[A-Za-z0-9\\]+\z";


    // ASCII letters, digits, hyphens (-) and underscores (_)
    // Empty string is NOT allowed
    public const string AsciiLettersDigitsHyphensAndUnderscores =
        @"\A[A-Za-z0-9_-]+\z";

    // ASCII letters, digits, hyphens (-) and underscores (_)
    // Empty string is allowed
    public const string AsciiLettersDigitsHyphensAndUnderscoresAllowEmpty =
        @"\A[A-Za-z0-9_-]*\z";


    // ASCII letters, digits, backslashes (\), hyphens (-) and underscores (_)
    // Empty string is NOT allowed
    public const string AsciiLettersDigitsBackslashesHyphensAndUnderscores =
        @"\A[A-Za-z0-9\\_-]+\z";

    // ASCII letters, digits, backslashes (\), hyphens (-) and underscores (_)
    // Empty string is allowed
    public const string AsciiLettersDigitsBackslashesHyphensAndUnderscoresAllowEmpty =
        @"\A[A-Za-z0-9\\_-]*\z";

    #endregion


    #region Unicode Letter Patterns

    // Unicode letters only
    // Empty string is NOT allowed
    public const string LettersOnly =
        @"\A[\p{L}\p{M}]+\z";

    // Unicode letters only
    // Empty string is allowed
    public const string LettersOnlyAllowEmpty =
        @"\A[\p{L}\p{M}]*\z";


    // Unicode letters and backslashes (\)
    // Empty string is NOT allowed
    public const string LettersAndBackslashes =
        @"\A[\p{L}\p{M}\\]+\z";

    // Unicode letters and backslashes (\)
    // Empty string is allowed
    public const string LettersAndBackslashesAllowEmpty =
        @"\A[\p{L}\p{M}\\]*\z";

    #endregion


    #region Unicode Letters And Digits Patterns

    // Unicode letters, digits, hyphens (-) and underscores (_)
    // Empty string is NOT allowed
    public const string LettersDigitsHyphensAndUnderscores =
        @"\A[\p{L}\p{M}0-9_-]+\z";

    // Unicode letters, digits, hyphens (-) and underscores (_)
    // Empty string is allowed
    public const string LettersDigitsHyphensAndUnderscoresAllowEmpty =
        @"\A[\p{L}\p{M}0-9_-]*\z";


    // Unicode letters, digits, backslashes (\), hyphens (-) and underscores (_)
    // Empty string is NOT allowed
    public const string LettersDigitsBackslashesHyphensAndUnderscores =
        @"\A[\p{L}\p{M}0-9\\_-]+\z";

    // Unicode letters, digits, backslashes (\), hyphens (-) and underscores (_)
    // Empty string is allowed
    public const string LettersDigitsBackslashesHyphensAndUnderscoresAllowEmpty =
        @"\A[\p{L}\p{M}0-9\\_-]*\z";


    // Unicode letters, digits, spaces, hyphens (-) and underscores (_)
    // Empty string is NOT allowed
    public const string LettersDigitsSpacesHyphensAndUnderscores =
        @"\A[\p{L}\p{M}0-9 _\-]+\z";

    #endregion


    #region Numeric Patterns

    // Digits, spaces and plus signs (+)
    // Empty string is NOT allowed
    public const string DigitsSpacesAndPlusSigns =
        @"\A[0-9 +]+\z";

    // Digits, spaces and plus signs (+)
    // Empty string is allowed
    public const string DigitsSpacesAndPlusSignsAllowEmpty =
        @"\A[0-9 +]*\z";


    // Digits, spaces, plus signs (+) and hyphens (-)
    // Empty string is NOT allowed
    public const string DigitsSpacesPlusSignsAndHyphens =
        @"\A[0-9 +\-]+\z";

    // Digits, spaces, plus signs (+) and hyphens (-)
    // Empty string is allowed
    public const string DigitsSpacesPlusSignsAndHyphensAllowEmpty =
        @"\A[0-9 +\-]*\z";

    #endregion
}
