using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Enums;

namespace TimeTile.API.Submissions.Endpoints.Review
{
    public class RequestBodyValidator : AbstractValidator<ReviewSubmissionEndpoint.RequestBody>
    {
        public RequestBodyValidator()
        {
            When(x => x.Grade.HasValue, () =>
            {
                RuleFor(x => x.Grade.Value!.Weight)
                    .GreaterThan(0)
                    .WithMessage("Weight should be greater than zero");

                RuleFor(x => (int)x.Grade.Value!.Value)
                    .GreaterThan(0)
                    .WithMessage("Value of grade should be greater than zero");
            });

            When(x => x.Feedback.HasValue, () =>
            {
                RuleFor(x => x.Feedback.Value!)
                    .MustBeValidString();
            });
        }
    }
}
