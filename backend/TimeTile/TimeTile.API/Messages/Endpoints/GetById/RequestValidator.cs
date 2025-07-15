using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetMessageByIdEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidEntityId<GetMessageByIdEndpoint.Request, Message>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Id)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    return await db.Messages
                                        .Where(m => m.Id == id)
                                        .Select(m => m.Course.InstitutionId)
                                        .FirstAsync(cancellationToken) == institutionId;
                                })
                                .WithMessage("There is no Message with this id in your current institution");
                        });
                });
        }
    }
}
