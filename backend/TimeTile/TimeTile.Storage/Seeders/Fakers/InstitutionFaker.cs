using Bogus;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class InstitutionFaker : BaseFaker<Institution>
    {
        // For generating unique values
        private readonly HashSet<string> _usedEmails = new();
        private readonly HashSet<string> _usedDomains = new();

        public InstitutionFaker()
        {
            _faker
                .RuleFor(i => i.Title, GenerateValidTitle)
                .RuleFor(i => i.Address, GenerateValidAddress)
                .RuleFor(i => i.PhoneNumber, GenerateValidPhoneNumber)
                .RuleFor(i => i.Email, GenerateValidEmail)
                .RuleFor(i => i.Domain, GenerateValidDomain);
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> generator = () => MakeUniqueValue(faker.Company.CompanyName());

            return GenerateValidValue(generator, RegexPatterns.Pattern.Title);
        }

        private string GenerateValidAddress(Faker faker)
        {
            Func<string> generator = faker.Address.FullAddress;

            return GenerateValidValue(generator, RegexPatterns.Pattern.Address);
        }

        private string GenerateValidPhoneNumber(Faker faker)
        {
            Func<string> generator = () => $"+1{faker.Phone.PhoneNumber("##########")}";

            return GenerateValidValue(generator, RegexPatterns.Pattern.PhoneE164);
        }

        private string GenerateValidEmail(Faker faker)
        {
            Func<string> generator = () => faker.Internet.Email();

            while (true)
            {
                string email = GenerateValidValue(generator, RegexPatterns.Pattern.Email);

                if (! _usedEmails.Contains(email))
                {
                    _usedEmails.Add(email);

                    return email;
                }
            }
        }

        private string GenerateValidDomain(Faker faker)
        {
            Func<string> generator = () => faker.Internet.DomainName();

            while (true)
            {
                string domain = GenerateValidValue(generator, RegexPatterns.Pattern.Domain);

                if (! _usedDomains.Contains(domain))
                {
                    _usedDomains.Add(domain);

                    return domain;
                }
            }
        }
    }
}
