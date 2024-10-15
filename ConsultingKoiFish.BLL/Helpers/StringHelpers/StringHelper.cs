using System.Globalization;
using System.Text.RegularExpressions;

namespace ConsultingKoiFish.BLL.Helpers.StringHelpers;

public class StringHelper
{
    /// <summary>
    /// This is used to convert a string to camelCase or PascalCase
    /// </summary>
    /// <param name="input"></param>
    /// <param name="isCamelCase"></param>
    /// <returns></returns>
    public static string ConvertToPascalOrCamelCase(string input, bool isCamelCase = false)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        string cleanedInput = Regex.Replace(input, @"[^a-zA-Z0-9]", " "); 
        cleanedInput = Regex.Replace(cleanedInput, @"^\d+", ""); 
        
        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        string pascalCase = textInfo.ToTitleCase(cleanedInput.ToLower()).Replace(" ", "");
        
        if (isCamelCase && pascalCase.Length > 0)
        {
            return char.ToLower(pascalCase[0]) + pascalCase.Substring(1);
        }

        return pascalCase;
    }
}