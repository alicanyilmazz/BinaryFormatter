using System;
using System.Collections.Generic;
using System.Text;

private static readonly Dictionary<char, char> ArabicMap =
    new Dictionary<char, char>
    {
        /*
         * NDC Double Arabic mapping.
         *
         * Sol taraf  : Switch'ten gelen karakter
         * Sağ taraf : Unicode Arabic karakter
         *
         * Dokümandaki mapping'in tamamını buraya ekle.
         */

        { '"',  'ا' },
        { '#',  'ا' },
        { '(',  'ت' },
        { '8',  'ب' },
        { '<',  'خ' },
        { '@',  'ر' },

        { 'C',  'س' },
        { 'D',  'ش' },
        { 'E',  'ص' },
        { 'F',  'ض' },
        { 'G',  'ط' },
        { 'H',  'ظ' },

        { 'I',  'ع' },
        { 'J',  'ع' },

        { 'K',  'غ' },
        { 'L',  'غ' },

        { 'M',  'ف' },

        { 'N',  'ق' },
        { 'O',  'ق' },

        { 'P',  'ك' },
        { 'Q',  'ك' },
        { 'R',  'ك' },
        { 'S',  'ك' },

        { 'T',  'ل' },
        { 'U',  'ل' },

        { 'W',  'م' },
        { 'X',  'م' },

        { 'Y',  'ن' },
        { 'Z',  'ن' },

        { '[',  'و' },

        // Bunları kendi dokümanındaki karşılıkla kontrol et.
        { '\\', 'ة' },
        { ']',  'ه' },

        { '^',  'ي' },
        { '_',  'ي' }

        /*
         * Buraya fotoğraftaki diğer special-character
         * mappinglerini de aynı şekilde ekle:
         *
         * { '!',  '...' },
         * { '$',  '...' },
         * { '%',  '...' },
         * { '&',  '...' },
         * { '\'', '...' },
         * { ')',  '...' },
         * { '*',  '...' },
         * ...
         */
    };


private static string ConvertArabicHeader(string rawHeader)
{
    if (string.IsNullOrEmpty(rawHeader))
        return rawHeader;

    var mapped = new StringBuilder(rawHeader.Length);

    foreach (char character in rawHeader)
    {
        char arabicCharacter;

        if (ArabicMap.TryGetValue(character, out arabicCharacter))
        {
            mapped.Append(arabicCharacter);
        }
        else
        {
            // Mapping yoksa karakteri kaybetme.
            // Space, sayı, noktalama vb. aynen kalır.
            mapped.Append(character);
        }
    }

    /*
     * NDC Double Arabic data visual order geliyor.
     *
     * Örnek:
     *
     * KU8XU"
     *
     * mapping sonrası:
     *
     * غلبملا
     *
     * reverse sonrası:
     *
     * المبلغ
     */

    char[] chars = mapped.ToString().ToCharArray();

    Array.Reverse(chars);

    string result = new string(chars);

    /*
     * Eğer header içerisinde sayı/date gibi LTR değerler
     * bulunursa tüm header reverse edildiği için onlar da
     * ters döner. Onları tekrar kendi yönüne çeviriyoruz.
     *
     * Örnek:
     *
     * 20-02-26
     *
     * reverse sonrası:
     *
     * 62-20-02
     *
     * RestoreNumericRuns sonrası:
     *
     * 20-02-26
     */

    return RestoreNumericRuns(result);
}


private static string RestoreNumericRuns(string value)
{
    if (string.IsNullOrEmpty(value))
        return value;

    char[] chars = value.ToCharArray();

    int index = 0;

    while (index < chars.Length)
    {
        if (!char.IsDigit(chars[index]))
        {
            index++;
            continue;
        }

        int start = index;

        while (index < chars.Length &&
               IsNumericCharacter(chars[index]))
        {
            index++;
        }

        int length = index - start;

        if (length > 1)
        {
            Array.Reverse(chars, start, length);
        }
    }

    return new string(chars);
}


private static bool IsNumericCharacter(char character)
{
    return char.IsDigit(character)
           || character == '.'
           || character == ','
           || character == '-'
           || character == '/'
           || character == ':';
}
