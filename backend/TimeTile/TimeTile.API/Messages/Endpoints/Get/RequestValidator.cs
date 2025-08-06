using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetMessagesEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            // Sorting
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetMessagesEndpoint.Request, AllowedSortFields>();

            // Date
            RuleFor(x => x)
                .Must(x => !x.From.HasValue || !x.Until.HasValue || x.From <= x.Until)
                .WithMessage("From must be less than or equal to Until");

            // UserIds
            RuleFor(x => x.UserIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.UserIds != null, () => {
                        RuleFor(x => x.UserIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetMessagesEndpoint.Request, User>(db, institutionId);
                    });
                });

            // SenderIds
            RuleFor(x => x.SenderIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.SenderIds != null, () => {
                        RuleFor(x => x.SenderIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetMessagesEndpoint.Request, User>(db, institutionId);
                    });
                });

            // CourseIds
            RuleFor(x => x.CourseIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.CourseIds != null, () => {
                        RuleFor(x => x.CourseIds!)
                            .MustBeValidInstitutionEntityIdsList<GetMessagesEndpoint.Request, Course>(db, institutionId);
                    });
                });
        }
    }
}
