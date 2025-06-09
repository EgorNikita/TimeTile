using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.Core.Common.Interfaces.Services;

public interface IFileService
{
    Task<string> GetFileUrl(string filePath, CancellationToken cancellationToken);
    Task<string> SaveFile(Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<Result<Stream>> GetFileStream(int id, CancellationToken cancellationToken);
}