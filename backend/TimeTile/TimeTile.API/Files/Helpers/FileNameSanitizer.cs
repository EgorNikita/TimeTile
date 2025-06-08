using System.Text.RegularExpressions;

namespace TimeTile.API.Files.Helpers
{
    public static class FileNameSanitizer
    {
        public static string MakeValidFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
            {
                "CON", "PRN", "AUX", "CLOCK$", "NUL",
                "COM0", "COM1", "COM2", "COM3", "COM4",
                "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");

            // Replace reserved words if they appear at the start, with or without an extension
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format(@"^{0}(\.|$)", Regex.Escape(reservedWord));
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_$1", RegexOptions.IgnoreCase);
            }

            // Trim trailing dots and spaces
            sanitisedNamePart = sanitisedNamePart.TrimEnd('.', ' ');

            return sanitisedNamePart;
        }

    }
}
