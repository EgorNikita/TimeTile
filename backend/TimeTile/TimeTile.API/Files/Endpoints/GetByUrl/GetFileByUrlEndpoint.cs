using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Enums;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Files.Endpoints.GetByUrl
{
    public class GetFileByUrlEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{guid:guid}", Handle)
                .WithSummary("Returns File by passed Url(Guid)")
                .WithRequestValidation<Request>();
        }

        private static async Task<IResult> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            // Find File
            var file = await db.Files
                .AsNoTracking()
                .FirstAsync(f => f.FileGuid == request.Guid, cancellationToken);

            var fileStream = new FileStream(file.StoragePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            var contentType = fileService.GetContentType(file.Extension);

            return TypedResults.File(fileStream, contentType, file.OriginalName, enableRangeProcessing: true);
        }

        public sealed record Request(
            Guid Guid
        );
    }
}
