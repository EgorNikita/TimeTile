using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.Get
{
    public class GetInstitutionMembersEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of institution members")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            // Form a final paged list
            var institutionMembers = await BuildFilteredQuery(request, institutionId, db)
                .Include(m => m.Avatar)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                ).Select(m => new Response(
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
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(institutionMembers);

            return TypedResults.Ok(result);
        }

        private static IQueryable<InstitutionMember> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
        {
            var baseQuery = db.InstitutionMembers
                .AsNoTracking()
                .Include(m => m.TeacherToSubjects)
                .Include(m => m.InstitutionMemberToGroups)
                .Where(m => m.InstitutionId == institutionId);

            if (request.SubjectIds is not null && request.SubjectIds.Any())
                baseQuery = baseQuery.Where(m =>
                    m.TeacherToSubjects.Any(ts => request.SubjectIds.Contains(ts.SubjectId))
                );

            if (request.PreferredClassroomIds is not null && request.PreferredClassroomIds.Any())
                baseQuery = baseQuery.Where(m =>
                    m.PreferredClassroomId != null && 
                    request.PreferredClassroomIds.Contains(m.PreferredClassroomId.Value)
                );

            if (request.RoleIds is not null && request.RoleIds.Any())
                baseQuery = baseQuery.Where(m =>
                    request.RoleIds.Contains(m.RoleId)
                );

            if (request.GroupIds is not null && request.GroupIds.Any())
                baseQuery = baseQuery.Where(m =>
                    m.InstitutionMemberToGroups.Any(mg => request.GroupIds.Contains(mg.GroupId))
                );

            return baseQuery;
        }

        public sealed record Request(
            int[]? SubjectIds = null,
            int[]? PreferredClassroomIds = null,
            int[]? RoleIds = null,
            int[]? GroupIds = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

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
