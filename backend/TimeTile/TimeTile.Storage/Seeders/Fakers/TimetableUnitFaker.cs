using Bogus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class TimetableUnitFaker : BaseFaker<TimetableUnit>
    {
        // Title constraints
        private const int MAX_LESSON_NUMBER = 10;

        // LessonTime's logic
        private const int MIN_UNIT_DURATION_MINUTES = 35;
        private const int MAX_UNIT_DURATION_MINUTES = 60;

        private readonly Dictionary<int, List<KeyValuePair<TimeOnly, TimeOnly>>> _institutionsToLessonTime = new();

        public TimetableUnitFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(u => u.Title, GenerateValidTitle)
                .RuleFor(u => u.InstitutionId, f => f.PickRandom(institutions).Id)
                .RuleFor(u => u.StartTime, (f, u) => GenerateValidStartTime(f, u.InstitutionId))
                .RuleFor(u => u.EndTime, (f, u) => GenerateValidEndTime(f, TimeOnly.FromDateTime(u.StartTime.DateTime), u.InstitutionId));
        }

        private string GenerateValidTitle(Faker faker)
        {
            int lessonNumber = faker.Random.Int(1, MAX_LESSON_NUMBER);
            return MakeUniqueValue($"Lesson {lessonNumber}");
        }

        private DateTimeOffset GenerateValidStartTime(Faker faker, int institutionId)
        {
            if (! _institutionsToLessonTime.ContainsKey(institutionId))
            {
                _institutionsToLessonTime[institutionId] = new List<KeyValuePair<TimeOnly, TimeOnly>>();
            }
            List<KeyValuePair<TimeOnly, TimeOnly>> times = _institutionsToLessonTime[institutionId];

            Func<TimeOnly> generator = () => 
            {
                TimeOnly start = TimeOnly.MinValue;
                TimeOnly end = TimeOnly.MaxValue.AddMinutes(-MAX_UNIT_DURATION_MINUTES);

                return faker.Date.BetweenTimeOnly(start, end);
            };

            Predicate<TimeOnly> predicate = (startTime) =>
                ! times.Any(t => (startTime >= t.Key && startTime <= t.Value)   // not a part of other lesson
                    || (startTime <= t.Key && startTime.AddMinutes(MIN_UNIT_DURATION_MINUTES) >= t.Key));       // not too close to other lesson

            TimeOnly startTime = GenerateValidValue(generator, predicate);

            return ConvertToDefaultDateTimeOffset(startTime);
        }

        private DateTimeOffset GenerateValidEndTime(Faker faker, TimeOnly startTime, int institutionId)
        {
            List<KeyValuePair<TimeOnly, TimeOnly>> times = _institutionsToLessonTime[institutionId];

            Func<TimeOnly> generator = () =>
                startTime.AddMinutes(faker.Random.Int(MIN_UNIT_DURATION_MINUTES, MAX_UNIT_DURATION_MINUTES));

            Predicate<TimeOnly> predicate = (endTime) =>
                ! times.Any(t => t.Key <= endTime && t.Value >= endTime);   // not a part of other lesson

            TimeOnly endTime = GenerateValidValue(generator, predicate);

            times.Add(new(startTime, endTime));

            return ConvertToDefaultDateTimeOffset(endTime);
        }

        private DateTimeOffset ConvertToDefaultDateTimeOffset(TimeOnly time)
        {
            return new DateTimeOffset(
                DateOnly.MinValue,
                time,
                TimeSpan.Zero
            );
        }
    }
}
