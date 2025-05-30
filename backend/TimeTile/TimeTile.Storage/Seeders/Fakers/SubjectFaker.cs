using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class SubjectFaker : BaseFaker<Subject>
    {
        // Title constraints
        private const string TITLE_REGEX = @"^[\w -]+$";
        private const int TITLE_MAX_LENGTH = 255;

        public SubjectFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(s => s.Title, GenerateValidTitle)
                .RuleFor(s => s.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> generator = () => MakeUniqueValue($"{faker.Commerce.Department()} Studies");

            return GenerateValidValue(generator, TITLE_REGEX, TITLE_MAX_LENGTH);
        }
    }
}
