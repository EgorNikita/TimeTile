using TimeTile.API.Files.Services.Interfaces;

namespace TimeTile.API.Files.Services;

public class FileService : IFileService
{
    private static readonly string UploadsFolder = Path.Combine(Environment.CurrentDirectory, "uploads");

    public static string FileServiceBaseUrl { get; } = UploadsFolder;
    
    public Task<string> GetFileUrl(string filePath, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(filePath);
        var fileUrl = $"{FileDownloadBaseUrl}/{Uri.EscapeDataString(fileName)}";

        return Task.FromResult(fileUrl);
    }
    
    public async Task<string> SaveFile(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken)
    {
        
        if (!Directory.Exists(UploadsFolder))
        {
            Directory.CreateDirectory(UploadsFolder);
        }
        
        // Sanitize the file name to avoid invalid path chars
        var safeFileName = Path.GetFileName(fileName);

        // Full path to save the file
        var filePath = Path.Combine(UploadsFolder, safeFileName);

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