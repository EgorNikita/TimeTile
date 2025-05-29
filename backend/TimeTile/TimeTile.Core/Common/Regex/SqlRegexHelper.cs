namespace TimeTile.Core.Common.Regex;

public static class SqlRegexHelper
{
    public static string SqlSafe(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        // Replace regex special characters to prevent SQL injection or syntax errors
        return pattern.Replace("'", "''") // Escape single quotes
            .Replace("\\", "\\\\") // Escape backslashes
            .Replace("\"", "\\\""); // Escape double quotes
    }
}