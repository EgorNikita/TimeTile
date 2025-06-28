using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.UpdateStudents
{
    public class UpdateCourseStudentsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}/students", Handle)
                .WithSummary("Updates Students by Course")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            if (body.StudentsToAdd is not null)
            {
                await AddStudents(parameters.Id, body.StudentsToAdd, db, cancellationToken);
            }
            if (body.StudentsToRemove is not null)
            {
                await RemoveStudents(parameters.Id, body.StudentsToRemove, db, cancellationToken);
            }

            var relations = await db.CoursesStudents
                .AsNoTracking()
                .Where(cs => cs.CourseId == parameters.Id)
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
                .ToListAsync(cancellationToken);

            var result = Result.Success(relations);

            return TypedResults.Ok(result);
        }

        private static async Task AddStudents(
            int id,
            List<int> studentIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var existingStudentIds = await db.CoursesStudents
                .Where(x => x.CourseId == id)
                .Select(x => x.StudentId)
                .ToListAsync(cancellationToken);

            var relationsToAdd = studentIds
                .Where(studentId => !existingStudentIds.Contains(studentId))
                .Select(studentId => new CourseToStudent
                {
                    CourseId = id,
                    StudentId = studentId
                })
                .ToList();

            if (relationsToAdd.Any())
            {
                await db.CoursesStudents.AddRangeAsync(relationsToAdd, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task RemoveStudents(
            int id,
            List<int> studentIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relationsToRemove = await db.CoursesStudents
                .Where(x => x.CourseId == id && studentIds.Contains(x.StudentId))
                .ToListAsync(cancellationToken);

            if (relationsToRemove.Any())
            {
                db.CoursesStudents.RemoveRange(relationsToRemove);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public sealed record RequestParameters(
            int Id
        );

        public sealed record RequestBody(
            [property: JsonPropertyName("add")] List<int>? StudentsToAdd,
            [property: JsonPropertyName("remove")] List<int>? StudentsToRemove
        );

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
