using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Classrooms.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateClassroomEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Title)
                .MustBeValidTitle()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Title)
                        .MustAsync(async (title, cancellationToken) =>
                        {
                            title = title.Trim();

                            return !await db.Classrooms
                                .Where(c => c.InstitutionId == institutionId)
                                .AnyAsync(c => c.Title == title, cancellationToken);
                        })
                        .WithMessage("Title is already taken.");
                });

            RuleFor(x => x.Capacity)
                .NotEmpty()
                .WithMessage("Capacity is required")
                .DependentRules(() => 
                {
                    RuleFor(x => x.Capacity)
                        .GreaterThanOrEqualTo(1)
                        .WithMessage("Capacity should be greater or equal to 1");
                });

            RuleFor(x => x.ClassroomTypeId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.ClassroomTypeId)
                        .MustBeValidInstitutionEntityId<CreateClassroomEndpoint.Request, ClassroomType>(db, institutionId);
                });
        }
    }
}
