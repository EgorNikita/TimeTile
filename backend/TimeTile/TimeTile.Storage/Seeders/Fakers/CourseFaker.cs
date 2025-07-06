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
        // Cashing for optimization
        private readonly Dictionary<int, List<Subject>> _institutionSubjects = new();
        private readonly Dictionary<int, List<InstitutionMember>> _subjectTeachers = new();
        private readonly Dictionary<int, List<Term>> _institutionTerms = new();

        private static readonly Dictionary<int, int> _teachersOrderNumbers = new();

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

                    var teacherId = course.Teacher.Id;

                    if (!_teachersOrderNumbers.ContainsKey(teacherId))
                    {
                        _teachersOrderNumbers.Add(teacherId, 1);
                    }

                    var orderNumber = _teachersOrderNumbers.GetValueOrDefault(teacherId);

                    course.CoursesToUsers.Add(new CourseToUser { UserId = teacherId, OrderNumber = orderNumber });

                    _teachersOrderNumbers[teacherId] = orderNumber + 1;

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