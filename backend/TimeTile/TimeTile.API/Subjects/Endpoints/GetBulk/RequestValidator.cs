using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Subjects.Endpoints.Get;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Subjects.Endpoints.GetBulk
{
    public class RequestValidator : AbstractValidator<GetSubjectsBulkEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Ids)
                .MustBeValidListOfIds()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ids)
                        .MustBeValidInstitutionEntityIdsList<GetSubjectsBulkEndpoint.Request, Subject>(db, institutionId);
                });
        }
    }
}
