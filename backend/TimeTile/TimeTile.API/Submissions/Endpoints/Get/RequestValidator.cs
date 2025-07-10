using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetSubmissionsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetSubmissionsEndpoint.Request, AllowedSortFields>();

            // StudentIds
            RuleFor(x => x.StudentIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentIds != null, () => {
                        RuleFor(x => x.StudentIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetSubmissionsEndpoint.Request, Student>(db, institutionId);
                    });
                });

            // AssignmentIds
            RuleFor(x => x.AssignmentIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.AssignmentIds != null, () => {
                        RuleFor(x => x.AssignmentIds!)
                            .MustBeValidEntityIdsList<GetSubmissionsEndpoint.Request, Assignment>(db)
                            .DependentRules(() =>
                            {
                                RuleFor(x => x.AssignmentIds!)
                                    .MustAsync(async (ids, cancellationToken) =>
                                    {
                                        return !await db.Assignments
                                            .AsNoTracking()
                                            .AnyAsync(a => a.Lesson.Course.InstitutionId != institutionId, cancellationToken);
                                    })
                                    .WithMessage("Some AssignmentIds are invalid");
                            });
                    });
                });

            // Statuses
            RuleFor(x => x.Statuses)
                .Must(statuses => statuses == null || statuses.All(t =>
                    Enum.TryParse<SubmissionStatus>(t, ignoreCase: true, out _)))
                .WithMessage("One or more SubmissionStatus values are invalid.");
        }
    }
}
