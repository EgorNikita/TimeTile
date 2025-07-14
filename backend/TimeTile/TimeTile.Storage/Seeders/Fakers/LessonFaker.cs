using Bogus;
using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class LessonFaker : BaseFaker<Lesson>
    {
        // Lesson constraints
        private const float TWO_LESSONS_IN_A_ROW_POSSIBILITY = 0.8f;

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

        // Extra fakers
        private readonly LessonToStudentFaker _lessonToStudentFaker = new();

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

                        Result<(int courseId, int classroomId, int[] timetableUnitIds, DateTimeOffset date)> result =
                            FindPossibleCombinationOfDependencies(
                                faker,
                                suitableCourses.OrderBy(_ => Guid.NewGuid()),
                                suitableTimetableUnits.OrderBy(_ => Guid.NewGuid()),
                                suitableClassrooms.OrderBy(_ => Guid.NewGuid())
                            );

                        if (result.IsFailure)
                            continue;

                        var combination = result.Data;

                        lesson.CourseId = combination.courseId;
                        lesson.ClassroomId = combination.classroomId;
                        lesson.LessonToTimetableUnits = suitableTimetableUnits
                            .Where(u => combination.timetableUnitIds.Contains(u.Id))
                            .Select(timetableUnit => new LessonToTimetableUnit
                            {
                                TimetableUnit = timetableUnit
                            })
                            .ToList();
                        lesson.Date = combination.date;
                        
                        AddAssociationsWithStudents(lesson, suitableCourses, combination.courseId);

                        if (faker.Random.Bool(ASSIGNMENT_PRESENCE_POSSIBILITY))
                        {
                            AddAssignment(faker, lesson, combination.date);
                        }

                        return;
                    }

                    throw new ArgumentException("It is impossible to find combination for all dependencies: classroom, course, timetableUnit and date");
                })
                .RuleFor(l => l.Description, GenerateValidDescription);
        }

        private Result<(int CourseId, int ClassroomId, int[] TimetableUnitIds, DateTimeOffset Date)> FindPossibleCombinationOfDependencies(Faker faker, IEnumerable<Course> suitableCourses, IEnumerable<TimetableUnit> suitableTimetableUnits, IEnumerable<Classroom> classrooms)
        {
            for (int i = 0; i < suitableTimetableUnits.Count(); ++i)
            {
                var timetableUnit = suitableTimetableUnits.ElementAt(i);

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

                                    if (faker.Random.Bool(TWO_LESSONS_IN_A_ROW_POSSIBILITY))
                                    {
                                        var nextTimetableUnit = suitableTimetableUnits.ElementAtOrDefault(i + 1);

                                        if (nextTimetableUnit is not null)
                                        {
                                            // If it is possible to have second lesson with the same data in a row
                                            var nextTeacherActivityInfo = (course.TeacherId, nextTimetableUnit.Id, currentDate);
                                            var nextClassroomUsageInfo = (classroom.Id, nextTimetableUnit.Id, currentDate);

                                            if (!_usedTeachers.Contains(nextTeacherActivityInfo) && !_usedClassrooms.Contains(nextClassroomUsageInfo))
                                            {
                                                _usedTeachers.Add(nextTeacherActivityInfo);
                                                _usedClassrooms.Add(nextClassroomUsageInfo);

                                                return Result.Success((course.Id, classroom.Id, new[] { timetableUnit.Id, nextTimetableUnit.Id }, currentDate));
                                            }
                                        }
                                    }

                                    return Result.Success((course.Id, classroom.Id, new[] { timetableUnit.Id }, currentDate));
                                }
                            }
                        }

                        currentDate = currentDate.AddDays(1);
                    }
                }
            }

            var error = Error.From("Unable to find combination.");

            return Result.Failure<(int, int, int[], DateTimeOffset)>(error);
        }

        private void AddAssociationsWithStudents(Lesson lesson, IEnumerable<Course> courses, int courseId)
        {
            var course = courses.First(c => c.Id == courseId);

            lesson.LessonsToStudents = course.CoursesToStudents.Select(cs =>
            {
                var lessonToStudent = new LessonToStudent
                {
                    Lesson = lesson,
                    StudentId = cs.StudentId
                };

                // If lesson date is in the future, we do not fill CameAt, LeftAt and Grade
                if (lesson.Date > DateTimeOffset.UtcNow)
                {
                    return lessonToStudent;
                }

                lessonToStudent.CameAt = _lessonToStudentFaker.GenerateValidCameAt(lesson);
                lessonToStudent.LeftAt = _lessonToStudentFaker.GenerateValidLeftAt(lessonToStudent);
                lessonToStudent.Grade = _lessonToStudentFaker.GenerateValidGrade(lessonToStudent);

                return lessonToStudent;
            })
            .ToList();
        }

        private void AddAssignment(Faker faker, Lesson lesson, DateTimeOffset date)
        {
            lesson.Assignment = GenerateValidAssignment(
                faker,
                date,
                lesson.EndTime
            );

            // Add Submissions
            lesson.Assignment.Submissions = lesson.LessonsToStudents.Select(ls => new Submission
            {
                StudentId = ls.StudentId,
                Assignment = lesson.Assignment,
                Status = SubmissionStatus.NotSubmitted
            })
            .ToList();
        }

        private string GenerateValidDescription(Faker faker)
        {
            string description = $"{faker.Commerce.ProductAdjective()} {faker.Company.CatchPhrase()}. {faker.Lorem.Sentence()}";

            int maxLength = RegexPatterns.Patterns[RegexPatterns.Pattern.Description].MaxLength;

            return TruncateToMaxLength(description, maxLength);
        }

        private Assignment GenerateValidAssignment(Faker faker, DateTimeOffset date, DateTimeOffset endTime)
        {
            string title = faker.Lorem.Sentence(3, 5);
            string description = faker.Lorem.Sentences(3);
            DateTimeOffset publishedAt = new DateTimeOffset(date.UtcDateTime.Date + endTime.UtcDateTime.TimeOfDay);
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
