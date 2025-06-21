using TimeTile.API.Files.Helpers;
using TimeTile.Core.Common.Interfaces.Repositories;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Files.Services;

public class FileService : IFileService
{
    private const string STORAGE_ASSEMBLY = "TimeTile.Storage";
    private const string STORAGE_FOLDER = "Uploads";
    private readonly IFileRepository _fileRepository;
    private readonly string _storagePath;

    public FileService(IWebHostEnvironment env, IFileRepository repository)
    {
        var solutionRoot = Directory.GetParent(env.ContentRootPath)!.FullName;

        _storagePath = Path.Combine(
            solutionRoot,
            STORAGE_ASSEMBLY,
            STORAGE_FOLDER
        );

        _fileRepository = repository;
    }

    public async Task<int> SaveFile(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);

        // Sanitize the file name to avoid invalid path chars
        var safeFileName = FileNameSanitizer.MakeValidFileName(Path.GetFileName(fileName));

        // Creating unique new filename
        var extension = Path.GetExtension(safeFileName);
        var fileGuid = Guid.NewGuid();

        var newFileName = $"{fileGuid}{extension}";

        // Full path to save the file
        var filePath = Path.Combine(_storagePath, newFileName);

        // Save stream to file asynchronously
        await using var fileStreamOutput = File.Create(filePath);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        var file = await _fileRepository.Add(
            fileName, 
            extension, 
            fileStreamOutput.Length, 
            filePath, 
            fileGuid, 
            cancellationToken
        );

        return file.Id;
    }

    public async Task DeleteFilePhysically(int id, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.GetById(id, cancellationToken);

        if (file.IsSuccess && File.Exists(file.Data!.StoragePath))
        {
            File.Delete(file.Data!.StoragePath);
        }
    }

    public async Task<Result<Stream>> GetFileStream(int id, CancellationToken cancellationToken)
    {
        var result = await _fileRepository.GetById(id, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<Stream>(result.Error);

        var file = result.Data;

        var fileStream = new FileStream(file!.StoragePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

        return Result.Success<Stream>(fileStream);
    }

    public string GetContentType(FileExtension extension)
    {
        return extension switch
        {
            FileExtension.Pdf => "application/pdf",
            FileExtension.Docx => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            FileExtension.Xlsx => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            FileExtension.Png => "image/png",
            FileExtension.Jpg or FileExtension.Jpeg => "image/jpeg",
            FileExtension.Txt => "text/plain",
            FileExtension.Zip => "application/zip",
            _ => "application/octet-stream"
        };
    }
}