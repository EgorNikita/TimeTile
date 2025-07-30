using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Storage.Contexts;
using Microsoft.EntityFrameworkCore;

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

            When(x => x.FilesToRemove != null, () =>
            {
                RuleFor(x => x.FilesToRemove!)
                    .MustAsync(async (fileGuids, cancellationToken) =>
                    {
                        foreach (var fileGuid in fileGuids)
                        {
                            Guid guid = Guid.Parse(fileGuid);
                            if (await db.Files.AnyAsync(f => f.FileGuid == guid, cancellationToken))
                                continue;

                            return false;
                        }

                        return true;
                    })
                    .WithMessage("Some Guids are invalid.");
            });
        }
    }
}
