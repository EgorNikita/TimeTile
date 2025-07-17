using Bogus;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class InstitutionFaker : BaseFaker<Institution>
    {
        // For generating unique values
        private static readonly HashSet<string> _usedTitles = new();
        private static readonly HashSet<string> _usedEmails = new();
        private static readonly HashSet<string> _usedDomains = new();

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
            Func<string> rawTitleGenerator = () => faker.Company.CompanyName();
            Func<string> generator = () => GenerateValidValue(rawTitleGenerator, RegexPatterns.Pattern.Title);

            return MakeUniqueValue(generator, _usedTitles);
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
            Func<string> rawEmailGenerator = () => faker.Internet.Email();
            Func<string> generator = () => GenerateValidValue(rawEmailGenerator, RegexPatterns.Pattern.Email);

            return MakeUniqueValue(generator, _usedEmails);
        }

        private string GenerateValidDomain(Faker faker)
        {
            Func<string> rawDomainGenerator = faker.Internet.DomainName;
            Func<string> generator = () => GenerateValidValue(rawDomainGenerator, RegexPatterns.Pattern.Domain);

            return MakeUniqueValue(generator, _usedDomains);
        }
    }
}
