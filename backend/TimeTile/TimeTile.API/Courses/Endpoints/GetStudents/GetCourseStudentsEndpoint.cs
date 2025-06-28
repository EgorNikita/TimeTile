using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.GetStudents
{
    public class GetCourseStudentsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{Id:int}/students", Handle)
                .WithSummary("Gets Students of the Course")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relations = await db.CoursesStudents
                .AsNoTracking()
                .Where(cs => cs.CourseId == request.Id)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(cs => new Response(
                    cs.StudentId,
                    new StudentInfo(
                        cs.Student.Id,
                        cs.Student.Firstname,
                        cs.Student.Lastname,
                        cs.Student.HomeAddress,
                        cs.Student.PhoneNumber,
                        cs.Student.BirthDate,
                        cs.Student.Login,
                        cs.Student.GroupId,
                        cs.Student.Avatar.FileGuid.ToString()
                    ),
                    cs.ExamGradeId,
                    cs.HasExam,
                    cs.PositionX,
                    cs.PositionY
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
            int? ExamGradeId,
            bool HasExam,
            short PositionX,
            short PositionY
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
