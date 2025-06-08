using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Files.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task Add(string fileName, string extension, long fileLength, string filePath, CancellationToken cancellationToken);

        Task<Result<Core.Models.File>> GetById(int id, CancellationToken cancellationToken);
    }
}