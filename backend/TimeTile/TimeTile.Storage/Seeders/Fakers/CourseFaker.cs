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
    internal class CourseFaker : BaseFaker<Course>
    {
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
                .RuleFor(c => c.Title, (f, c) => GenerateValidTitle(c.Subject.Title, c.Term.StartDate, c.Term.EndDate))
                .RuleFor(c => c.IsAdvanced, f => f.Random.Bool(0.2f));
        }

        private string GenerateValidTitle(string subjectTitle, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            string mainPart = $"{subjectTitle.Substring(0, 10)} {startDate.Date:dd.MM.yyyy}-{endDate.Date:dd.MM.yyyy}";

            return TruncateToMaxLength(MakeUniqueValue(mainPart), RegexPatterns.Patterns[RegexPatterns.Pattern.Title].MaxLength);
        }
    }
}