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
    internal class LessonToStudentFaker
    {
        // CameAt constraints
        private const float CAME_AT_TIME_POSSIBILITY = 0.65f;
        private const float CAME_AT_PRESENCE_POSSIBILITY = 0.8f;

        // LeftAt constraints
        private const float STAYED_TILL_THE_END = 0.95f;

        // ClassworkGrade constraints
        private const float CLASSWORK_GRADE_PRESENCE_POSSIBILITY = 1f;

        // Extra fakers
        private readonly GradeFaker _gradeFaker = new GradeFaker(GradeType.Classwork);
        private readonly Faker _faker = new Faker();

        public DateTimeOffset? GenerateValidCameAt(Lesson lesson)
        {
            if (IsLessonStartFromFuture(lesson))
            {
                return null;
            }

            if (_faker.Random.Bool(CAME_AT_PRESENCE_POSSIBILITY))
            {
                DateTimeOffset startTime = lesson.StartTime;
                DateTimeOffset endTime = lesson.EndTime;

                if (_faker.Random.Bool(CAME_AT_TIME_POSSIBILITY))
                {
                    return startTime;
                }

                return _faker.Date.BetweenOffset(startTime.AddMinutes(1), endTime);
            }

            return null;
        }

        public DateTimeOffset? GenerateValidLeftAt(LessonToStudent lessonToStudent)
        {
            if (IsLessondEndFromFuture(lessonToStudent.Lesson))
            {
                return null;
            }

            if (lessonToStudent.CameAt is not null)
            {
                DateTimeOffset endTime = lessonToStudent.Lesson.EndTime;

                if (_faker.Random.Bool(STAYED_TILL_THE_END))
                {
                    return endTime;
                }

                if (lessonToStudent.CameAt >= endTime.AddMinutes(-2))
                    return endTime;

                return _faker.Date.BetweenOffset(((DateTimeOffset)lessonToStudent.CameAt).AddMinutes(1), endTime.AddMinutes(-1));
            }

            return null;
        }

        public Grade? GenerateValidGrade(LessonToStudent lessonToStudent)
        {
            if (IsLessondEndFromFuture(lessonToStudent.Lesson) 
                || lessonToStudent.CameAt is null)
            {
                return null;
            }

            if (_faker.Random.Bool(CLASSWORK_GRADE_PRESENCE_POSSIBILITY))
            {
                return _gradeFaker.Generate(1).First();
            }

            return null;
        }

        private bool IsLessondEndFromFuture(Lesson lesson)
        {
            return IsFromFuture(
                lesson.Date,
                lesson.EndTime
            );
        }

        private bool IsLessonStartFromFuture(Lesson lesson)
        {
            return IsFromFuture(
                lesson.Date,
                lesson.StartTime
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
    }
}
