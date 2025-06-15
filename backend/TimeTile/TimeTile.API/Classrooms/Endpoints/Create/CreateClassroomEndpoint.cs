using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Claims;
using TimeTile.API.Authentication;
using TimeTile.API.ClassroomTypes.Services;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Classrooms.Endpoints.Create
{
    public class CreateClassroomEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new ClassroomType")
                .WithRequestValidation<Request>()
                .DisableAntiforgery();
        }

        private static async Task<Results<Created<Result<Response>>, BadRequest<Result>, JsonHttpResult<Result>>> Handle(
            Request request,
            TimetileDbContext db,
            ClaimsPrincipal claimsPrincipal,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionResult = await claimsPrincipal.GetValidatedInstitutionIdAsync(db, cancellationToken);

            if (institutionResult.IsFailure)
                return TypedResults.Json(
                    Result.Failure(institutionResult.Error),
                    statusCode: StatusCodes.Status401Unauthorized
                );

            var institutionId = institutionResult.Data;

            // Check if already exists
            var duplicateCheckResult = await IsClassroomDuplicate(request, institutionId, db, cancellationToken);

            if (duplicateCheckResult.IsFailure)
                return TypedResults.BadRequest(duplicateCheckResult);

            // Check if ClassroomTypeId is valid
            var classroomTypeExistsCheck = await ClassroomTypeExists(request.ClassroomTypeId, institutionId, db);

            if (classroomTypeExistsCheck.IsFailure)
                return TypedResults.BadRequest(classroomTypeExistsCheck);

            // Save classroom
            var classroom = new Classroom
            {
                Title = request.Title.Trim(),
                Capacity = request.Capacity,
                InstitutionId = institutionId,
                ClassroomTypeId = request.ClassroomTypeId
            };

            await db.Classrooms.AddAsync(classroom, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                classroom.Id,
                classroom.Title,
                classroom.Capacity,
                classroom.InstitutionId,
                classroom.ClassroomTypeId
            );

            var result = Result.Success(response);

            return TypedResults.Created($"/classrooms/{classroom.Id}", result);
        }

        private static async Task<Result> ClassroomTypeExists(int classroomTypeId, int institutionId, TimetileDbContext db)
        {
            var exists = await db.ClassroomTypes
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId)
                .AnyAsync(x => x.Id == classroomTypeId);

            if (exists)
                return Result.Success();

            var error = Error.From(
                $"Classroom type with ID '{classroomTypeId}' does not exist.",
                "ENTITY_DOES_NOT_EXIST"
            );

            return Result.Failure(error);
        }

        private static async Task<Result> IsClassroomDuplicate(
            Request request,
            int institutionId,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var isDuplicate = await db.Classrooms
                .AsNoTracking()
                .Where(c => c.InstitutionId == institutionId)
                .AnyAsync(c => c.DeletedAt == null && c.Title == request.Title, cancellationToken);

            if (isDuplicate)
            {
                var error = Error.From(
                    $"A classroom with the title '{request.Title}' already exists in your institution.",
                    "ENTITY_ALREADY_EXISTS"
                );
                return Result.Failure(error);
            }

            return Result.Success();
        }

        public sealed record Request(
            string Title,
            int Capacity,
            int ClassroomTypeId
        );

        private sealed record Response(
            int Id,
            string Title,
            int Capacity,
            int InstitutionId,
            int ClassroomTypeId
        );
    }
}
