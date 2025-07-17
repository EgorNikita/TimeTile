using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class CourseFaker : BaseFaker<Course>
    {
        // Title constraints
        private const float MODIFIER_PRESENCE_POSSIBILITY = 0.4f;
        private const float YEAR_INFO_PRESENCE_POSSIBILITY = 0.5f;
        private const float TEACHER_INFO_PRESENCE_POSSIBILITY = 0.3f;

        // Cashing for optimization
        private readonly Dictionary<int, List<Subject>> _institutionSubjects = new();
        private readonly Dictionary<int, List<InstitutionMember>> _subjectTeachers = new();
        private readonly Dictionary<int, List<Term>> _institutionTerms = new();

        // For generating unique values
        private static readonly HashSet<string> _usedTitles = new();

        private readonly ICourseService _courseService;
        private readonly IFileService _fileService;

        public CourseFaker(
            List<Subject> subjects, 
            List<InstitutionMember> teachers, 
            List<Institution> institutions, 
            List<Term> terms, 
            ICourseService courseService, 
            IFileService fileService)
        {
            _courseService = courseService;
            _fileService = fileService;

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
                .RuleFor(c => c.Title, (f, c) => GenerateValidTitle(f, c.Teacher.Lastname, c.Subject.Title, c.Term.StartDate))
                .RuleFor(c => c.IsAdvanced, f => f.Random.Bool(0.2f));
        }

        private string GenerateValidTitle(Faker faker, string teacherName, string subjectTitle, DateTimeOffset startDate)
        {
            var modifiers = new[]
            {
                "Intro to", "Advanced", "Seminar", "Workshop", "Project", "Lab", "Review",
                "New Course", "Theory of", "Basics of", "Applied", "Foundations of", "Essentials of"
            };

            Func<string> rawTitleGenerator = () =>
            {
                var modifier = faker.Random.Bool(MODIFIER_PRESENCE_POSSIBILITY) 
                    ? $"{faker.PickRandom(modifiers)}"
                    : string.Empty;

                var year = faker.Random.Bool(YEAR_INFO_PRESENCE_POSSIBILITY) 
                    ? startDate.Year.ToString()
                    : string.Empty;

                var teacherInfo = faker.Random.Bool(TEACHER_INFO_PRESENCE_POSSIBILITY)
                    ? teacherName
                    : string.Empty;

                return $"{modifier} {subjectTitle} {year} {teacherInfo}".Trim();
            };
            Func<string> generator = () => GenerateValidValue(rawTitleGenerator, RegexPatterns.Pattern.Title);

            return MakeUniqueValue(generator, _usedTitles);
        }


        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private Task GenerateIcons(List<Course> courses, CancellationToken cancellationToken)
        {
            return Task.WhenAll(courses.Select(async course =>
            {
                var avatarStream = await _courseService.GenerateDefaultIcon(course.Title);

                var fileName = $"{course.Title}_icon.png";

                int savedFileId;

                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    savedFileId = await _fileService.SaveFile(avatarStream, fileName, cancellationToken);
                }
                finally
                {
                    _semaphore.Release();
                }

                course.IconId = savedFileId;
            }));
        }


        public override List<Course> Generate(int count)
        {
            var courses = base.Generate(count);

            Task.Run(async () =>
                await GenerateIcons(courses, CancellationToken.None)).Wait();

            return courses;
        }

        public async Task<List<Course>> GenerateAsync(int count, CancellationToken cancellationToken)
        {
            var courses = base.Generate(count);

            await GenerateIcons(courses, cancellationToken);

            return courses;
        }
    }
}