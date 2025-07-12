using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Assignments.Endpoints.Update
{
    public class RequestParametersValidator : AbstractValidator<UpdateAssignmentEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Id)
               .MustBeValidId()
               .DependentRules(() =>
               {
                   RuleFor(x => x.Id)
                       .MustBeValidEntityId<UpdateAssignmentEndpoint.RequestParameters, Assignment>(db)
                       .DependentRules(() =>
                       {
                           RuleFor(x => x.Id)
                               .MustAsync(async (id, cancellationToken) =>
                               {
                                   var assignment = await db.Assignments
                                       .Include(a => a.Lesson)
                                           .ThenInclude(l => l.Course)
                                       .FirstAsync(a => a.Id == id, cancellationToken);

                                   return assignment.Lesson.Course.InstitutionId == institutionId;
                               })
                               .WithMessage("There is no Assignment with this id in your current institution");
                       });
               });
        }
    }
}
