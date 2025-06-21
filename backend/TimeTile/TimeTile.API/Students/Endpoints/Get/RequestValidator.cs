using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.Get;

public class RequestValidator : PagedRequestValidator<GetStudentsEndpoint.Request>
{
    public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
    {
        var institutionId = institutionProvider.GetInstitutionId();

        RuleFor(x => x)
            .Must(x => !x.BirthDateFrom.HasValue || !x.BirthDateTo.HasValue || x.BirthDateFrom <= x.BirthDateTo)
            .WithMessage("BirthDateFrom must be less than or equal to BirthDateTo");

        RuleFor(x => x.SortBy)
            .MustBeValidSortField<GetStudentsEndpoint.Request, AllowedSortFields>();

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must be 100 characters or less");

        // GroupIds
        RuleFor(x => x.GroupIds)
            .MustBeValidOptionalListOfIds()
            .DependentRules(() =>
            {
                When(x => x.GroupIds != null, () => {
                    RuleFor(x => x.GroupIds!)
                        .MustBeValidInstitutionEntityIdsList<GetStudentsEndpoint.Request, Group>(db, institutionId);
                });
            });

        // CourseIds
        RuleFor(x => x.CourseIds)
            .MustBeValidOptionalListOfIds()
            .DependentRules(() =>
            {
                When(x => x.CourseIds != null, () => {
                    RuleFor(x => x.CourseIds!)
                        .MustBeValidInstitutionEntityIdsList<GetStudentsEndpoint.Request, Course>(db, institutionId);
                });
            });

        

        // LessonIds
        RuleFor(x => x.LessonIds)
            .MustBeValidOptionalListOfIds()
            .DependentRules(() =>
            {
                When(x => x.LessonIds != null, () => {
                    RuleFor(x => x.LessonIds!)
                        .MustBeValidEntityIdsList<GetStudentsEndpoint.Request, Lesson>(db);
                });
            });
    }
}