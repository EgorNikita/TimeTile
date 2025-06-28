using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
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
                .WithRequestValidation<Request>()
                .DisableAntiforgery();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromForm] Request request,
            TimetileDbContext db,
            IGroupService groupService,
            IFileService fileService,
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

            await SaveGroup(
                group,
                request,
                db,
                groupService,
                fileService,
                cancellationToken
            );

            // Return result
            var response = new Response(
                group.Id,
                group.Title,
                groupService.GetAvatarUrl(group)
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{Routes.Groups}/{group.Id}", result);
        }

        private static async Task SaveGroup(
            Group group,
            Request request,
            TimetileDbContext db,
            IGroupService groupService,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                if (request.Avatar is not null)
                {
                    group.AvatarId = await groupService.SaveAvatar(request.Avatar, cancellationToken);
                }

                await db.Groups.AddAsync(group, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                if (group.AvatarId is not null)
                {
                    await fileService.DeleteFilePhysically(group.AvatarId.Value, cancellationToken);
                }

                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        }

        public sealed record Request
        {
            public string Title { get; set; } = null!;
            public List<int>? StudentIds { get; set; }
            public List<int>? InstitutionMemberIds { get; set; }
            public IFormFile? Avatar { get; set; }
        }

        private sealed record Response(
            int Id,
            string Title,
            string? AvatarUrl
        );
    }
}
