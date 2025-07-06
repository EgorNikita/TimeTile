using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Groups.Endpoints.UpdateInstitutionMembers
{
    public class RequestBodyValidator : AbstractValidator<UpdateGroupInstitutionMembersEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.InstitutionMembersToAdd)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.InstitutionMembersToAdd != null, () =>
                    {
                        RuleFor(x => x.InstitutionMembersToAdd!)
                            .MustBeValidOptionalInstitutionEntityIdsList<UpdateGroupInstitutionMembersEndpoint.RequestBody, InstitutionMember>(db, institutionId);
                    });
                });

            RuleFor(x => x.InstitutionMembersToRemove)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.InstitutionMembersToRemove != null, () =>
                    {
                        RuleFor(x => x.InstitutionMembersToRemove!)
                            .MustBeValidOptionalInstitutionEntityIdsList<UpdateGroupInstitutionMembersEndpoint.RequestBody, InstitutionMember>(db, institutionId);
                    });
                });
        }
    }
}
