using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Institutions.Endpoints.GetInstitutions;

public class GetInstitutionsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapGet("/", Handle)
            .WithSummary("Returns a page of Institutions")
            .WithRequestValidation<Request>();
    }

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        TimetileDbContext context,
        CancellationToken cancellationToken)
    {
        var institutions = await context.Institutions
            .AsNoTracking()
            .ApplySorting(
                request.SortBy,
                request.Descending)
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

        return TypedResults.Ok(result);
    }

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false
    ) : IPagedRequest, ISortRequest;

    public sealed record Response(
        int Id,
        string Title,
        string Address,
        string PhoneNumber,
        string Email,
        string Domain
    );
}