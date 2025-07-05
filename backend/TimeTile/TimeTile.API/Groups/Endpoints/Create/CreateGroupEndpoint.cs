using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using static TimeTile.API.Endpoints;

namespace TimeTile.API.Groups.Endpoints.Create
{
    public class CreateGroupEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new Group")
                .WithRequestValidation<Request>();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromBody] Request request,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = institutionProvider.GetInstitutionId();

            // Save Group
            var group = new Group
            {
                InstitutionId = institutionId,
                Title = request.Title.Trim()
            };

            if (request.StudentIds is not null && request.StudentIds.Any())
                group.Students = await db.Students
                    .Where(s => request.StudentIds.Contains(s.Id))
                    .ToListAsync(cancellationToken);

            if (request.InstitutionMemberIds is not null && request.InstitutionMemberIds.Any())
                group.InstitutionMembersToGroup = request.InstitutionMemberIds
                    .Select(memberId => new InstitutionMemberToGroup { InstitutionMemberId = memberId })
                    .ToList();

            await db.Groups.AddAsync(group, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                group.Id,
                group.Title
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{Routes.Groups}/{group.Id}", result);
        }

        public sealed record Request(
            string Title,
            List<int>? StudentIds,
            List<int>? InstitutionMemberIds
        );

        private sealed record Response(
            int Id,
            string Title
        );
    }
}
