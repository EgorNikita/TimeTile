using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.UpdateGroups
{
    public class RequestBodyValidator : AbstractValidator<UpdateInstitutionMemberGroupsEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.GroupsToAdd)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.GroupsToAdd != null, () =>
                    {
                        RuleFor(x => x.GroupsToAdd!)
                            .MustBeValidInstitutionEntityIdsList<UpdateInstitutionMemberGroupsEndpoint.RequestBody, Group>(db, institutionId);
                    });
                });

            RuleFor(x => x.GroupsToRemove)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.GroupsToRemove != null, () =>
                    {
                        RuleFor(x => x.GroupsToRemove!)
                            .MustBeValidInstitutionEntityIdsList<UpdateInstitutionMemberGroupsEndpoint.RequestBody, Group>(db, institutionId);
                    });
                });
        }
    }
}
