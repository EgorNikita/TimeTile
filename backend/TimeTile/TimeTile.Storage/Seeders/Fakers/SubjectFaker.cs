using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class SubjectFaker : BaseFaker<Subject>
    {
        public SubjectFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(s => s.Title, GenerateValidTitle)
                .RuleFor(s => s.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> generator = () => MakeUniqueValue($"{faker.Commerce.Department()} Studies");

            return GenerateValidValue(generator, RegexPatterns.Pattern.Title);
        }
    }
}
