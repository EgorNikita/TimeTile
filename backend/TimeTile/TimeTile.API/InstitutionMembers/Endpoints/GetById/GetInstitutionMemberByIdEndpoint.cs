using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.GetById
{
    public class GetInstitutionMemberByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns InstitutionMember by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Find InstitutionMember
            var institutionMember = await db.InstitutionMembers
                .AsNoTracking()
                .FirstAsync(s => s.Id == request.Id, cancellationToken);

            var response = new Response(
                institutionMember.Id,
                institutionMember.Firstname,
                institutionMember.Lastname,
                institutionMember.HomeAddress,
                institutionMember.PhoneNumber,
                institutionMember.BirthDate,
                institutionMember.Login,
                institutionMember.RoleId,
                institutionMember.WeekWorkHours,
                institutionMember.PreferredClassroomId,
                institutionMember.Avatar.FileGuid.ToString()
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
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
