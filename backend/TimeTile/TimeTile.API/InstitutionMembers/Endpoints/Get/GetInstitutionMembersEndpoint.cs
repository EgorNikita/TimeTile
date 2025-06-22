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
            var institutionMembers = await db.InstitutionMembers
                .AsNoTracking()
                .Where(m => m.InstitutionId == institutionId)
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

        public sealed record Request(
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
