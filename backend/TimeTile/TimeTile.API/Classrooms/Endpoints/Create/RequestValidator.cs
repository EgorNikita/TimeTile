using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Classrooms.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateClassroomEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Title)
                .MustBeValidTitle();

            RuleFor(x => x.Capacity)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Capacity should be greater or equal to 1");

            RuleFor(x => x.ClassroomTypeId)
                .MustBeValidId()
                .MustBeValidForeignKey<CreateClassroomEndpoint.Request, ClassroomType>(db, institutionId);
        }
    }
}
