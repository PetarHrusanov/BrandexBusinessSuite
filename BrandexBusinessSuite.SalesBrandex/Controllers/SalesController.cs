using System.Collections.Immutable;
using System.Text;
using BrandexBusinessSuite.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BrandexBusinessSuite.SalesBrandex.Controllers;

public class SalesController : ApiController
{
    
    
    private static readonly ImmutableDictionary<char, string> cyrillicToLatinMapping = new Dictionary<char, string>
    {
        { 'а', "a"}, { 'А', "A"},
        { 'б', "b"}, { 'Б', "B"},
        { 'в', "v"}, { 'В', "V"},
        { 'г', "g"}, { 'Г', "G"},
        { 'д', "d"}, { 'Д', "D"},
        { 'е', "e"}, { 'Е', "E"},
        { 'ж', "zh"}, { 'Ж', "Zh"},
        { 'з', "z"}, { 'З', "Z"},
        { 'и', "i"}, { 'И', "I"},
        { 'й', "y"}, { 'Й', "Y"},
        { 'к', "k"}, { 'К', "K"},
        { 'л', "l"}, { 'Л', "L"},
        { 'м', "m"}, { 'М', "M"},
        { 'н', "n"}, { 'Н', "N"},
        { 'о', "o"}, { 'О', "O"},
        { 'п', "p"}, { 'П', "P"},
        { 'р', "r"}, { 'Р', "R"},
        { 'с', "s"}, { 'С', "S"},
        { 'т', "t"}, { 'Т', "T"},
        { 'у', "u"}, { 'У', "U"},
        { 'ф', "f"}, { 'Ф', "F"},
        { 'х', "h"}, { 'Х', "H"},
        { 'ц', "ts"}, { 'Ц', "Ts"},
        { 'ч', "ch"}, { 'Ч', "Ch"},
        { 'ш', "sh"}, { 'Ш', "Sh"},
        { 'щ', "sht"}, { 'Щ', "Sht"},
        { 'ъ', "a"}, { 'Ъ', "A"},
        { 'ь', "y"}, { 'Ь', "Y"},
        { 'ю', "yu"}, { 'Ю', "Yu"},
        { 'я', "ya"}, { 'Я', "Ya"},
        { ' ', " " }
    }.ToImmutableDictionary();

    private static readonly ImmutableArray<(string Latin, char Cyrillic)> latinToCyrillicMapping =
        cyrillicToLatinMapping
            .OrderByDescending(v => v.Value.Length)
            .Select(d => (Latin: d.Value, Cyrillic: d.Key))
            .ToImmutableArray();

    public static string CyrillicToLatin(string text)
        => string.Join("", text.ToCharArray().Select(c => cyrillicToLatinMapping[c]));

    public static string LatinToCyrillic(string text)
    {
        int startIdx = 0;
        StringBuilder accumulator = new();
        while (startIdx != text.Length)
        {
            foreach (var (latin, cyrillic) in latinToCyrillicMapping)
            {
                if (text[startIdx..].StartsWith(latin))
                {
                    accumulator.Append(cyrillic);
                    startIdx += latin.Length;
                    break;
                }
            }
        }
        return accumulator.ToString();
    }
}