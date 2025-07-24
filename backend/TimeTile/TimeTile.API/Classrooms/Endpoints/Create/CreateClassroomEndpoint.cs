using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Claims;
using TimeTile.API.Authentication;
using TimeTile.API.ClassroomTypes.Services;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
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
                .WithRequestValidation<Request>();
        }

        private static async Task<Created<Result<Response>>> Handle(
            Request request,
            IClassroomTypeService classroomTypeService,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = institutionProvider.GetInstitutionId();

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

            var classroomType = await db.ClassroomTypes
                .AsNoTracking()
                .Include(t => t.Icon)
                .FirstAsync(x => x.Id == classroom.ClassroomTypeId, cancellationToken);

            // Return result
            var response = new Response(
                classroom.Id,
                classroom.Title,
                classroom.Capacity,
                new ClassroomTypeInfo(
                    classroomType.Id,
                    classroomType.Description,
                    classroomTypeService.GetIconUrl(classroomType)
                )
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{API.Endpoints.Routes.Classrooms}/{classroom.Id}", result);
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
            ClassroomTypeInfo ClassroomType
        );

        private sealed record ClassroomTypeInfo(
            int Id,
            string Description,
            string? IconUrl
        );
    }
}
