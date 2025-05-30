using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class ClassroomFaker : BaseFaker<Classroom>
    {
        // Capacity constraints
        private const int CAPACITY_MIN_VALUE = 10;
        private const int CAPACITY_MAX_VALUE = 40;

        // Title constraints
        private const int TITLE_MAX_LENGTH = 255;
        private const string TITLE_REGEX = @"^[a-zA-Z \d-]+$";

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

        private static string GenerateRandomTitle(Faker faker)
        {
            char building = faker.Random.Char('A', 'Z');
            int classroomNumber = faker.Random.Int(1, 1000);

            return $"{building}{classroomNumber}";
        }

        private string GenerateValidTitle(Faker faker)
        {
            string randomTitle = GenerateRandomTitle(faker);
            Func<string> generator = () => MakeUniqueValue(randomTitle);

            return GenerateValidValue(generator, TITLE_REGEX, TITLE_MAX_LENGTH);
        }
    }
}
