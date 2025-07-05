using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;
using static TimeTile.Core.Common.Constants.Permissions;

namespace TimeTile.API.Groups.Endpoints.UpdateStudents
{
    public class UpdateGroupStudentsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}/students", Handle)
                .WithSummary("Updates Students by Group")
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

            var students = await db.Students
                .AsNoTracking()
                .Include(s => s.Avatar)
                .Where(s => s.GroupId == parameters.Id)
                .Select(s => new Response(
                    s.Id,
                    s.Firstname,
                    s.Lastname,
                    s.HomeAddress,
                    s.PhoneNumber,
                    s.BirthDate,
                    s.Login,
                    s.GroupId,
                    s.Avatar.FileGuid.ToString()
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(students);

            return TypedResults.Ok(result);
        }

        private static async Task AddStudents(
            int id,
            List<int> studentIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var students = await db.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            students.ForEach(s => s.GroupId = id);

            await db.SaveChangesAsync(cancellationToken);
        }

        private static async Task RemoveStudents(
            int id,
            List<int> studentIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var students = await db.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            students.ForEach(s => s.GroupId = null);

            await db.SaveChangesAsync(cancellationToken);
        }

        public sealed record RequestParameters(
            int Id
        );

        public sealed record RequestBody(
            [property: JsonPropertyName("add")] List<int>? StudentsToAdd,
            [property: JsonPropertyName("remove")] List<int>? StudentsToRemove
        );

        private sealed record Response(
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
