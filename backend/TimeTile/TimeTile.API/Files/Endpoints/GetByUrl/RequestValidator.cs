using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Http;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Files.Endpoints.GetByUrl
{
    public class RequestValidator : AbstractValidator<GetFileByUrlEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db)
        {
            RuleFor(x => x.Guid)
                .NotEmpty().WithMessage("Guid must not be empty.")
                .NotEqual(Guid.Empty).WithMessage("Guid must be a valid GUID.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Guid)
                        .MustAsync(async (guid, cancellationToken) =>
                        {
                            return await db.Files
                                .AsNoTracking()
                                .AnyAsync(f => f.FileGuid == guid, cancellationToken);
                        })
                        .WithMessage("Guid is invalid");
                });
        }
    }
}
