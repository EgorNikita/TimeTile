using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;
using static TimeTile.Core.Common.Constants.Permissions;

namespace TimeTile.Storage.Seeders.RealisticFakers
{
    internal class TimetableUnitRealisticFaker
    {
        // Constants for realistic timetable generation
        public readonly static TimeOnly START_TIME = new(8, 0, 0);  // 8:00
        public readonly static TimeOnly END_TIME = new(20, 0, 0);   // 20:00
        public const int LESSON_DURATION_MINUTES = 45;
        public const int BREAK_DURATION_MINUTES = 15;

        public static List<TimetableUnit> Generate(List<Institution> institutions)
        {
            List<TimetableUnit> timetableUnits = new();

            foreach (var institution in institutions)
            {
                var startTime = new DateTimeOffset(DateOnly.MinValue, START_TIME, TimeSpan.Zero);
                var endTime = new DateTimeOffset(DateOnly.MinValue, END_TIME, TimeSpan.Zero);

                var actualTime = startTime;

                var lessonIndex = 1;

                while (actualTime < endTime)
                {
                    timetableUnits.Add(new TimetableUnit
                    {
                        Title = $"Lesson {lessonIndex++}",
                        InstitutionId = institution.Id,
                        StartTime = actualTime,
                        EndTime = actualTime.AddMinutes(LESSON_DURATION_MINUTES)
                    });

                    timetableUnits.Add(new TimetableUnit
                    {
                        Title = $"Lesson {lessonIndex++}",
                        InstitutionId = institution.Id,
                        StartTime = actualTime.AddMinutes(LESSON_DURATION_MINUTES),
                        EndTime = actualTime.AddMinutes(LESSON_DURATION_MINUTES * 2)
                    });

                    var intervalInMinutes = LESSON_DURATION_MINUTES * 2 + BREAK_DURATION_MINUTES;
                    actualTime = actualTime.AddMinutes(intervalInMinutes);
                }
            }

            return timetableUnits;
        }
    }
}
