using System.Web;

namespace TimeTile.API.Common;

public static class InputSanitizer
{
    public static string Sanitize(string input)
    {
        return string.IsNullOrEmpty(input) ? input : HttpUtility.HtmlEncode(input); // Basic HTML encoding
    }
}