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
        // For generating unique values
        private static readonly HashSet<string> _usedTitles = new();

        public GroupFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(i => i.Title, GenerateValidTitle)
                .RuleFor(i => i.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> rawTitleGenerator = faker.Company.CatchPhrase;
            Func<string> generator = () => GenerateValidValue(rawTitleGenerator, RegexPatterns.Pattern.Title);

            return MakeUniqueValue(generator, _usedTitles);
        }
    }
}