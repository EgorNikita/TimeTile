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
    internal class RoleFaker : BaseFaker<Role>
    {
        public RoleFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(r => r.Title, GenerateValidTitle)
                .RuleFor(r => r.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> generator = () =>
                MakeUniqueValue(faker.Name.JobTitle());

            return GenerateValidValue(generator, RegexPatterns.Pattern.Title);
        }
    }
}
