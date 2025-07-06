using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;

namespace TimeTile.API.Courses.Services
{
    public class CourseService : ICourseService
    {
        private readonly IFileService _fileService;
        private readonly IAvatarService _avatarService;

        public CourseService(IFileService fileService, IAvatarService avatarService)
        {
            _fileService = fileService;
            _avatarService = avatarService;
        }

        public string GetIconUrl(Course course)
        {
            return course.Icon.FileGuid.ToString();
        }

        public async Task<int> SaveIcon(
            IFormFile? icon,
            string title,
            CancellationToken cancellationToken)
        {
            await using var iconStream = icon != null
                ? icon.OpenReadStream()
                : await _avatarService.GenerateDefaultIcon(title);

            return await _fileService.SaveFile(
                iconStream,
                icon != null ? icon.FileName : $"{title}_icon.png",
                cancellationToken
            );
        }

        public Task<Stream> GenerateDefaultIcon(string title)
        {
            return _avatarService.GenerateDefaultIcon(title);
        }
    }
}
