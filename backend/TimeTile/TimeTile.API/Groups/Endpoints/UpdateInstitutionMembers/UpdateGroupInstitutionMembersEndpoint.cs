using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Groups.Endpoints.UpdateInstitutionMembers
{
    public class UpdateGroupInstitutionMembersEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}/institution-members", Handle)
                .WithSummary("Updates InstitutionMembers by Group")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            if (body.InstitutionMembersToAdd is not null)
            {
                await AddInstitutionMembers(parameters.Id, body.InstitutionMembersToAdd, db, cancellationToken);
            }
            if (body.InstitutionMembersToRemove is not null)
            {
                await RemoveInstitutionMembers(parameters.Id, body.InstitutionMembersToRemove, db, cancellationToken);
            }

            var groups = await db.InstitutionMembers
                .AsNoTracking()
                .Include(m => m.Avatar)
                .Where(m => m.InstitutionMemberToGroups.Any(x => x.GroupId == parameters.Id))
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

            var result = Result.Success(groups);

            return TypedResults.Ok(result);
        }

        private static async Task AddInstitutionMembers(
            int id,
            List<int> institutionMemberIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var existingInstitutionMemberIds = await db.InstitutionMembersGroups
                .Where(x => x.GroupId == id)
                .Select(x => x.InstitutionMemberId)
                .ToListAsync(cancellationToken);

            var relationsToAdd = institutionMemberIds
                .Where(memberId => !existingInstitutionMemberIds.Contains(memberId))
                .Select(memberId => new InstitutionMemberToGroup
                {
                    InstitutionMemberId = memberId,
                    GroupId = id
                })
                .ToList();

            if (relationsToAdd.Any())
            {
                await db.InstitutionMembersGroups.AddRangeAsync(relationsToAdd, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task RemoveInstitutionMembers(
            int id,
            List<int> institutionMemberIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relationsToRemove = await db.InstitutionMembersGroups
                .Where(x => x.GroupId == id && institutionMemberIds.Contains(x.InstitutionMemberId))
                .ToListAsync(cancellationToken);

            if (relationsToRemove.Any())
            {
                db.InstitutionMembersGroups.RemoveRange(relationsToRemove);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public sealed record RequestParameters(
            int Id
        );

        public sealed record RequestBody(
            [property: JsonPropertyName("add")] List<int>? InstitutionMembersToAdd,
            [property: JsonPropertyName("remove")] List<int>? InstitutionMembersToRemove
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
