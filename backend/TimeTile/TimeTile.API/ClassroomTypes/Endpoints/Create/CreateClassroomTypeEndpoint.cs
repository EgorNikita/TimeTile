using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Files.Services;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.ClassroomTypes.Endpoints.Create
{
    public class CreateClassroomTypeEndpoint : IEndpoint
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
            [FromForm] Request request,
            TimetileDbContext db,
            IClassroomTypeService classroomTypeService,
            IFileService fileService,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = httpContext.GetInstitutionId();

            // Check if already exists
            var duplicateCheckResult = await IsClassroomTypeDuplicate(request, institutionId, db, cancellationToken);

            if (duplicateCheckResult.IsFailure)
                return TypedResults.BadRequest(duplicateCheckResult);

            // Save classroom type
            var description = request.Description.Trim();

            var classroomType = new ClassroomType
            {
                Description = description,
                InstitutionId = institutionId,
            };

            await SaveClassroomType(
                classroomType, 
                request, 
                db, 
                fileService, 
                cancellationToken
            );

            // Return result
            var response = new Response(
                classroomType.Id,
                classroomType.Description,
                classroomTypeService.GetIconUrl(classroomType)
            );

            var result = Result.Success(response);

            return TypedResults.Created($"/classroom-types/{classroomType.Id}", result);
        }

        private static async Task SaveClassroomType(
            ClassroomType classroomType,
            Request request,
            TimetileDbContext db,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                if (request.Icon is not null)
                {
                    classroomType.IconId = await SaveIcon(request.Icon, fileService, cancellationToken);
                }

                await db.ClassroomTypes.AddAsync(classroomType, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                if (classroomType.IconId is not null)
                {
                    await fileService.DeleteFilePhysically(classroomType.IconId.Value, cancellationToken);
                }

                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        }

        private static async Task<Result> IsClassroomTypeDuplicate(
            Request request,
            int institutionId,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var existingStudent = await db.ClassroomTypes
                .AsNoTracking()
                .Where(s => s.InstitutionId == institutionId)
                .AnyAsync(s => s.DeletedAt == null && s.Description == request.Description, cancellationToken);

            if (existingStudent)
            {
                var error = Error.From(
                    $"A classroom type with the description '{request.Description}' already exists in your institution.",
                    "ENTITY_ALREADY_EXISTS"
                );
                return Result.Failure(error);
            }

            return Result.Success();
        }

        private static async Task<int> SaveIcon(
            IFormFile icon,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            await using var iconStream = icon.OpenReadStream();

            return await fileService.SaveFile(
                iconStream,
                icon.FileName,
                cancellationToken
            );
        }

        public record Request
        {
            public string Description { get; init; } = null!;
            public IFormFile? Icon { get; init; }
        }

        private record Response(
            int Id,
            string Description,
            string? IconUrl
        );
    }
}
