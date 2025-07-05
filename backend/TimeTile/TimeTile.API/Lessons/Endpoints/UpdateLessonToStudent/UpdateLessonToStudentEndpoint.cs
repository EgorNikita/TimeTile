using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Json;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.UpdateLessonToStudent
{
    public class UpdateLessonToStudentEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{LessonId:int}/students/{StudentId:int}", Handle)
                .WithSummary("Partial update of Lesson to Student association")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Results<Ok<Result<Response>>, BadRequest<Result>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var lessonStudent = await db.LessonsStudents
                .Include(ls => ls.Student)
                    .ThenInclude(s => s.Avatar)
                .Include(ls => ls.ClassworkGrade)
                .Include(ls => ls.HomeworkGrade)
                .Include(ls => ls.Lesson)
                .FirstAsync(ls =>
                    ls.StudentId == parameters.StudentId &&
                    ls.LessonId == parameters.LessonId, cancellationToken
                );

            await UpdateEntity(lessonStudent, body, db, cancellationToken);

            if (lessonStudent.CameAt >= lessonStudent.LeftAt)
            {
                var error = Error.From("CameAt should be less than LeftAt.");
                var failure = Result.Failure(error);

                return TypedResults.BadRequest(failure);
            }

            if (lessonStudent.CameAt is null && lessonStudent.LeftAt is not null)
            {
                var error = Error.From("Student cannot have LeftAt, but no CameAt.");
                var failure = Result.Failure(error);

                return TypedResults.BadRequest(failure);
            }

            await db.SaveChangesAsync(cancellationToken);

            var response = new Response(
                lessonStudent.StudentId,
                new StudentInfo(
                    lessonStudent.Student.Id,
                    lessonStudent.Student.Firstname,
                    lessonStudent.Student.Lastname,
                    lessonStudent.Student.HomeAddress,
                    lessonStudent.Student.PhoneNumber,
                    lessonStudent.Student.BirthDate,
                    lessonStudent.Student.Login,
                    lessonStudent.Student.GroupId,
                    lessonStudent.Student.Avatar.FileGuid.ToString()
                ),
                lessonStudent.CameAt,
                lessonStudent.LeftAt,
                lessonStudent.ClassworkGradeId,
                lessonStudent.HomeworkGradeId
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        private static async Task UpdateEntity(
            LessonToStudent lessonStudent,
            RequestBody request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // If user included CameAt to request's body
            if (request.CameAt.WasProvided)
            {
                lessonStudent.CameAt = request.CameAt.Value;
            }

            // If user included LeftAt to request's body
            if (request.LeftAt.WasProvided)
            {
                lessonStudent.LeftAt = request.LeftAt.Value;
            }

            // If user included ClassworkGrade to request's body
            if (request.ClassworkGrade.WasProvided)
            {
                if (lessonStudent.ClassworkGrade is not null)
                {
                    db.Grades.Remove(lessonStudent.ClassworkGrade);
                }

                var grade = request.ClassworkGrade.Value;

                if (grade is null)
                {
                    lessonStudent.ClassworkGrade = null;
                }
                else
                {
                    var gradeToAdd = new Grade
                    {
                        Value = grade.Value,
                        Weight = grade.Weight,
                        Type = GradeType.Classwork
                    };

                    await db.Grades.AddAsync(gradeToAdd, cancellationToken);

                    lessonStudent.ClassworkGrade = gradeToAdd;
                }
            }

            // If user included HomeworkGrade to request's body
            if (request.HomeworkGrade.WasProvided)
            {
                if (lessonStudent.HomeworkGrade is not null)
                {
                    db.Grades.Remove(lessonStudent.HomeworkGrade);
                }

                var grade = request.HomeworkGrade.Value;

                if (grade is null)
                {
                    lessonStudent.HomeworkGrade = null;
                }
                else
                {
                    var gradeToAdd = new Grade
                    {
                        Value = grade.Value,
                        Weight = grade.Weight,
                        Type = GradeType.Homework
                    };

                    await db.Grades.AddAsync(gradeToAdd, cancellationToken);

                    lessonStudent.HomeworkGrade = gradeToAdd;
                }
            }
        }

        public sealed record RequestParameters(
            int LessonId,
            int StudentId
        );

        public sealed record RequestBody(
            PatchOptionalProperty<DateTimeOffset?> CameAt,
            PatchOptionalProperty<DateTimeOffset?> LeftAt,
            PatchOptionalProperty<GradeInfo?> ClassworkGrade,
            PatchOptionalProperty<GradeInfo?> HomeworkGrade
        );

        public sealed record GradeInfo(
            short Value,
            float Weight
        );

        private sealed record Response(
            int StudentId,
            StudentInfo Student,
            DateTimeOffset? CameAt,
            DateTimeOffset? LeftAt,
            int? ClassworkGradeId,
            int? HomeworkGradeId
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
