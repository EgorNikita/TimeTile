using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.UpdateGroups
{
    public class RequestParametersValidator : AbstractValidator<UpdateInstitutionMemberGroupsEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.InstitutionMemberId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.InstitutionMemberId)
                        .MustBeValidOptionalInstitutionEntityId<UpdateInstitutionMemberGroupsEndpoint.RequestParameters, InstitutionMember>(db, institutionId);
                });
        }
    }
}
