using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.GetStudents;

public class GetStudentsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapGet("/", Handle)
            .WithSummary("Retrieves a list of all students")
            .WithRequestValidation<Request>();
    }

    private static async Task<Results<Ok<Result<PagedList<Response>>>, NotFound<Result>>> Handle(
        [AsParameters] Request request,
        TimetileDbContext db,
        IFileService fileService,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var institutionResult = await claimsPrincipal.GetValidatedInstitutionIdAsync(db, cancellationToken);
        if (!institutionResult.IsSuccess)
            return TypedResults.NotFound(Result.Failure(institutionResult.Error));

        var institutionId = institutionResult.Data;

        var query = BuildFilteredQuery(request, institutionId, db);

        query = ApplySorting(query, request);

        var students = await query.ToPagedListAsync(request, cancellationToken);

        var responses = await MapToResponses(students.Items, fileService, cancellationToken);

        var pagedResponse = new PagedList<Response>(
            responses,
            students.Page,
            students.PageSize,
            students.TotalPages,
            students.TotalCount
        );

        var result = Result.Success(pagedResponse);

        return TypedResults.Ok(result);
    }

    private static IQueryable<Student> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
    {
        var query = db.Students
            .AsNoTracking()
            .Where(s => s.InstitutionId == institutionId);

        if (request.GroupIds != null && request.GroupIds.Any())
            query = query.Where(s =>
                s.GroupId != null && request.GroupIds.Cast<int?>().Contains(s.GroupId));

        if (request.CourseIds != null && request.CourseIds.Any())
            query = query.Where(s =>
                db.CoursesStudents.Any(cs => cs.StudentId == s.Id && request.CourseIds.Contains(cs.CourseId)));

        if (request.LessonIds != null && request.LessonIds.Any())
            query = query.Where(s =>
                db.LessonsStudents.Any(ls => ls.StudentId == s.Id && request.LessonIds.Contains(ls.LessonId)));

        if (request.BirthDateFrom is not null)
            query = query.Where(s => s.BirthDate >= request.BirthDateFrom.Value);

        if (request.BirthDateTo is not null)
            query = query.Where(s => s.BirthDate <= request.BirthDateTo.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(s =>
                s.Firstname.ToLower().Contains(search) ||
                s.Lastname.ToLower().Contains(search) ||
                s.Login.ToLower().Contains(search));
        }

        return query;
    }

    private static IQueryable<Student> ApplySorting(IQueryable<Student> query, Request request)
    {
        if (string.IsNullOrEmpty(request.SortBy) ||
            !Enum.TryParse<AllowedSortFields>(request.SortBy, true, out var sortField))
            // Default sorting
            return query.OrderBy(s => s.Firstname);

        return sortField switch
        {
            AllowedSortFields.Firstname => request.Descending
                ? query.OrderByDescending(s => s.Firstname)
                : query.OrderBy(s => s.Firstname),

            AllowedSortFields.Lastname => request.Descending
                ? query.OrderByDescending(s => s.Lastname)
                : query.OrderBy(s => s.Lastname),

            AllowedSortFields.Birthdate => request.Descending
                ? query.OrderByDescending(s => s.BirthDate)
                : query.OrderBy(s => s.BirthDate),

            AllowedSortFields.Login => request.Descending
                ? query.OrderByDescending(s => s.Login)
                : query.OrderBy(s => s.Login),

            _ => query.OrderBy(s => s.Firstname)
        };
    }

    private static async Task<List<Response>> MapToResponses(
        IEnumerable<Student> students,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var responses = await Task.WhenAll(students.Select(async s =>
        {
            var avatarUrl = await fileService.GetFileUrl(s.Avatar.StoragePath, cancellationToken);
            return new Response(
                s.Id,
                s.Firstname,
                s.Lastname,
                s.Login,
                avatarUrl
            );
        }));

        return responses.ToList();
    }

    public sealed record Request(
        int[]? GroupIds = null,
        int[]? CourseIds = null,
        int[]? LessonIds = null,
        [FromQuery] string? Search = null,
        [FromQuery] DateOnly? BirthDateFrom = null,
        [FromQuery] DateOnly? BirthDateTo = null,
        [FromQuery] int? Page = 1,
        [FromQuery] int? PageSize = 10,
        [FromQuery] string? SortBy = null,
        [FromQuery] bool Descending = false
    ) : IPagedRequest;

    public sealed record Response(
        int Id,
        string Firstname,
        string Lastname,
        string Login,
        string AvatarUrl
    );
}