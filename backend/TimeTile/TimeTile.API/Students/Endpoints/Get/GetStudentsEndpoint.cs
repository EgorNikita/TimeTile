using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.Get;

public class GetStudentsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapGet("/", Handle)
            .WithSummary("Returns a page of students")
            .WithRequestValidation<Request>();
    }

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        TimetileDbContext db,
        IFileService fileService,
        IInstitutionProvider institutionProvider,
        CancellationToken cancellationToken)
    {
        var institutionId = institutionProvider.GetInstitutionId();

        // Form a final paged list
        var students = await BuildFilteredQuery(request, institutionId, db)
            .Include(s => s.Avatar)
            .ApplySorting(
                request.SortBy,
                request.Descending
            ).Select(s => new Response(
                s.Id,
                s.Firstname,
                s.Lastname,
                s.HomeAddress,
                s.PhoneNumber,
                s.BirthDate,
                s.Login,
                s.GroupId,
                fileService.GetFileUrl(s.Avatar.StoragePath)
            ))
            .ToPagedListAsync(request, cancellationToken);

        var result = Result.Success(students);

        return TypedResults.Ok(result);
    }

    private static IQueryable<Student> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
    {
        var query = db.Students
            .AsNoTracking()
            .Where(s => s.InstitutionId == institutionId);

        if (request.BirthDateFrom is not null)
            query = query.Where(s => s.BirthDate >= request.BirthDateFrom.Value);

        if (request.BirthDateTo is not null)
            query = query.Where(s => s.BirthDate <= request.BirthDateTo.Value);

        if (request.GroupIds != null && request.GroupIds.Any())
            query = query.Where(s =>
                s.GroupId != null && request.GroupIds.Cast<int?>().Contains(s.GroupId));

        if (request.CourseIds != null && request.CourseIds.Any())
            query = query.Where(s =>
                db.CoursesStudents.Any(cs => cs.StudentId == s.Id && request.CourseIds.Contains(cs.CourseId)));

        if (request.LessonIds != null && request.LessonIds.Any())
            query = query.Where(s =>
                db.LessonsStudents.Any(ls => ls.StudentId == s.Id && request.LessonIds.Contains(ls.LessonId)));

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

    public sealed record Request(
        int[]? GroupIds = null,
        int[]? CourseIds = null,
        int[]? LessonIds = null,
        string? Search = null,
        DateOnly? BirthDateFrom = null,
        DateOnly? BirthDateTo = null,
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false
    ) : IPagedRequest, ISortRequest;

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