using SkiaSharp;
using TimeTile.API.Users.Services.Interfaces;

namespace TimeTile.API.Users.Services;

public class AvatarService : IAvatarService
{
    public Task<Stream> GenerateDefaultAvatar(string firstname, string lastname, int size = 100)
    {
        return Task.Run(() =>
        {
            var initials = GetInitials(firstname, lastname);
            var backgroundColor = GetColorFromName($"{firstname} {lastname}");
            
            using var bitmap = new SKBitmap(100, 100);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Transparent);

            // Draw square background
            using var backgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = backgroundColor,
                IsAntialias = true
            };
            canvas.DrawRect(0, 0, size, size, backgroundPaint);
            
            // Create font
            float fontSize = size * 0.4f;
            using var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold);
            using var font = new SKFont(typeface, fontSize);

            // Create paint for text
            using var textPaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true
            };

            // Measure and center text
            font.MeasureText(initials, out var textBounds, textPaint);
            float x = size / 2f - textBounds.MidX;
            float y = size / 2f - textBounds.MidY;

            canvas.DrawText(initials, x, y, font, textPaint);

            // Encode to memory stream
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            var stream = new MemoryStream();
            data.SaveTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return (Stream)stream;
        });
    }
    
    private string GetInitials(string firstname, string lastname)
    {
        var firstInitial = string.IsNullOrEmpty(firstname) ? "" : firstname.Trim().ToUpper()[0].ToString();
        var lastInitial = string.IsNullOrEmpty(lastname) ? "" : lastname.Trim().ToUpper()[0].ToString();
        return $"{firstInitial}{lastInitial}";
    }

    private SKColor GetColorFromName(string name)
    {
        var hash = name.GetHashCode();
        var r = (byte)((hash >> 16) & 0xFF);
        var g = (byte)((hash >> 8) & 0xFF);
        var b = (byte)(hash & 0xFF);
        return new SKColor(r, g, b);
    }
}