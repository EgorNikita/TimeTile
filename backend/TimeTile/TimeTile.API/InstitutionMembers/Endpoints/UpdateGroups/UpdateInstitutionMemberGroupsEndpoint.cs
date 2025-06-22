using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.UpdateGroups
{
    public class UpdateInstitutionMemberGroupsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{InstitutionMemberId:int}/groups", Handle)
                .WithSummary("Updates Groups by InstitutionMember")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            if (body.GroupsToAdd is not null)
            {
                await AddGroups(parameters.InstitutionMemberId, body.GroupsToAdd, db, cancellationToken);
            }
            if (body.GroupsToRemove is not null)
            {
                await RemoveGroups(parameters.InstitutionMemberId, body.GroupsToRemove, db, cancellationToken);
            }

            var groups = await db.Groups
                .AsNoTracking()
                .Include(g => g.InstitutionMembersToGroup)
                .Where(g => g.InstitutionMembersToGroup.Any(x => x.InstitutionMemberId == parameters.InstitutionMemberId))
                .Select(g => new Response(
                    g.Id,
                    g.Title
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(groups);

            return TypedResults.Ok(result);
        }

        private static async Task AddGroups(
            int institutionMemberId,
            List<int> groupsIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var existingGroupsIds = await db.InstitutionMembersGroups
                .Where(x => x.InstitutionMemberId == institutionMemberId)
                .Select(x => x.GroupId)
                .ToListAsync(cancellationToken);

            var relationsToAdd = groupsIds
                .Where(groupId => !existingGroupsIds.Contains(groupId))
                .Select(groupId => new InstitutionMemberToGroup
                {
                    InstitutionMemberId = institutionMemberId,
                    GroupId = groupId
                })
                .ToList();

            if (relationsToAdd.Any())
            {
                await db.InstitutionMembersGroups.AddRangeAsync(relationsToAdd, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task RemoveGroups(
            int institutionMemberId,
            List<int> groupsIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relationsToRemove = await db.InstitutionMembersGroups
                .Where(x => x.InstitutionMemberId == institutionMemberId && groupsIds.Contains(x.GroupId))
                .ToListAsync(cancellationToken);

            if (relationsToRemove.Any())
            {
                db.InstitutionMembersGroups.RemoveRange(relationsToRemove);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public sealed record RequestParameters(
            int InstitutionMemberId
        );

        public sealed record RequestBody(
            [property: JsonPropertyName("add")] List<int>? GroupsToAdd,
            [property: JsonPropertyName("remove")] List<int>? GroupsToRemove
        );

        private sealed record Response(
            int Id,
            string Title
        );
    }
}
