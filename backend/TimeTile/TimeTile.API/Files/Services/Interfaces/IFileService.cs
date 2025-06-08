using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Files.Services.Interfaces;

public interface IFileService
{
    Task<string> SaveFile(Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<Result<Stream>> GetFileStream(int id, CancellationToken cancellationToken);
}