using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Json;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.UpdateCourseToStudent
{
    public class UpdateCourseToStudentEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{CourseId:int}/students/{StudentId:int}", Handle)
                .WithSummary("Gets information about Course to Student association")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            IOptions<JsonOptions> jsonOptions,
            CancellationToken cancellationToken)
        {
            var courseStudent = await db.CoursesStudents
                .Include(cs => cs.Student)
                    .ThenInclude(s => s.Avatar)
                .Include(cs => cs.ExamGrade)
                .FirstAsync(cs =>
                    cs.StudentId == parameters.StudentId &&
                    cs.CourseId == parameters.CourseId, cancellationToken
                );

            await UpdateEntity(courseStudent, body, db, jsonOptions, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);

            var response = new Response(
                courseStudent.StudentId,
                new StudentInfo(
                    courseStudent.Student.Id,
                    courseStudent.Student.Firstname,
                    courseStudent.Student.Lastname,
                    courseStudent.Student.HomeAddress,
                    courseStudent.Student.PhoneNumber,
                    courseStudent.Student.BirthDate,
                    courseStudent.Student.Login,
                    courseStudent.Student.GroupId,
                    courseStudent.Student.Avatar.FileGuid.ToString()
                ),
                courseStudent.ExamGradeId,
                courseStudent.HasExam,
                courseStudent.PositionX,
                courseStudent.PositionY
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        private static async Task UpdateEntity(
            CourseToStudent courseStudent,
            RequestBody request,
            TimetileDbContext db,
            IOptions<JsonOptions> jsonOptions,
            CancellationToken cancellationToken)
        {
            // If user included Grade to request's body
            if (request.Grade.ValueKind != JsonValueKind.Undefined)
            {
                var gradeInfo = request.GradeInfo;

                if (gradeInfo is null)
                {
                    if (courseStudent.ExamGrade is not null)
                    {
                        db.Grades.Remove(courseStudent.ExamGrade);
                    }

                    courseStudent.ExamGrade = null;
                }
                else
                {
                    var grade = new Grade
                    {
                        Value = gradeInfo.Value,
                        Weight = gradeInfo.Weight,
                        Type = GradeType.Exam
                    };

                    await db.Grades.AddAsync(grade, cancellationToken);

                    courseStudent.ExamGrade = grade;
                }
            }

            if (request.HasExam is not null)
                courseStudent.HasExam = request.HasExam.Value;

            if (request.PositionX is not null)
                courseStudent.PositionX = request.PositionX.Value;

            if (request.PositionY is not null)
                courseStudent.PositionY = request.PositionY.Value;
        }

        public sealed record RequestParameters(
            int CourseId,
            int StudentId
        );

        public sealed record RequestBody
        {
            public bool? HasExam { get; set; }
            public short? PositionX { get; set; }
            public short? PositionY { get; set; }
            public JsonElement Grade { get; set; }
            public GradeInfo? GradeInfo => Grade.ValueKind != JsonValueKind.Undefined
                ? Grade.Deserialize<GradeInfo>(JsonConfiguration.DefaultOptions)
                : null;
        };

        public sealed record GradeInfo(
            short Value,
            float Weight
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
