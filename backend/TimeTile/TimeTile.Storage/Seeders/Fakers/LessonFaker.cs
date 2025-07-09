using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class LessonFaker : BaseFaker<Lesson>
    {
        // Assignment constraints
        private const float ASSIGNMENT_PRESENCE_POSSIBILITY = 0.6f;
        private const int DEADLINE_DAYS = 7;
        private const float UPLOAD_AFTER_DEADLINE_POSSIBILITY = 0.8f;

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

                        Result<(int courseId, int classroomId, int timetableUnitId, DateTimeOffset date)> result =
                            FindPossibleCombinationOfDependencies(
                                suitableCourses.OrderBy(_ => Guid.NewGuid()),
                                suitableTimetableUnits.OrderBy(_ => Guid.NewGuid()),
                                suitableClassrooms.OrderBy(_ => Guid.NewGuid())
                            );

                        if (result.IsFailure)
                            continue;

                        var combination = result.Data;

                        lesson.CourseId = combination.courseId;
                        lesson.ClassroomId = combination.classroomId;
                        lesson.TimetableUnitId = combination.timetableUnitId;
                        lesson.Date = combination.date;

                        if (faker.Random.Bool(ASSIGNMENT_PRESENCE_POSSIBILITY))
                        {
                            lesson.Assignment = GenerateValidAssignment(
                                faker,
                                combination.date,
                                timetableUnits.First(u => u.Id == combination.timetableUnitId)
                            );
                        }

                        return;
                    }

                    throw new ArgumentException("It is impossible to find combination for all dependencies: classroom, course, timetableUnit and date");
                })
                .RuleFor(l => l.Description, GenerateValidDescription);
        }

        private Result<(int CourseId, int ClassroomId, int TimetableUnitId, DateTimeOffset Date)> FindPossibleCombinationOfDependencies(IEnumerable<Course> suitableCourses, IEnumerable<TimetableUnit> suitableTimetableUnits, IEnumerable<Classroom> classrooms)
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

                                    return Result.Success((course.Id, classroom.Id, timetableUnit.Id, currentDate));
                                }
                            }
                        }

                        currentDate = currentDate.AddDays(1);
                    }
                }
            }

            var error = Error.From("Unable to find combination.");

            return Result.Failure<(int, int, int, DateTimeOffset)>(error);
        }

        private string GenerateValidDescription(Faker faker)
        {
            string description = $"{faker.Commerce.ProductAdjective()} {faker.Company.CatchPhrase()}. {faker.Lorem.Sentence()}";

            int maxLength = RegexPatterns.Patterns[RegexPatterns.Pattern.Description].MaxLength;

            return TruncateToMaxLength(description, maxLength);
        }

        private Assignment GenerateValidAssignment(Faker faker, DateTimeOffset date, TimetableUnit timetableUnit)
        {
            string title = faker.Lorem.Sentence(3, 5);
            string description = faker.Lorem.Sentences(3);
            DateTimeOffset publishedAt = new DateTimeOffset(date.UtcDateTime.Date + timetableUnit.EndTime.UtcDateTime.TimeOfDay);
            DateTimeOffset deadline = publishedAt.AddDays(DEADLINE_DAYS);
            bool uploadAfterDeadline = faker.Random.Bool(UPLOAD_AFTER_DEADLINE_POSSIBILITY);

            return new Assignment
            {
                Title = title,
                Description = description,
                PublishedAt = publishedAt,
                Deadline = deadline,
                UploadAfterDeadline = uploadAfterDeadline,
            };
        }
    }
}
