using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Users.Endpoints.GetBulk
{
    public class GetUsersBulkEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/bulk", Handle)
                .WithSummary("Returns users by passed ids")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var users = await db.Users
                .AsNoTracking()
                .Include(u => u.Avatar)
                .Where(u => request.Ids.Contains(u.Id))
                .Select(u => new Response(
                    u.Id,
                    u.Firstname,
                    u.Lastname,
                    u.HomeAddress,
                    u.PhoneNumber,
                    u.BirthDate,
                    u.Login,
                    u.RoleId,
                    u.Avatar.FileGuid.ToString()
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(users);

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
            string AvatarUrl
        );
    }
}
