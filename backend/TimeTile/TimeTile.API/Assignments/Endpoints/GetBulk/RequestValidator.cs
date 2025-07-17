using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Assignments.Endpoints.GetBulk
{
    public class RequestValidator : AbstractValidator<GetAssignmentsBulkEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Ids)
                .MustBeValidListOfIds()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ids)
                        .MustBeValidEntityIdsList<GetAssignmentsBulkEndpoint.Request, Assignment>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Ids)
                                .MustAsync(async (ids, cancellationToken) =>
                                {
                                    var invalidAssignmentExists = await db.Assignments
                                        .AsNoTracking()
                                        .Where(a => ids.Contains(a.Id))
                                        .AnyAsync(a => a.Lesson.Course.InstitutionId != institutionId, cancellationToken);

                                    return !invalidAssignmentExists;
                                })
                                .WithMessage("Some of Assignments do not belong to the current institution");
                        });
                });
        }
    }
}
