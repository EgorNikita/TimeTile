using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class GroupFaker : BaseFaker<Group>
    {
        // Title constraints
        private const string TITLE_REGEX = @"^[\w -.*]+$";
        private const int TITLE_MAX_LENGTH = 255;

        public GroupFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(i => i.Title, GenerateValidTitle)
                .RuleFor(i => i.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> generator = () => MakeUniqueValue(faker.Company.CatchPhrase());

            return GenerateValidValue(generator, TITLE_REGEX, TITLE_MAX_LENGTH);
        }
    }
}