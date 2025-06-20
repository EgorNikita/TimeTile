using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Regex;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.Create;

public class RequestValidator : AbstractValidator<CreateStudentEndpoint.Request>
{
    public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
    {
        var institutionId = institutionProvider.GetInstitutionId();

        RuleFor(u => u.Firstname)
            .MustBeValidName();

        RuleFor(u => u.Lastname)
            .MustBeValidName();

        RuleFor(u => u.PhoneNumber)
            .MustBeValidPhoneNumber();

        RuleFor(u => u.HomeAddress)
            .MustBeValidAddress();

        RuleFor(u => u.BirthDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("BirthDate cannot be in the future.");

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

        RuleFor(u => u)
            .MustAsync(async (request, cancellationToken) =>
            {
                var firstname = request.Firstname.Trim();
                var lastname = request.Lastname.Trim();

                return !await db.Users
                    .AsNoTracking()
                    .Where(u => u.InstitutionId == institutionId)
                    .AnyAsync(u =>
                        u.Firstname == firstname &&
                        u.Lastname == lastname &&
                        u.BirthDate == request.BirthDate,
                        cancellationToken
                    );
            })
            .WithMessage("User with such personal data already exists")
            // Call to the database only in case of successfull validation before
            .When(request => 
            {
                var validator = new InlineValidator<CreateStudentEndpoint.Request>();

                validator.RuleFor(x => x.Firstname).MustBeValidName();
                validator.RuleFor(x => x.Lastname).MustBeValidName();
                validator.RuleFor(x => x.BirthDate).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

                var result = validator.Validate(request);
                return result.IsValid;
            });
    }
}