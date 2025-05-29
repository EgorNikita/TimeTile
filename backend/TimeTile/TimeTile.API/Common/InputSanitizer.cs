namespace TimeTile.API.Common;

public static class InputSanitizer
{
    public static string Sanitize(string input)
    {
        return string.IsNullOrEmpty(input) ? input : System.Web.HttpUtility.HtmlEncode(input); // Basic HTML encoding
    }
}