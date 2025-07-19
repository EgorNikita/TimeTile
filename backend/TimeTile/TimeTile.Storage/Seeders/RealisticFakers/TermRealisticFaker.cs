using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.RealisticFakers
{
    internal class TermRealisticFaker
    {
        // Constants for realistic dates generation
        public readonly static DateOnly START_DATE = new(2025, 4, 1);   // 01.05.2025
        public readonly static DateOnly END_DATE = new(2025, 8, 31);   // 31.8.2025

        public static List<Term> Generate(List<Institution> institutions)
        {
            Random random = new Random();

            List<Term> terms = new();

            foreach (var institution in institutions)
            {
                DateTimeOffset startDate = new DateTimeOffset(START_DATE, TimeOnly.MinValue, TimeSpan.Zero);
                DateTimeOffset endDate = new DateTimeOffset(END_DATE, TimeOnly.MinValue, TimeSpan.Zero);
                
                var actualDate = startDate;

                var termIndex = 1;

                var newEndDate = actualDate.AddMonths(2).AddDays(-1); // Assuming each term lasts 2 months

                while (newEndDate < endDate)
                {
                    terms.Add(new Term
                    {
                        Title = $"Term {actualDate.Year} Q{termIndex++}",
                        InstitutionId = institution.Id,
                        StartDate = actualDate,
                        EndDate = newEndDate
                    });

                    var intervalInDays = (newEndDate - actualDate).Days + random.Next(1, 3 + 1) * 7; // Random break between terms of 1 to 3 weeks
                    actualDate = actualDate.AddDays(intervalInDays);

                    newEndDate = actualDate.AddMonths(2).AddDays(-1);
                }
            }

            return terms;
        }
    }
}
