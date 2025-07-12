using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.GetStudents
{
    public class GetLessonStudentsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{Id:int}/students", Handle)
                .WithSummary("Gets Students of the Lesson")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relations = await db.LessonsStudents
                .AsNoTracking()
                .Where(ls => ls.LessonId == request.Id)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(ls => new Response(
                    ls.StudentId,
                    new StudentInfo(
                        ls.Student.Id,
                        ls.Student.Firstname,
                        ls.Student.Lastname,
                        ls.Student.HomeAddress,
                        ls.Student.PhoneNumber,
                        ls.Student.BirthDate,
                        ls.Student.Login,
                        ls.Student.GroupId,
                        ls.Student.Avatar.FileGuid.ToString()
                    ),
                    ls.CameAt,
                    ls.LeftAt,
                    ls.GradeId
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(relations);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

        private sealed record Response(
            int StudentId,
            StudentInfo Student,
            DateTimeOffset? CameAt,
            DateTimeOffset? LeftAt,
            int? GradeId
        );

        private sealed record StudentInfo(
            int Id,
            string Firstname,
            string Lastname,
            string HomeAddress,
            string PhoneNumber,
            DateOnly BirthDate,
            string Login,
            int? GroupId,
            string AvatarUrl
        );
    }
}
