using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class LessonToStudentFaker : BaseFaker<LessonToStudent>
    {
        // CameAt constraints
        private const float CAME_AT_TIME_POSSIBILITY = 0.65f;
        private const float CAME_AT_PRESENCE_POSSIBILITY = 0.8f;

        // LeftAt constraints
        private const float STAYED_TILL_THE_END = 0.95f;

        // ClassworkGrade constraints
        private const float CLASSWORK_GRADE_PRESENCE_POSSIBILITY = 0.8f;

        // Pre generate all possible combinations
        private readonly List<(int LessonId, int StudentId)> _possiblePairs = new();
        private int _actualIndex = 0;

        private readonly GradeFaker _gradeFaker = new GradeFaker(GradeType.Classwork);

        public LessonToStudentFaker(List<Lesson> lessons, List<Student> students)
        {
            foreach (var lesson in lessons)
            {
                foreach (var student in lesson.Course.Students)
                {
                    if (students.Any(s => s.Id == student.Id))
                    {
                        _possiblePairs.Add((lesson.Id, student.Id));
                    }
                }
            }

            _possiblePairs = _possiblePairs.OrderBy(_ => Guid.NewGuid()).ToList();

            _faker
                .Rules((faker, lessonToStudent) =>
                {
                    (int, int) element = _possiblePairs.ElementAt(_actualIndex);

                    lessonToStudent.Lesson = lessons.First(l => l.Id == element.Item1);
                    lessonToStudent.StudentId = element.Item2;

                    _actualIndex++;
                })
                .RuleFor(lts => lts.CameAt, GenerateValidCameAt)
                .RuleFor(lts => lts.LeftAt, GenerateValidLeftAt)
                .RuleFor(lts => lts.Grade, GenerateValidClassworkGrade);
        }

        private DateTimeOffset? GenerateValidCameAt(Faker faker, LessonToStudent lessonToStudent)
        {
            if (IsLessonStartFromFuture(lessonToStudent))
            {
                return null;
            }

            if (faker.Random.Bool(CAME_AT_PRESENCE_POSSIBILITY))
            {
                DateTimeOffset startTime = lessonToStudent.Lesson.TimetableUnit.StartTime;
                DateTimeOffset endTime = lessonToStudent.Lesson.TimetableUnit.EndTime;

                if (faker.Random.Bool(CAME_AT_TIME_POSSIBILITY))
                {
                    return startTime;
                }

                return faker.Date.BetweenOffset(startTime.AddMinutes(1), endTime);
            }

            return null;
        }

        private DateTimeOffset? GenerateValidLeftAt(Faker faker, LessonToStudent lessonToStudent)
        {
            if (IsLessondEndFromFuture(lessonToStudent))
            {
                return null;
            }

            if (lessonToStudent.CameAt is not null)
            {
                DateTimeOffset endTime = lessonToStudent.Lesson.TimetableUnit.EndTime;

                if (faker.Random.Bool(STAYED_TILL_THE_END))
                {
                    return endTime;
                }

                if (lessonToStudent.CameAt >= endTime.AddMinutes(-2))
                    return endTime;

                return faker.Date.BetweenOffset(((DateTimeOffset)lessonToStudent.CameAt).AddMinutes(1), endTime.AddMinutes(-1));
            }

            return null;
        }

        private Grade? GenerateValidClassworkGrade(Faker faker, LessonToStudent lessonToStudent)
        {
            if (IsLessondEndFromFuture(lessonToStudent) 
                || lessonToStudent.CameAt is null)
            {
                return null;
            }

            if (faker.Random.Bool(CLASSWORK_GRADE_PRESENCE_POSSIBILITY))
            {
                return _gradeFaker.Generate(1).First();
            }

            return null;
        }

        private bool IsLessondEndFromFuture(LessonToStudent lessonToStudent)
        {
            return IsFromFuture(
                lessonToStudent.Lesson.Date,
                lessonToStudent.Lesson.TimetableUnit.EndTime
            );
        }

        private bool IsLessonStartFromFuture(LessonToStudent lessonToStudent)
        {
            return IsFromFuture(
                lessonToStudent.Lesson.Date,
                lessonToStudent.Lesson.TimetableUnit.StartTime
            );
        }

        private bool IsFromFuture(DateTimeOffset date, DateTimeOffset time)
        {
            var onlyDate = date.UtcDateTime.Date;
            var onlyTime = time.UtcDateTime.TimeOfDay;

            var currentDate = DateTime.UtcNow.Date;
            var currentTime = DateTime.UtcNow.TimeOfDay;

            return onlyDate > currentDate ||
                (onlyDate == currentDate && onlyTime > currentTime);
        }

        public override List<LessonToStudent> Generate(int count)
        {
            int rest = _possiblePairs.Count - _actualIndex;

            if (count > rest)
            {
                return base.Generate(rest);
            }

            return base.Generate(count);
        }
    }
}
