using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal abstract class BaseFaker<T>
        where T : class
    {
        // Handling unexpected cases
        private const int MAX_ATTEMPTS_COUNT = 1000;
        private const string INVALID_GENERATOR_ERROR = "Unable to generate a valid value.";

        protected readonly Faker<T> _faker = new Faker<T>();

        public virtual List<T> Generate(int count)
        {
            return _faker.Generate(count);
        }

        protected string TruncateToMaxLength(string baseValue, int maxLength)
        {
            return baseValue.Length <= maxLength ? baseValue : baseValue.Substring(0, maxLength);
        }

        protected string GenerateValidValue(Func<string> valueGenerator, RegexPatterns.Pattern pattern)
        {
            string regex = RegexPatterns.Patterns[pattern].Pattern.ToString();
            int maxLength = RegexPatterns.Patterns[pattern].MaxLength;

            return GenerateValidValue(valueGenerator, regex, maxLength);
        }

        private string GenerateValidValue(Func<string> valueGenerator, string regex, int maxLength)
        {
            Regex regexObj = new Regex(regex);

            for (int i = 0; i < MAX_ATTEMPTS_COUNT; ++i)
            {
                string value = valueGenerator();

                // Truncate if needed
                if (maxLength > 0)
                {
                    value = TruncateToMaxLength(value, maxLength);
                }

                if (regexObj.IsMatch(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException(INVALID_GENERATOR_ERROR);
        }

        protected static K GenerateValidValue<K>(Func<K> generator, Predicate<K> predicate)
        {
            for (int i = 0; i < MAX_ATTEMPTS_COUNT; ++i)
            {
                K value = generator();

                if (predicate(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException(INVALID_GENERATOR_ERROR);
        }

        protected static K PickAssociatedEntity<K>(
            Faker faker,
            int basisId,
            List<K> allValues,
            Dictionary<int, List<K>> cachedAssociations,
            Predicate<K> predicate)
        {
            List<K> suitableValues;

            if (cachedAssociations.ContainsKey(basisId))
            {
                suitableValues = cachedAssociations[basisId];
            }
            else
            {
                suitableValues = allValues
                    .Where(v => predicate(v))
                    .ToList();

                cachedAssociations.Add(basisId, suitableValues);
            }

            return faker.PickRandom(suitableValues);
        }

        protected string MakeUniqueValue(
            Func<string> generator,
            HashSet<string> usedValues)
        {
            for (int i = 0; i < MAX_ATTEMPTS_COUNT; ++i)
            {
                string value = generator();

                if (!usedValues.Contains(value))
                {
                    usedValues.Add(value);
                    return value;
                }
            }

            throw new InvalidOperationException(INVALID_GENERATOR_ERROR);
        }

        protected static string FormFullPath(string fileName)
        {
            return Path.Combine(DataSeeder.CURRENT_DIRECTORY, fileName);
        }
    }
}
