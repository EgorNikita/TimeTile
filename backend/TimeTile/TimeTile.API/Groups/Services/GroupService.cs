using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;

namespace TimeTile.API.Groups.Services
{
    public class GroupService : IGroupService
    {
        private readonly IFileService _fileService;

        public GroupService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public string? GetAvatarUrl(Group group)
        {
            if (group.AvatarId.HasValue)
            {
                return group.Avatar!.FileGuid.ToString();
            }

            return null;
        }

        public async Task<int> SaveAvatar(IFormFile avatar, CancellationToken cancellationToken)
        {
            await using var avatarStream = avatar.OpenReadStream();

            return await _fileService.SaveFile(
                avatarStream,
                avatar.FileName,
                cancellationToken
            );
        }
    }
}
