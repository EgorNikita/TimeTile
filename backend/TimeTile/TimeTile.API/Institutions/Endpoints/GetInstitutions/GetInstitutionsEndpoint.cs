using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;
using TimeTile.Storage.Seeders.Fakers;

namespace TimeTile.API.Institutions.Endpoints.GetInstitutions
{
    public class GetInstitutionsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
            .MapGet("/", Handle)
            .WithSummary("Returns a page of Institutions")
            .WithRequestValidation<Request>()
            .Produces<Result<PagedList<Response>>>(StatusCodes.Status200OK);

        public sealed record Request(
            int? Page = 1,
            int? PageSize = 10
        ) : IPagedRequest;

        public sealed record Response(
            int Id,
            string Title,
            string Address,
            string PhoneNumber,
            string Email,
            string Domain
        );

        private static async Task<IResult> Handle(
            [AsParameters] Request request,
            TimetileDbContext context,
            CancellationToken cancellationToken)
        {
            var institutions = await context.Institutions
                .AsNoTracking()
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.Address,
                    x.PhoneNumber,
                    x.Email,
                    x.Domain
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(institutions);

            return Results.Ok(result);
        }
    }
}
