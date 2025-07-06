using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
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
                .WithRequestValidation<Request>()
                .DisableAntiforgery();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromForm] Request request,
            TimetileDbContext db,
            ICourseService courseService,
            IFileService fileService,
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

            await AssignStudentsToCourse(request, course, db, cancellationToken);

            await SaveCourse(
                course,
                request,
                db,
                courseService,
                fileService,
                cancellationToken
            );

            // Return result
            var response = new Response(
                course.Id,
                course.Title,
                course.SubjectId,
                course.TeacherId,
                course.IsAdvanced,
                course.TermId,
                courseService.GetIconUrl(course)
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{Routes.Courses}/{course.Id}", result);
        }

        private static async Task AssignStudentsToCourse(Request request, Course course, TimetileDbContext db, CancellationToken cancellationToken)
        {
            List<Student> students = new();

            if (request.GroupIds is not null && request.GroupIds.Any())
                students.AddRange(await db.Students
                    .Where(s =>
                        s.GroupId != null &&
                        request.GroupIds.Contains(s.GroupId.Value)
                )
                    .ToListAsync(cancellationToken));

            if (request.StudentIds is not null && request.StudentIds.Any())
                students.AddRange(await db.Students
                    .Where(s => request.StudentIds.Contains(s.Id))
                    .ToListAsync(cancellationToken));

            if (students.Any())
                course.CoursesToStudents = students
                    .DistinctBy(s => s.Id)
                    .Select(s => new CourseToStudent { StudentId = s.Id })
                    .ToList();
        }

        private static async Task SaveCourse(
            Course course,
            Request request,
            TimetileDbContext db,
            ICourseService courseService,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                course.IconId = await courseService.SaveIcon(request.Icon, request.Title, cancellationToken);

                await db.Courses.AddAsync(course, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                if (course.Icon is not null)
                {
                    await fileService.DeleteFilePhysically(course.IconId, cancellationToken);
                }

                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        }

        public sealed record Request
        {
            public string Title { get; set; } = null!;
            public int SubjectId { get; set; }
            public int TeacherId { get; set; }
            public bool IsAdvanced { get; set; }
            public int TermId { get; set; }
            public List<int>? GroupIds { get; set; }
            public List<int>? StudentIds { get; set; }
            public IFormFile? Icon { get; set; }
        }

        private sealed record Response(
            int Id,
            string Title,
            int SubjectId,
            int TeacherId,
            bool IsAdvanced,
            int TermId,
            string IconUrl
        );
    }
}
