using System.Threading;
using TimeTile.API.Files.Services;
using TimeTile.Core.Common.Interfaces.Services;

namespace TimeTile.API.Assignments.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IFileService _fileService;

        public AssignmentService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<List<int>> SaveFiles(IFormFileCollection files, CancellationToken cancellationToken)
        {
            List<int> savedFileIds = [];

            foreach (var file in files)
            {
                var savedFileId = await _fileService.SaveFile(file.OpenReadStream(), file.FileName, cancellationToken);
                savedFileIds.Add(savedFileId);
            }

            return savedFileIds;
        }
    }
}
