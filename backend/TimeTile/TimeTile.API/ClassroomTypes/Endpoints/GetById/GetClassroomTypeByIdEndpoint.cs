using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Files.Services;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.ClassroomTypes.Endpoints.GetById
{
    public class GetClassroomTypeByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns ClassroomType by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db, 
            IClassroomTypeService classroomTypeService,
            CancellationToken cancellationToken)
        {
            // Find ClassroomType
            var classroomType = await db.ClassroomTypes
                .AsNoTracking()
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            // Return response
            var response = new Response(
                classroomType.Id,
                classroomType.Description,
                classroomTypeService.GetIconUrl(classroomType)
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

        public sealed record Response(
            int Id,
            string Description,
            string? IconUrl
        );
    }
}
