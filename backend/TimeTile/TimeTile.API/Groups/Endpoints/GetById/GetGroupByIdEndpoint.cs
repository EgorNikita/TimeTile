using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Groups.Endpoints.GetById
{
    public class GetGroupByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns Group by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            IGroupService groupService,
            CancellationToken cancellationToken)
        {
            // Find Group
            var group = await db.Groups
                .AsNoTracking()
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            var response = new Response(
                group.Id,
                group.Title,
                groupService.GetAvatarUrl(group)
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

        private sealed record Response(
            int Id,
            string Title,
            string? AvatarUrl
        );
    }
}
