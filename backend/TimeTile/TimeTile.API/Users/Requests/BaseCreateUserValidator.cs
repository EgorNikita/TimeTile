using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.InstitutionMembers.Endpoints.Create;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Users.Requests
{
    public abstract class BaseCreateUserValidator<T> : AbstractValidator<T>
        where T : class, ICreateUserRequest
    {
        public BaseCreateUserValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
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
                    var validator = new InlineValidator<ICreateUserRequest>();

                    validator.RuleFor(x => x.Firstname).MustBeValidName();
                    validator.RuleFor(x => x.Lastname).MustBeValidName();
                    validator.RuleFor(x => x.BirthDate).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

                    var result = validator.Validate(request);
                    return result.IsValid;
                });
        }
    }
}
