public static class ValidationRegexPatterns
{
    // Letters and spaces only (Unicode supported)
    // Example: Alican Yılmaz
    public const string Name =
        @"\A[\p{L}\p{M} ]+\z";

    // Letters, numbers and spaces
    // Example: Alican123 Yılmaz
    public const string NameWithNumbers =
        @"\A[\p{L}\p{M}0-9 ]+\z";

    // Letters, numbers and spaces
    // Blocks special characters
    public const string NoSpecialCharacters =
        @"\A[\p{L}\p{M}0-9 ]+\z";

    // Letters and numbers, without spaces
    // Example: Customer123
    public const string AlphaNumeric =
        @"\A[\p{L}\p{M}0-9]+\z";

    // Numbers only
    // Example: 123456789
    public const string NumbersOnly =
        @"\A[0-9]+\z";

    // Common email address format
    // Example: test.user123@gmail.com
    public const string Email =
        @"\A[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(?:\.[A-Za-z0-9\-]+)+\z";
}

public void MustMatchRegex(
    string value,
    string pattern,
    string field,
    string message)
{
    if (string.IsNullOrWhiteSpace(value))
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

ctx.When(m.Customer != null, () =>
{
    ctx.Required(
        m.Customer.Name,
        nameof(m.Customer.Name),
        "Customer name is required.");

    ctx.MustMatchRegex(
        m.Customer.Name,
        ValidationRegexPatterns.NameWithNumbers,
        nameof(m.Customer.Name),
        "Customer name contains invalid characters.");
});

ctx.Required(
    m.Customer.Email,
    nameof(m.Customer.Email),
    "Email is required.");

ctx.MustMatchRegex(
    m.Customer.Email,
    ValidationRegexPatterns.Email,
    nameof(m.Customer.Email),
    "Email format is invalid.");

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
