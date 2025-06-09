using TimeTile.Core.Common.UnifiedResponse;
using File = TimeTile.Core.Models.File;

namespace TimeTile.Core.Common.Interfaces.Repositories;

public interface IFileRepository
{
    Task Add(string fileName, string extension, long fileLength, string filePath, CancellationToken cancellationToken);

    Task<Result<File>> GetById(int id, CancellationToken cancellationToken);
}