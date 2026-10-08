private static readonly Dictionary<char, char> ArabicMap =
    new Dictionary<char, char>
    {
        // Bunlar elimizdeki örneklerden bildiklerimiz.

        { '"', 'ا' },
        { '#', 'ا' },
        { '(', 'ت' },
        { '8', 'ب' },
        { '<', 'خ' },
        { '@', 'ر' },

        { 'I', 'ع' },
        { 'J', 'ع' },
        { 'K', 'غ' },

        { 'U', 'ل' },
        { 'X', 'م' },
        { 'Z', 'ن' },

        { '[', 'و' },
        { '\\', 'ة' },
        { '_', 'ي' },

        // ------------------------------------
        // Dokümandaki geri kalan mappingleri
        // buraya sen ekleyeceksin.
        // ------------------------------------

        // Örnek:
        // { 'C', 'س' },
        // { 'D', 'ش' },
        // { 'E', 'ص' },
        // { 'F', 'ض' },
        // ...
    };

    private static string ConvertArabicHeader(string rawHeader)
{
    if (string.IsNullOrEmpty(rawHeader))
        return rawHeader;

    // NDC Arabic header visual order geliyor.
    // Önce tamamını ters çeviriyoruz.
    char[] chars = rawHeader.ToCharArray();
    Array.Reverse(chars);

    var result = new StringBuilder(chars.Length);

    foreach (char c in chars)
    {
        char arabicChar;

        if (ArabicMap.TryGetValue(c, out arabicChar))
        {
            result.Append(arabicChar);
        }
        else
        {
            // Space veya mapping'de olmayan bir şey geldiyse
            // veri kaybolmasın, olduğu gibi bırak.
            result.Append(c);
        }
    }

    return result.ToString();
}

// Language
string langCode = data.Substring(0, 2);

bool isArabic = langCode.StartsWith(
    "A",
    StringComparison.OrdinalIgnoreCase);

result.LanguageInfo =
    langCode.StartsWith("E", StringComparison.OrdinalIgnoreCase)
        ? "English"
        : isArabic
            ? "Arabic"
            : "Unknown";

string remaining = data.Substring(2);

if (remaining.Length < LineLength)
{
    throw Commons.Common.GenerateHostException(
        "HeaderLengthInsufficient",
        false);
}

// Header
string rawHeader = remaining.Substring(0, LineLength);

result.HeaderRawData = isArabic
    ? ConvertArabicHeader(rawHeader)
    : rawHeader;


// Transactions
// Bunlara KESİNLİKLE Arabic conversion uygulanmıyor.

string rowsBlock = remaining.Substring(LineLength);

for (int i = 0;
     i + LineLength <= rowsBlock.Length;
     i += LineLength)
{
    string rawLine =
        rowsBlock.Substring(i, LineLength);

    if (string.IsNullOrWhiteSpace(rawLine))
        continue;

    result.Transactions.Add(
        new OffusMiniStatementTransactions
        {
            RawData = rawLine
        });
}
