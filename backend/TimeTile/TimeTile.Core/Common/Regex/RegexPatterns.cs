using System.Text.RegularExpressions;

namespace TimeTile.Core.Common.Regex;

public static class RegexPatterns
{
    public static readonly Dictionary<string, (System.Text.RegularExpressions.Regex Pattern, short MaxLength, string Description)> Patterns = new()
    {
        ["Title"] = (new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9\s\-.,_&()]+$", RegexOptions.Compiled), 100, "Titles must contain letters, numbers, spaces, and punctuation like -.,_&()."),
        ["Name"] = (new System.Text.RegularExpressions.Regex(@"^[[:alpha:]]+(?:[\s'-][[:alpha:]]+)*$", RegexOptions.Compiled), 100, "Full names may include letters, apostrophes, hyphens, and spaces."),
        ["Address"] = (new System.Text.RegularExpressions.Regex(@"^[[:alpha:]\d\s'.,#/()-]+$", RegexOptions.Compiled), 200, "Addresses may contain letters, numbers, spaces, and punctuation like '.,#/-()."),
        ["Email"] = (new System.Text.RegularExpressions.Regex(@"^(?!\.)[A-Za-z0-9._%+-]+(?<!\.)@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$", RegexOptions.Compiled), 100, "Must be a valid email format like user@example.com."),
        ["PhoneE164"] = (new System.Text.RegularExpressions.Regex(@"^\+[1-9]\d{6,14}$", RegexOptions.Compiled), 15, "Phone number must follow the E.164 format, e.g., +1234567890."),
        ["Domain"] = (new System.Text.RegularExpressions.Regex(@"^(?:[[:alpha:]0-9-]{1,63}\.)+[A-Za-z]{2,}$", RegexOptions.Compiled), 255, "Domain must be valid and support international formats like sub.domain.com."),
        ["Description"] = (new System.Text.RegularExpressions.Regex(@"^[[:alpha:]\d\s.,!?]+$", RegexOptions.Compiled), 250, "Descriptions may include letters, numbers, spaces, and punctuation like .,!?"),
    };
}