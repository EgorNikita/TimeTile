using Bogus;
using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class TermFaker : BaseFaker<Term>
    {
        private readonly Dictionary<int, List<KeyValuePair<DateOnly, DateOnly>>> _institutionsToTerms = new();

        // Title constraints
        private const string TITLE_REGEX = @"^[\w -.*+,]+$";
        private const int MAX_QUARTER_NUMBER = 12;

        // StartDate's logic
        private const int MAX_QUARTER_DURATION_DAYS = 90;
        private const int MIN_QUARTER_DURATION_DAYS = 30;

        private const int MIN_YEAR = 2024;
        private const int MAX_YEAR = 2025;

        public TermFaker(List<Institution> institutions)
        {
            _faker
                .RuleFor(t => t.InstitutionId, f => f.PickRandom(institutions).Id)
                .Rules((f, t) =>
                {                    
                    int year = f.Random.Int(MIN_YEAR, MAX_YEAR);

                    t.Title = GenerateValidTitle(f, year);
                    t.StartDate = GenerateValidStartDate(f, year, t.InstitutionId);

                    DateOnly startDateOnly = DateOnly.FromDateTime(t.StartDate.DateTime);
                    t.EndDate = GenerateValidEndDate(f, startDateOnly, t.InstitutionId);
                });
        }

        private string GenerateValidTitle(Faker faker, int year)
        {
            Func<string> generator = () =>
            {
                int quarter = faker.Random.Int(1, MAX_QUARTER_NUMBER);

                return MakeUniqueValue($"Quarter {year} {quarter}");
            };

            return GenerateValidValue(generator, TITLE_REGEX);
        }

        private DateTimeOffset GenerateValidStartDate(Faker faker, int year, int institutionId)
        {
            if (!_institutionsToTerms.ContainsKey(institutionId))
            {
                _institutionsToTerms[institutionId] = new List<KeyValuePair<DateOnly, DateOnly>>();
            }
            List<KeyValuePair<DateOnly, DateOnly>> dates = _institutionsToTerms[institutionId];

            Func<DateOnly> generator = () => 
                faker.Date.BetweenDateOnly(new DateOnly(year, 1, 1), new DateOnly(year, 12, 31));

            Predicate<DateOnly> predicate = (startDate) => 
                ! dates.Any(d => (startDate >= d.Key && startDate <= d.Value)     // not a part of other term
                    || (startDate <= d.Key && startDate.AddDays(MIN_QUARTER_DURATION_DAYS) >= d.Key));      // not too close to other term

            DateOnly startDate = GenerateValidValue(generator, predicate);

            return ConvertToDefaultDateTimeOffset(startDate);
        }

        private DateTimeOffset GenerateValidEndDate(Faker faker, DateOnly startDate, int institutionId)
        {
            List<KeyValuePair<DateOnly, DateOnly>> dates = _institutionsToTerms[institutionId];

            Func<DateOnly> generator = () =>
            {
                int quarterDuration = faker.Random.Int(MIN_QUARTER_DURATION_DAYS, MAX_QUARTER_DURATION_DAYS);
                return startDate.AddDays(quarterDuration);
            };

            Predicate<DateOnly> predicate = (endDate) =>
                !dates.Any(d => endDate >= d.Key && endDate <= d.Value);    // not a part of other term

            DateOnly endDate = GenerateValidValue(generator, predicate);

            dates.Add(new(startDate, endDate));

            return ConvertToDefaultDateTimeOffset(endDate);
        }

        private static DateTimeOffset ConvertToDefaultDateTimeOffset(DateOnly date)
        {
            return new DateTimeOffset(
                date,
                TimeOnly.MinValue,
                TimeSpan.Zero
            );
        }
    }
}
