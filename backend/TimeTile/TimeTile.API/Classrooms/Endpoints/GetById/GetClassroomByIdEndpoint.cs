using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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

namespace TimeTile.API.Classrooms.Endpoints.GetById
{
    public class GetClassroomByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns Classroom by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Find Classroom
            var classroom = await db.Classrooms
                .AsNoTracking()
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            var response = new Response(
                classroom.Id,
                classroom.Title,
                classroom.Capacity,
                classroom.ClassroomTypeId
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

        private sealed record Response(
            int Id,
            string Title,
            int Capacity,
            int ClassroomTypeId
        );
    }
}
