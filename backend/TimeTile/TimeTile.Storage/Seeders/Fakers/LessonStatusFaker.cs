using Bogus;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class LessonStatusFaker : BaseFaker<LessonStatus>
    {
        // Description constraints
        private const string DESCRIPTION_REGEX = @"^[a-zA-Z\d ]+$";
        private const int DESCRIPTION_MAX_LENGTH = 255;

        private int _currentId = 0;

        private static readonly List<string> _descriptions = new()
        {
            "Scheduled",
            "Canceled",
            "Replace",
            "New room",
            "Test"
        };

        public LessonStatusFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(s => s.Description, GenerateValidDescription)
                .RuleFor(s => s.ArgbColor, GenerateArgbColor)
                .RuleFor(s => s.InstitutionId, f => f.PickRandom(institutions).Id);
        }

        private string GenerateValidDescription(Faker faker)
        {
            Func<string> descriptionGenerator;

            // 70% - one value from _descriptions
            // 30% - some new status
            if (faker.Random.Bool(0.7f))
            {
                _currentId++;
                descriptionGenerator = () => _descriptions.ElementAt((_currentId - 1) % _descriptions.Count);     // starts from 0
            }
            else
                descriptionGenerator = faker.Random.Word;

            Func<string> uniqueDescriptionGenerator = () => MakeUniqueValue(descriptionGenerator());

            return GenerateValidValue(uniqueDescriptionGenerator, DESCRIPTION_REGEX, DESCRIPTION_MAX_LENGTH);
        }

        private int GenerateArgbColor(Faker faker)
        {
            return (faker.Random.Byte() << 24)  // Alpha
                | (faker.Random.Byte() << 16)  // Red
                | (faker.Random.Byte() << 8)   // Green
                | faker.Random.Byte();         // Blue
        }
    }
}
