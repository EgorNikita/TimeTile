using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.GetBulk
{
    public class GetInstitutionMembersBulkEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/bulk", Handle)
                .WithSummary("Returns institution members by passed ids")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var institutionMembers = await db.InstitutionMembers
                .AsNoTracking()
                .Where(m => request.Ids.Contains(m.Id))
                .Select(m => new Response(
                    m.Id,
                    m.Firstname,
                    m.Lastname,
                    m.HomeAddress,
                    m.PhoneNumber,
                    m.BirthDate,
                    m.Login,
                    m.RoleId,
                    m.WeekWorkHours,
                    m.PreferredClassroomId,
                    m.Avatar.FileGuid.ToString()
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(institutionMembers);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int[] Ids
        );

        private sealed record Response(
            int Id,
            string Firstname,
            string Lastname,
            string HomeAddress,
            string PhoneNumber,
            DateOnly BirthDate,
            string Login,
            int RoleId,
            int WeekWorkHours,
            int? PreferredClassroomId,
            string AvatarUrl
        );
    }
}
