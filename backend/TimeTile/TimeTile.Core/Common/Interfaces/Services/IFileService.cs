using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.Core.Common.Interfaces.Services;

public interface IFileService
{
    string GetFileUrl(string filePath);
    Task<int> SaveFile(Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<Result<Stream>> GetFileStream(int id, CancellationToken cancellationToken);
}