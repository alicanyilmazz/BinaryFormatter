public static class ValidationRegexPatterns
{
    // Letters only, spaces allowed
    public const string Name =
        @"\A[\p{L}\p{M} ]+\z";

    // Letters and numbers, spaces allowed
    public const string NameWithNumbers =
        @"\A[\p{L}\p{M}0-9 ]+\z";

    // Letters and numbers, no spaces
    public const string AlphaNumeric =
        @"\A[\p{L}\p{M}0-9]+\z";

    // ASCII letters and numbers, no spaces
    public const string CustomerId =
        @"\A[A-Za-z0-9]+\z";

    // Numbers only
    public const string NumbersOnly =
        @"\A[0-9]+\z";

    // Phone number: optional +, 10-15 digits
    public const string PhoneNumber =
        @"\A\+?[0-9]{10,15}\z";

    // Common email format
    public const string Email =
        @"\A[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(?:\.[A-Za-z0-9\-]+)+\z";
}
