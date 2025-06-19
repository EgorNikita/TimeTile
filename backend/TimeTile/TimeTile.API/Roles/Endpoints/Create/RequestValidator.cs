using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.Create;

public class RequestValidator : AbstractValidator<CreateRoleEndpoint.Request>
{
    public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
    {
        var institutionId = institutionProvider.GetInstitutionId();

        RuleFor(x => x.Title)
            .MustBeValidTitle()
            .MustAsync(async (title, cancellationToken) =>
            {
                title = title.Trim();

                return !await db.Roles
                    .Where(x => x.InstitutionId == institutionId)
                    .AnyAsync(x => x.Title == title, cancellationToken);
            })
            .WithMessage("Title is already taken.");

        RuleFor(x => x.PermissionsIds)
            .MustBeValidListOfIds()
            .MustAsync(async (list, cancellationToken) =>
            {       // Check if all PermissionsIds are valid
                var count = await db.Permissions.CountAsync(p => list.Contains(p.Id), cancellationToken);
                return count == list.Count();
            })
            .WithMessage("Permissions IDs are invalid.");
    }
}