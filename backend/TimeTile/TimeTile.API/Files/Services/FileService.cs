using TimeTile.API.Files.Services.Interfaces;

namespace TimeTile.API.Files.Services;

public class FileService : IFileService
{
    public async Task<string> SaveFile(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken)
    {
        var uploadsFolder = Path.Combine(Environment.CurrentDirectory, "uploads");
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }
        
        // Sanitize the file name to avoid invalid path chars
        var safeFileName = Path.GetFileName(fileName);

        // Full path to save the file
        var filePath = Path.Combine(uploadsFolder, safeFileName);

        // Save stream to file asynchronously
        await using var fileStreamOutput = File.Create(filePath);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        // Return the path where file was saved
        return filePath;
    }

    public Task<Stream> GetFileStream(string fileName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}