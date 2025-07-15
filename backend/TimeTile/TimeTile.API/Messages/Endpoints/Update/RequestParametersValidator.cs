using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Endpoints.Update
{
    public class RequestParametersValidator : AbstractValidator<UpdateMessageEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider, IUserProvider userProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();
            var userId = userProvider.GetUserId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidEntityId<UpdateMessageEndpoint.RequestParameters, Message>(db)
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
                                .WithMessage("There is no Message with this id in your current institution.")
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x.Id)
                                        .MustAsync(async (id, cancellationToken) =>
                                        {
                                            return await db.Messages
                                                .Where(m => m.Id == id)
                                                .Select(m => m.UserId)
                                                .FirstAsync(cancellationToken) == userId;
                                        })
                                        .WithMessage("This message does not belong to you.");
                                });
                        });
                });
        }
    }
}
