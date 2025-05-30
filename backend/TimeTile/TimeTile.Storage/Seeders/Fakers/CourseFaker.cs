using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class CourseFaker : BaseFaker<Course>
    {
        // Title constraints
        private const string TITLE_REGEX = @"^[\w -.*+,]+$";
        private const int TITLE_MAX_LENGTH = 255;

        // Cashing for optimization
        private readonly Dictionary<int, List<Subject>> _institutionSubjects = new();
        private readonly Dictionary<int, List<InstitutionMember>> _subjectTeachers = new();
        private readonly Dictionary<int, List<Term>> _institutionTerms = new();

        public CourseFaker(List<Subject> subjects, List<InstitutionMember> teachers, List<Institution> institutions, List<Term> terms)
        {
            var suitableInstitutions = institutions
                .Where(i =>
                    subjects.Any(s => s.InstitutionId == i.Id) &&
                    teachers.Any(m => m.InstitutionId == i.Id) &&
                    terms.Any(t => t.InstitutionId == i.Id))
                .ToList();

            if (suitableInstitutions.Count == 0)
                throw new InvalidOperationException("There are no associations between institutions, subjects, teachers and terms.");

            var suitableSubjects = subjects
                .Where(s => s.Teachers.Any(teacher => teachers.Any(t => t.Id == teacher.Id)))
                .ToList();

            if (suitableSubjects.Count == 0)
                throw new InvalidOperationException("There are no associations between subjects and teachers.");

            _faker
                .Rules((faker, course) =>
                {
                    course.InstitutionId = faker.PickRandom(suitableInstitutions).Id;

                    course.Subject = PickAssociatedEntity(
                        faker,
                        course.InstitutionId,
                        suitableSubjects,
                        _institutionSubjects,
                        s => s.InstitutionId == course.InstitutionId
                    );

                    course.Teacher = PickAssociatedEntity(
                        faker,
                        course.Subject.Id,
                        teachers,
                        _subjectTeachers,
                        t => t.Subjects.Any(s => s.Id == course.Subject.Id)
                    );

                    course.Term = PickAssociatedEntity(
                        faker,
                        course.InstitutionId,
                        terms,
                        _institutionTerms,
                        t => t.InstitutionId == course.InstitutionId
                    );
                })
                .RuleFor(c => c.Title, (f, c) => GenerateValidTitle(f, c.Subject.Title, c.Term.StartDate, c.Term.EndDate))
                .RuleFor(c => c.IsAdvanced, f => f.Random.Bool(0.2f));
        }

        private string GenerateValidTitle(Faker faker, string subjectTitle, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            Func<string> generator = () =>
            {
                string mainPart = $"{subjectTitle} {startDate.Date:dd.MM.yyyy}-{endDate.Date:dd.MM.yyyy}";
                return MakeUniqueValue(mainPart);
            };

            return GenerateValidValue(generator, TITLE_REGEX, TITLE_MAX_LENGTH);
        }
    }
}