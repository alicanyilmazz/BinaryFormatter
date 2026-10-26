public void MustMatchRegex(
    string value,
    string pattern,
    string field,
    string message)
{
    if (value == null)
        return;

    try
    {
        var isMatch = Regex.IsMatch(
            value,
            pattern,
            RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(200));

        if (!isMatch)
            Result.Add(field, message);
    }
    catch (RegexMatchTimeoutException)
    {
        Result.Add(field, message);
    }
}

public static class ValidationRegexPatterns
{
    #region ASCII Patterns

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


    #region Unicode Patterns

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
