// Unicode letters, digits, spaces, commas, dots, slashes and hyphens
// Empty string is allowed
public const string LettersDigitsSpacesCommasDotsSlashesAndHyphensAllowEmpty =
    @"\A[\p{L}\p{M}0-9 ,./-]*\z";

// Unicode letters, digits and spaces
// Empty string is allowed
public const string LettersDigitsAndSpacesAllowEmpty =
    @"\A[\p{L}\p{M}0-9 ]*\z";

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


public void MaxLength(
    string value,
    int maxLength,
    string field,
    string message)
{
    if (value == null)
        return;

    if (value.Length > maxLength)
        Result.Add(field, message);
}
