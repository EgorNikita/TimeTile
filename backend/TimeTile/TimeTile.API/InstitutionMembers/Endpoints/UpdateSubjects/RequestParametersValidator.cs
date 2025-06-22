using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.UpdateSubjects
{
    public class RequestParametersValidator : AbstractValidator<UpdateSubjectsEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.TeacherId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.TeacherId)
                        .MustBeValidOptionalInstitutionEntityId<UpdateSubjectsEndpoint.RequestParameters, InstitutionMember>(db, institutionId);
                });
        }
    }
}
