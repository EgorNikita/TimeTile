using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.ClassroomTypes.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateClassroomTypeEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Description)
                .MustBeValidDescription()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Description)
                        .MustAsync(async (description, cancellationToken) =>
                        {
                            description = description.Trim();

                            return !await db.ClassroomTypes
                                .Where(type => type.InstitutionId == institutionId)
                                .AnyAsync(type => type.Description == description, cancellationToken);
                        })
                        .WithMessage("Description is already taken.");
                });
        }
    }
}
