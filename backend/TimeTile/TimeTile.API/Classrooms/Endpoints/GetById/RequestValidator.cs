using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Classrooms.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetClassroomByIdEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            RuleFor(x => x.Id)
                .MustBeValidId()
                .MustBeValidEntityId<GetClassroomByIdEndpoint.Request, Classroom>(db, institutionProvider.GetInstitutionId());
        }
    }
}
