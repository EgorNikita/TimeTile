using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
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

        private static async Task<Created<Result<Response>>> Handle(
            [FromForm] Request request,
            TimetileDbContext db,
            IClassroomTypeService classroomTypeService,
            IFileService fileService,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = institutionProvider.GetInstitutionId();

            // Save classroom type
            var classroomType = new ClassroomType
            {
                Description = request.Description.Trim(),
                InstitutionId = institutionId,
            };

            await SaveClassroomType(
                classroomType,
                request,
                db,
                classroomTypeService,
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

            return TypedResults.Created($"{API.Endpoints.Routes.ClassroomTypes}/{classroomType.Id}", result);
        }

        private static async Task SaveClassroomType(
            ClassroomType classroomType,
            Request request,
            TimetileDbContext db,
            IClassroomTypeService classroomTypeService,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                if (request.Icon is not null)
                {
                    classroomType.IconId = await classroomTypeService.SaveIcon(request.Icon, cancellationToken);
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
