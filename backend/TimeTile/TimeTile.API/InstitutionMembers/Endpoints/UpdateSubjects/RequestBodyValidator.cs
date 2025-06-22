using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.UpdateSubjects
{
    public class RequestBodyValidator : AbstractValidator<UpdateTeacherSubjectsEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SubjectsToAdd)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.SubjectsToAdd != null, () =>
                    {
                        RuleFor(x => x.SubjectsToAdd!)
                            .MustBeValidInstitutionEntityIdsList<UpdateTeacherSubjectsEndpoint.RequestBody, Subject>(db, institutionId);
                    });
                });

            RuleFor(x => x.SubjectsToRemove)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.SubjectsToRemove != null, () =>
                    {
                        RuleFor(x => x.SubjectsToRemove!)
                            .MustBeValidInstitutionEntityIdsList<UpdateTeacherSubjectsEndpoint.RequestBody, Subject>(db, institutionId);
                    });
                });
        }
    }
}
