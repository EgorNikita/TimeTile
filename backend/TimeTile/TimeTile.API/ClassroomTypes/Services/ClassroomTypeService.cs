using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;

namespace TimeTile.API.ClassroomTypes.Services
{
    public class ClassroomTypeService : IClassroomTypeService
    {
        private readonly IFileService _fileService;

        public ClassroomTypeService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public string? GetIconUrl(ClassroomType classroomType)
        {
            if (classroomType.IconId.HasValue)
            {
                return classroomType.Icon!.FileGuid.ToString();
            }

            return null;
        }

        public async Task<int> SaveIcon(IFormFile icon, CancellationToken cancellationToken)
        {
            await using var iconStream = icon.OpenReadStream();

            return await _fileService.SaveFile(
                iconStream,
                icon.FileName,
                cancellationToken
            );
        }
    }
}
