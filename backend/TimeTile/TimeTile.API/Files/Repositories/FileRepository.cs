using Microsoft.EntityFrameworkCore;
using System.Threading;
using TimeTile.API.Files.Repositories.Interfaces;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using File = TimeTile.Core.Models.File;

namespace TimeTile.API.Files.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly TimetileDbContext _db;

        public FileRepository(TimetileDbContext db)
        {
            _db = db;
        }

        public async Task Add(string fileName, string extension, long fileLength, string filePath, CancellationToken cancellationToken)
        {
            var file = new File()
            {
                OriginalName = fileName,
                Extension = GetFileExtension(extension),
                Size = fileLength,
                StoragePath = filePath
            };

            await _db.Files.AddAsync(file, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        private static FileExtension GetFileExtension(string extension)
        {
            // Normalize: remove leading dot and convert to lower case
            var normalized = extension.Trim().TrimStart('.').ToLowerInvariant();

            return normalized switch
            {
                "pdf" => FileExtension.Pdf,
                "docx" => FileExtension.Docx,
                "xlsx" => FileExtension.Xlsx,
                "png" => FileExtension.Png,
                "jpg" => FileExtension.Jpg,
                "jpeg" => FileExtension.Jpeg,
                "txt" => FileExtension.Txt,
                "zip" => FileExtension.Zip,
                _ => throw new NotSupportedException($"Unsupported file extension: {extension}")
            };
        }

        public async Task<Result<File>> GetById(int id, CancellationToken cancellationToken)
        {
            var file = await _db.Files.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

            if (file is null)
            {
                var error = Error.From($"File with id '{id}' does not exist.");
                return Result.Failure<File>(error);
            }

            return Result.Success<File>(file);
        }
    }
}
