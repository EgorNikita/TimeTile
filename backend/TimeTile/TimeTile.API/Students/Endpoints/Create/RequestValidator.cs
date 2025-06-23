using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Users.Requests;
using TimeTile.Core.Common.Regex;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.Create;

public class RequestValidator : BaseCreateUserValidator<CreateStudentEndpoint.Request>
{
    public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        : base(db, institutionProvider)
    {
        var institutionId = institutionProvider.GetInstitutionId();

        RuleFor(u => u.GroupId)
            .Must(id => id == null || id >= 1)
            .WithMessage("Id must be greater or equal to 1")
            .DependentRules(() =>
            {
                RuleFor(u => u.GroupId)
                    .MustAsync(async (groupId, cancellationToken) =>
                    {
                        if (groupId is null)
                            return true;

                        return await db.Groups
                            .Where(g => g.InstitutionId == institutionId)
                            .AnyAsync(g => g.Id == groupId, cancellationToken);
                    })
                    .WithMessage("Group's ID is invalid.");
            });
    }
}