using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Assignments.Endpoints.Update;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using File = TimeTile.Core.Models.File;

namespace TimeTile.API.Submissions.Endpoints.Submit
{
    public class RequestBodyValidator : AbstractValidator<SubmitSubmissionEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db)
        {
            When(x => x.StudentNote != null, () =>
            {
                RuleFor(x => x.StudentNote!)
                    .MustBeValidString();
            });

            RuleFor(x => x.FilesToRemove)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.FilesToRemove != null, () =>
                    {
                        RuleFor(x => x.FilesToRemove!)
                            .MustBeValidEntityIdsList<SubmitSubmissionEndpoint.RequestBody, File>(db);
                    });
                });
        }
    }
}