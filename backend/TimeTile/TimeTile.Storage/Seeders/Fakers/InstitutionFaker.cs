using Bogus;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class InstitutionFaker : BaseFaker<Institution>
    {
        // Title constraints
        private const string TITLE_REGEX = @"^[\w \-.*&""'',\/\\|]+$";
        private const int TITLE_MAX_LENGTH = 255;

        // Address constraints
        private const string ADDRESS_REGEX = @"^[A-Za-z\d''\.\- \,]+$";
        private const int ADDRESS_MAX_LENGTH = 255;

        // PhoneNumber constraints
        private const string PHONE_NUMBER_REGEX = @"^(\+\d{1,2} )?\(?\d{3}\)?[ .-]\d{3}[ .-]\d{4}$";
        private const int PHONE_NUMBER_MAX_LENGTH = 20;

        // Email constraints
        private const string EMAIL_REGEX = @"^[A-Za-z\d._%+-]+@[A-Za-z\d.-]+\.[A-Za-z]{2,}$";
        private const int EMAIL_MAX_LENGTH = 255;

        public InstitutionFaker()
        {
            _faker
                .RuleFor(i => i.Title, GenerateValidTitle)
                .RuleFor(i => i.Address, GenerateValidAddress)
                .RuleFor(i => i.PhoneNumber, GenerateValidPhoneNumber)
                .RuleFor(i => i.Email, GenerateValidEmail);
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> generator = () => MakeUniqueValue(faker.Company.CompanyName());

            return GenerateValidValue(generator, TITLE_REGEX, TITLE_MAX_LENGTH);
        }

        private string GenerateValidAddress(Faker faker)
        {
            Func<string> generator = () => faker.Address.FullAddress();

            return GenerateValidValue(generator, ADDRESS_REGEX, ADDRESS_MAX_LENGTH);
        }

        private string GenerateValidPhoneNumber(Faker faker)
        {
            Func<string> generator = () => faker.Phone.PhoneNumber();

            return GenerateValidValue(generator, PHONE_NUMBER_REGEX, PHONE_NUMBER_MAX_LENGTH);
        }

        private string GenerateValidEmail(Faker faker)
        {
            Func<string> generator = () => faker.Internet.Email();

            return GenerateValidValue(generator, EMAIL_REGEX, EMAIL_MAX_LENGTH);
        }
    }
}
