using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Enums;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Update
{
    public class RequestBodyValidator : AbstractValidator<UpdateSubmissionEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db)
        {
            When(x => x.GradeWeight != null || x.GradeValue != null, () =>
            {
                RuleFor(x => x.GradeWeight)
                    .Must(weight => weight != null)
                    .WithMessage("GradeWeight is required if grade is provided.")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.GradeWeight!.Value)
                            .GreaterThan(0)
                            .WithMessage("Weight should be greater than zero");
                    });

                RuleFor(x => x.GradeValue)
                    .Must(value => value != null)
                    .WithMessage("GradeValue is required if grade is provided.")
                    .DependentRules(() =>
                    {
                        RuleFor(x => (int)x.GradeValue!.Value)
                            .GreaterThan(0)
                            .WithMessage("Value of grade should be greater than zero");
                    });

                RuleFor(x => x.Status)
                    .Must(status => status == null || status == SubmissionStatus.Accepted)
                    .WithMessage("Grade can be passed only if Submission status is Accepted.");
            });

            When(x => x.Feedback != null, () =>
            {
                RuleFor(x => x.Feedback!)
                    .MustBeValidString();
            });

            When(x => x.StudentNote != null, () =>
            {
                RuleFor(x => x.StudentNote!)
                    .MustBeValidString();
            });

            When(x => x.FilesToRemove != null, () =>
            {
                RuleFor(x => x.FilesToRemove!)
                    .MustBeValidOptionalListOfIds()
                    .DependentRules(() =>
                    {
                        When(x => x.FilesToRemove != null, () =>
                        {
                            RuleFor(x => x.FilesToRemove!)
                                .MustBeValidEntityIdsList<UpdateSubmissionEndpoint.RequestBody, Core.Models.File>(db);
                        });
                    });
            });
        }
    }
}
