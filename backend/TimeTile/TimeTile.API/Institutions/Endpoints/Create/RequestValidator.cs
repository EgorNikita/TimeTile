using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Regex;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Institutions.Endpoints.Create;

public class RequestValidator : AbstractValidator<CreateInstitutionEndpoint.Request>
{
    public RequestValidator(TimetileDbContext db)
    {
        RuleFor(x => x.Title)
            .MustBeValidTitle();

        RuleFor(x => x.Address)
            .MustBeValidAddress();

        RuleFor(x => x.PhoneNumber)
            .MustBeValidPhoneNumber();

        RuleFor(x => x.Email)
            .MustBeValidEmail();

        RuleFor(x => x.Domain)
            .MustBeValidString()
            .WithMessage("Domain contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Domain);

        RuleFor(x => x)
            .CustomAsync(async (request, context, cancellationToken) =>
            {
                var title = request.Title.Trim();
                var email = request.Email.Trim();
                var domain = request.Domain.Trim();

                var existing = await db.Institutions
                    .Where(i =>
                        i.Title == title ||
                        i.Email == email ||
                        i.Domain == domain
                    )
                    .Select(i => new { i.Title, i.Email, i.Domain })
                    .FirstOrDefaultAsync(cancellationToken);

                if (existing is not null)
                {
                    if (existing.Title == title)
                    {
                        context.AddFailure("Title", "Title is already taken.");
                    }
                    if (existing.Email == email)
                    {
                        context.AddFailure("Email", "Email is already taken.");
                    }
                    if (existing.Domain == domain)
                    {
                        context.AddFailure("Domain", "Domain is already taken");
                    }
                }
            });
    }
}