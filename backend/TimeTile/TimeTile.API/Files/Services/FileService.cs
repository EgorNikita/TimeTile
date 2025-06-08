using Microsoft.EntityFrameworkCore;
using System.Threading;
using TimeTile.API.Files.Helpers;
using TimeTile.API.Files.Repositories.Interfaces;
using TimeTile.API.Files.Services.Interfaces;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Files.Services;

public class FileService : IFileService
{
    private readonly TimetileDbContext _db;
    private readonly IFileRepository _fileRepository;

    private const string STORAGE_ASSEMBLY = "TimeTile.Storage";
    private const string STORAGE_FOLDER = "Uploads";
    private readonly string _storagePath;

    public FileService(IWebHostEnvironment env, TimetileDbContext db, IFileRepository repository)
    {
        var solutionRoot = Directory.GetParent(env.ContentRootPath)!.FullName;

        _storagePath = Path.Combine(
            solutionRoot,
            STORAGE_ASSEMBLY,
            STORAGE_FOLDER
        );

        _db = db;
        _fileRepository = repository;
    }

    public async Task<string> SaveFile(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }

        // Sanitize the file name to avoid invalid path chars
        var safeFileName = FileNameSanitizer.MakeValidFileName(Path.GetFileName(fileName));

        // Creating unique new filename
        var extension = Path.GetExtension(safeFileName);
        var newFileName = $"{Guid.NewGuid()}{extension}";

        // Full path to save the file
        var filePath = Path.Combine(_storagePath, newFileName);

        // Save stream to file asynchronously
        await using var fileStreamOutput = System.IO.File.Create(filePath);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        await _fileRepository.Add(fileName, extension, fileStreamOutput.Length, filePath, cancellationToken);

        // Return the relative path
        var relativePath = Path.Combine(STORAGE_FOLDER, newFileName).Replace(@"\", @"/");

        return relativePath;
    }

    public async Task<Result<Stream>> GetFileStream(int id, CancellationToken cancellationToken)
    {
        var result = await _fileRepository.GetById(id, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<Stream>(result.Error);

        var file = result.Data;

        var fileStream = new FileStream(file!.StoragePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

        return Result.Success<Stream>(fileStream);
    }
}