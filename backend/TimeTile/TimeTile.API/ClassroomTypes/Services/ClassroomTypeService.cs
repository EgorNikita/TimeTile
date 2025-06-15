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
                return _fileService.GetFileUrl(classroomType.Icon!.StoragePath);
            }

            return null;
        }
    }
}
