using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Groups.Endpoints.UpdateStudents
{
    public class RequestBodyValidator : AbstractValidator<UpdateGroupStudentsEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.StudentsToAdd)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentsToAdd != null, () =>
                    {
                        RuleFor(x => x.StudentsToAdd!)
                            .MustBeValidOptionalInstitutionEntityIdsList<UpdateGroupStudentsEndpoint.RequestBody, Student>(db, institutionId);
                    });
                });

            RuleFor(x => x.StudentsToRemove)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentsToRemove != null, () =>
                    {
                        RuleFor(x => x.StudentsToRemove!)
                            .MustBeValidOptionalInstitutionEntityIdsList<UpdateGroupStudentsEndpoint.RequestBody, Student>(db, institutionId);
                    });
                });
        }
    }
}
