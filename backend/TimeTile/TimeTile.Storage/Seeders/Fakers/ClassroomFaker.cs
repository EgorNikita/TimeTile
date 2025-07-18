using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class ClassroomFaker : BaseFaker<Classroom>
    {
        // Capacity constraints
        private const int CAPACITY_MIN_VALUE = 10;
        private const int CAPACITY_MAX_VALUE = 40;

        // For generating unique values
        private static readonly HashSet<string> _usedTitles = new();

        // Caching for optimization
        private readonly Dictionary<int, List<ClassroomType>> _institutionClassroomTypes = new();

        public ClassroomFaker(List<Institution> institutions, List<ClassroomType> classroomTypes) 
        {
            var suitableInstitutions = institutions
                .Where(i => classroomTypes.Any(t => t.InstitutionId == i.Id));

            if (!suitableInstitutions.Any())
                throw new InvalidOperationException("There are no associations between institutions and classroom types.");

            _faker
                .RuleFor(c => c.Capacity, f => f.Random.Int(CAPACITY_MIN_VALUE, CAPACITY_MAX_VALUE))
                .RuleFor(c => c.Title, GenerateValidTitle)
                .Rules((faker, classroom) =>
                {
                    classroom.InstitutionId = faker.PickRandom(suitableInstitutions).Id;

                    classroom.ClassroomTypeId = PickAssociatedEntity(
                        faker,
                        classroom.InstitutionId,
                        classroomTypes,
                        _institutionClassroomTypes,
                        t => t.InstitutionId == classroom.InstitutionId
                    ).Id;
                });
        }

        private string GenerateValidTitle(Faker faker)
        {
            Func<string> rawTitleGenerator = () => GenerateRandomTitle(faker);
            Func<string> generator = () => GenerateValidValue(rawTitleGenerator, RegexPatterns.Pattern.Title);

            return MakeUniqueValue(generator, _usedTitles);
        }

        private static string GenerateRandomTitle(Faker faker)
        {
            char building = faker.Random.Char('A', 'Z');
            int classroomNumber = faker.Random.Int(1, 1000);

            return $"{building}{classroomNumber}";
        }
    }
}
