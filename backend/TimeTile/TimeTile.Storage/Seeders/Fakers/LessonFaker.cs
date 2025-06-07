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
    internal class LessonFaker : BaseFaker<Lesson>
    {
        // Cashing for optimization
        private readonly Dictionary<int, List<LessonStatus>> _institutionLessonStatuses = new();

        private readonly Dictionary<int, IEnumerable<Course>> _institutionCourses = new();
        private readonly Dictionary<int, IEnumerable<Classroom>> _institutionClassrooms = new();
        private readonly Dictionary<int, IEnumerable<TimetableUnit>> _institutionTimetableUnits = new();

        // Maintain uniqueness of rows
        private readonly HashSet<(int ClassroomId, int TimatableUnitId, DateTimeOffset Date)> _usedClassrooms = new();
        private readonly HashSet<(int TeacherId, int TimatableUnitId, DateTimeOffset Date)> _usedTeachers = new();


        public LessonFaker(List<Institution> institutions, List<Classroom> classrooms, List<Course> courses, List<LessonStatus> lessonStatuses, List<TimetableUnit> timetableUnits)
        {
            var suitableInstitutions = institutions
                .Where(i =>
                    classrooms.Any(c => c.InstitutionId == i.Id) &&
                    courses.Any(c => c.InstitutionId == i.Id) &&
                    lessonStatuses.Any(s => s.InstitutionId == i.Id) &&
                    timetableUnits.Any(u => u.InstitutionId == i.Id))
                .ToList();

            if (suitableInstitutions.Count == 0)
                throw new InvalidOperationException("There are no associations between classrooms, courses, lesson statuses and timetable units.");

            _faker
                .Rules((faker, lesson) =>
                {
                    foreach (var institution in institutions.OrderBy(_ => Guid.NewGuid()))
                    {
                        int institutionId = institution.Id;

                        lesson.LessonStatus = PickAssociatedEntity(
                            faker,
                            institutionId,
                            lessonStatuses,
                            _institutionLessonStatuses,
                            s => s.InstitutionId == institutionId
                        );

                        var suitableTimetableUnits = _institutionTimetableUnits.ContainsKey(institutionId) 
                            ? _institutionTimetableUnits[institutionId]
                            : timetableUnits.Where(u => u.InstitutionId == institutionId);

                        var suitableCourses = _institutionCourses.ContainsKey(institutionId)
                            ? _institutionCourses[institutionId]
                            : courses.Where(c => c.InstitutionId == institutionId);

                        var suitableClassrooms = _institutionClassrooms.ContainsKey(institutionId)
                            ? _institutionClassrooms[institutionId]
                            : classrooms.Where(c => c.InstitutionId == institutionId);

                        try
                        {
                            (int courseId, int classroomId, int timetableUnitId, DateTimeOffset date) = 
                                FindPossibleCombinationOfDependencies(
                                    suitableCourses.OrderBy(_ => Guid.NewGuid()), 
                                    suitableTimetableUnits.OrderBy(_ => Guid.NewGuid()), 
                                    suitableClassrooms.OrderBy(_ => Guid.NewGuid())
                                );

                            lesson.CourseId = courseId;
                            lesson.ClassroomId = classroomId;
                            lesson.TimetableUnitId = timetableUnitId;
                            lesson.Date = date;

                            return;
                        } catch (ArgumentException)
                        {
                            continue;
                        }
                    }

                    throw new ArgumentException("It is impossible to find combination for all dependencies: classroom, course, timetableUnit and date");
                })
                .RuleFor(l => l.Description, GenerateValidDescription)
                .RuleFor(l => l.HomeworkDescription, GenerateValidHomeworkDescription);
        }

        private (int CourseId, int ClassroomId, int TimetableUnitId, DateTimeOffset Date) FindPossibleCombinationOfDependencies(IEnumerable<Course> suitableCourses, IEnumerable<TimetableUnit> suitableTimetableUnits, IEnumerable<Classroom> classrooms)
        {
            foreach (var timetableUnit in suitableTimetableUnits)
            {
                foreach (var course in suitableCourses)
                {
                    DateTimeOffset currentDate = course.Term.StartDate;
                    DateTimeOffset endDate = course.Term.EndDate;

                    while (currentDate <= endDate)
                    {
                        var teacherActivityInfo = (course.TeacherId, timetableUnit.Id, currentDate);
                        if (!_usedTeachers.Contains(teacherActivityInfo))
                        {
                            foreach (var classroom in classrooms)
                            {
                                var classroomUsageInfo = (classroom.Id, timetableUnit.Id, currentDate);

                                if (!_usedClassrooms.Contains(classroomUsageInfo))
                                {
                                    _usedTeachers.Add(teacherActivityInfo);
                                    _usedClassrooms.Add(classroomUsageInfo);

                                    return (course.Id, classroom.Id, timetableUnit.Id, currentDate);
                                }
                            }
                        }

                        currentDate = currentDate.AddDays(1);
                    }
                }
            }

            throw new ArgumentException("Unable to find combination.");
        }

        private string GenerateValidDescription(Faker faker)
        {
            string description = $"{faker.Commerce.ProductAdjective()} {faker.Company.CatchPhrase()}. {faker.Lorem.Sentence()}";

            int maxLength = RegexPatterns.Patterns[RegexPatterns.Pattern.Description].MaxLength;

            return TruncateToMaxLength(description, maxLength);
        }

        private string GenerateValidHomeworkDescription(Faker faker)
        {
            string description = faker.Lorem.Paragraphs(1, 2);

            int maxLength = RegexPatterns.Patterns[RegexPatterns.Pattern.Description].MaxLength;

            return TruncateToMaxLength(description, maxLength);
        }
    }
}
