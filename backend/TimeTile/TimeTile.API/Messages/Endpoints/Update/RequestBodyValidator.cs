using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Storage.Contexts;
using TimeTile.Core.Models;
using File = TimeTile.Core.Models.File;

namespace TimeTile.API.Messages.Endpoints.Update
{
    public class RequestBodyValidator : AbstractValidator<UpdateMessageEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db)
        {
            When(x => x.Content != null, () =>
            {
                RuleFor(x => x.Content!)
                    .MustBeValidString();
            });

            RuleFor(x => x.FilesToRemove)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.FilesToRemove != null, () =>
                    {
                        RuleFor(x => x.FilesToRemove!)
                            .MustBeValidEntityIdsList<UpdateMessageEndpoint.RequestBody, File>(db);
                    });
                });
        }
    }
}
