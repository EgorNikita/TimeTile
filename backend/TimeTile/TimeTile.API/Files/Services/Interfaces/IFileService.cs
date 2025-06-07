namespace TimeTile.API.Files.Services.Interfaces;

public interface IFileService
{
    Task<string> SaveFile(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken);
    Task<Stream> GetFileStream(string fileName, CancellationToken cancellationToken);
}