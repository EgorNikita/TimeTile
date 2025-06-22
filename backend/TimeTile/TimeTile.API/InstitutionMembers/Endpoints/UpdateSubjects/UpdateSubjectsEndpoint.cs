using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.UpdateSubjects
{
    public class UpdateSubjectsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{TeacherId:int}/subjects", Handle)
                .WithSummary("Updates Subjects by Teacher(InstitutionMember)")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            if (body.SubjectsToAdd is not null)
            {
                await AddSubjects(parameters.TeacherId, body.SubjectsToAdd, db, cancellationToken);
            }
            if (body.SubjectsToRemove is not null)
            {
                await RemoveSubjects(parameters.TeacherId, body.SubjectsToRemove, db, cancellationToken);
            }

            var subjects = await db.Subjects
                .AsNoTracking()
                .Include(p => p.TeachersToSubject)
                .Where(p => p.TeachersToSubject.Any(x => x.TeacherId == parameters.TeacherId))
                .Select(p => new Response(
                    p.Id,
                    p.Title
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(subjects);

            return TypedResults.Ok(result);
        }

        private static async Task AddSubjects(
            int teacherId,
            List<int> subjectsIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var existingSubjectsIds = await db.TeachersSubjects
                .Where(x => x.TeacherId == teacherId)
                .Select(x => x.SubjectId)
                .ToListAsync(cancellationToken);

            var relationsToAdd = subjectsIds
                .Where(subjectId => !existingSubjectsIds.Contains(subjectId))
                .Select(subjectId => new Core.Models.TeacherToSubject
                {
                    TeacherId = teacherId,
                    SubjectId = subjectId
                })
                .ToList();

            if (relationsToAdd.Any())
            {
                await db.TeachersSubjects.AddRangeAsync(relationsToAdd, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task RemoveSubjects(
            int teacherId,
            List<int> subjectsIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relationsToRemove = await db.TeachersSubjects
                .Where(x => x.TeacherId == teacherId && subjectsIds.Contains(x.SubjectId))
                .ToListAsync(cancellationToken);

            if (relationsToRemove.Any())
            {
                db.TeachersSubjects.RemoveRange(relationsToRemove);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public sealed record RequestParameters(
            int TeacherId
        );

        public sealed record RequestBody(
            [property: JsonPropertyName("add")] List<int>? SubjectsToAdd,
            [property: JsonPropertyName("remove")] List<int>? SubjectsToRemove
        );

        private sealed record Response(
            int Id,
            string Title
        );
    }
}
