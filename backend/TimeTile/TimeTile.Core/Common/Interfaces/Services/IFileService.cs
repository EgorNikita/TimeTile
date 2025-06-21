using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;

namespace TimeTile.Core.Common.Interfaces.Services;

public interface IFileService
{
    Task<int> SaveFile(Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task DeleteFilePhysically(int id, CancellationToken cancellationToken);
    Task<Result<Stream>> GetFileStream(int id, CancellationToken cancellationToken);
    string GetContentType(FileExtension extension);
}