using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.GetFiles
{
    public class GetSubmissionFilesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{Id:int}/files", Handle)
                .WithSummary("Gets Files of the Submission")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var fileUrls = await db.SubmissionsFiles
                .AsNoTracking()
                .Where(sf => sf.SubmissionId == request.Id)
                .Select(af => new Response(
                    af.File.FileGuid.ToString()
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(fileUrls);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

        private sealed record Response(
            string FileUrl
        );
    }
}
