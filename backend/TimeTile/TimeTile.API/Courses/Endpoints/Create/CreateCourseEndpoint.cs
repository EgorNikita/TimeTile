using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using static TimeTile.API.Endpoints;

namespace TimeTile.API.Courses.Endpoints.Create
{
    public class CreateCourseEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new Course")
                .WithRequestValidation<Request>();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromBody] Request request,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = institutionProvider.GetInstitutionId();

            // Save Course
            var course = new Course
            {
                Title = request.Title.Trim(),
                SubjectId = request.SubjectId,
                TeacherId = request.TeacherId,
                IsAdvanced = request.IsAdvanced,
                InstitutionId = institutionId,
                TermId = request.TermId
            };

            if (request.StudentIds is not null && request.StudentIds.Any())
                course.CoursesToStudents = await db.Students
                    .Where(s => request.StudentIds.Contains(s.Id))
                    .Select(s => new CourseToStudent { StudentId = s.Id })
                    .ToListAsync(cancellationToken);

            await db.Courses.AddAsync(course, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                course.Id,
                course.Title,
                course.SubjectId,
                course.TeacherId,
                course.IsAdvanced,
                course.TermId
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{Routes.Courses}/{course.Id}", result);
        }

        public sealed record Request(
            string Title,
            int SubjectId,
            int TeacherId,
            bool IsAdvanced,
            int TermId,
            List<int>? StudentIds
        );

        private sealed record Response(
            int Id,
            string Title,
            int SubjectId,
            int TeacherId,
            bool IsAdvanced,
            int TermId
        );
    }
}
