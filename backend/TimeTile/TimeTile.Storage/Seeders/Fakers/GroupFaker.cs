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
    internal class GroupFaker : BaseFaker<Group>
    {
        public GroupFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(i => i.Title, GenerateValidTitle)
                .RuleFor(i => i.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> generator = () => MakeUniqueValue(faker.Company.CatchPhrase());

            return GenerateValidValue(generator, RegexPatterns.Pattern.Title);
        }
    }
}