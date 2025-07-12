using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Courses.Endpoints.UpdateStudents;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using File = TimeTile.Core.Models.File;

namespace TimeTile.API.Assignments.Endpoints.Update
{
    public class RequestBodyValidator : AbstractValidator<UpdateAssignmentEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            When(x => x.Title != null, () =>
            {
                RuleFor(x => x.Title!)
                    .MustBeValidTitle();
            });

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description!)
                    .MustBeValidDescription();
            });

            When(x => x.Deadline != null, () =>
            {
                RuleFor(x => x.Deadline!)
                    .Must(deadline => deadline > DateTimeOffset.UtcNow)
                    .WithMessage("Deadlien should be in the future.");
            });

            RuleFor(x => x.FilesToRemove)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.FilesToRemove != null, () =>
                    {
                        RuleFor(x => x.FilesToRemove!)
                            .MustBeValidEntityIdsList<UpdateAssignmentEndpoint.RequestBody, File>(db);
                    });
                });
        }
    }
}
