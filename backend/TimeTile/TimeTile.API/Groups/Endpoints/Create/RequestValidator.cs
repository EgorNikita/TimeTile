using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Groups.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateGroupEndpoint.Request>
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

                            return !await db.Groups
                                .Where(g => g.InstitutionId == institutionId)
                                .AnyAsync(g => g.Title == title, cancellationToken);
                        })
                        .WithMessage("Title is already taken.");
                });

            RuleFor(x => x.StudentIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentIds != null, () =>
                    {
                        RuleFor(x => x.StudentIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<CreateGroupEndpoint.Request, Student>(db, institutionId);
                    });
                });

            RuleFor(x => x.InstitutionMemberIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.InstitutionMemberIds != null, () =>
                    {
                        RuleFor(x => x.InstitutionMemberIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<CreateGroupEndpoint.Request, InstitutionMember>(db, institutionId);
                    });
                });
        }
    }
}
