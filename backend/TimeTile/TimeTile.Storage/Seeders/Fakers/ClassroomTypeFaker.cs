using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class ClassroomTypeFaker : BaseFaker<ClassroomType>
    {
        // Description constraints
        private const int DESCRIPTION_WORDS_COUNT = 3;
        private const int DESCRIPTION_MAX_LENGTH = 255;

        public ClassroomTypeFaker(List<Institution> institutions)            // TODO: add icons
        {
            _faker
                .RuleFor(t => t.Description, GenerateValidDescription)
                .RuleFor(t => t.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidDescription(Faker faker)
        {
            string description = faker.Lorem.Sentence(DESCRIPTION_WORDS_COUNT);
            string uniqueDescription = MakeUniqueValue(description);

            return TruncateToMaxLength(uniqueDescription, DESCRIPTION_MAX_LENGTH);
        }
    }
}
