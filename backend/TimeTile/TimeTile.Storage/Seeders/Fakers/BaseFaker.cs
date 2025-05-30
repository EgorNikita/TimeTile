using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal abstract class BaseFaker<T>
        where T : class
    {
        private const int MAX_ATTEMPTS_COUNT = 1000;

        protected readonly Faker<T> _faker = new Faker<T>();

        public virtual List<T> Generate(int count)
        {
            return _faker.Generate(count);
        }

        protected string MakeUniqueValue(string baseValue)
        {
            Guid guid = Guid.NewGuid();
            BigInteger bigInt = new BigInteger(guid.ToByteArray());

            string numericString = BigInteger.Abs(bigInt).ToString().PadLeft(39, '0');

            return $"{baseValue} {numericString}";
        }

        protected string TruncateToMaxLength(string baseValue, int maxLength)
        {
            return baseValue.Length <= maxLength ? baseValue : baseValue.Substring(0, maxLength);
        }

        protected string GenerateValidValue(Func<string> valueGenerator, string regex, int maxLength = -1)
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

            throw new InvalidOperationException("Unable to generate a valid value.");
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

            throw new InvalidOperationException("Unable to generate a valid value.");
        }

        protected static T PickAssociatedEntity<T>(
            Faker faker,
            int basisId,
            List<T> allValues,
            Dictionary<int, List<T>> cachedAssociations,
            Predicate<T> predicate)
        {
            List<T> suitableValues;

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
    }
}
