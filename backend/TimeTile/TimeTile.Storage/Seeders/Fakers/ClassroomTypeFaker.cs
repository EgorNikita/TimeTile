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
    internal class ClassroomTypeFaker : BaseFaker<ClassroomType>
    {
        // Description constraints
        private const int DESCRIPTION_WORDS_COUNT = 3;

        // For generating unique values
        private static readonly HashSet<string> _usedDescriptions = new();

        public ClassroomTypeFaker(List<Institution> institutions)            // TODO: add icons
        {
            _faker
                .RuleFor(t => t.Description, GenerateValidDescription)
                .RuleFor(t => t.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidDescription(Faker faker)
        {
            Func<string> rawDescriptionGenerator = () => faker.Lorem.Sentence(DESCRIPTION_WORDS_COUNT);
            Func<string> generator = () => GenerateValidValue(rawDescriptionGenerator, RegexPatterns.Pattern.Description);

            return MakeUniqueValue(generator, _usedDescriptions);
        }
    }
}
