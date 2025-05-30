using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class UserFaker : BaseFaker<User>
    {
        // BirthDate constraints
        private const int MIN_AGE = 6;
        private const int MAX_AGE = 100;

        // Firstname constraints
        private const string FIRSTNAME_REGEX = @"^[a-zA-Z ,.''-]+$";
        private const int FIRSTNAME_MAX_LENGTH = 255;

        // Lastname constraints
        private const string LASTNAME_REGEX = @"^[a-zA-Z ,.''-]+$";
        private const int LASTNAME_MAX_LENGTH = 255;

        // Address constraints
        private const string ADDRESS_REGEX = @"^[A-Za-z\d''\.\- \,]+$";
        private const int ADDRESS_MAX_LENGTH = 255;

        // Login constraints
        private const string LOGIN_REGEX = @"^[\w -]+$";
        private const int LOGIN_MAX_LENGTH = 263;

        // Password constraints
        private const int PASSWORD_MAX_LENGTH = 256;

        // PhoneNumber constraints
        private const string PHONE_NUMBER_REGEX = @"^(\+\d{1,2} )?\(?\d{3}\)?[ .-]\d{3}[ .-]\d{4}$";
        private const int PHONE_NUMBER_MAX_LENGTH = 20;

        public DateOnly GenerateValidBirthDate(Faker faker, int minAge = MIN_AGE, int maxAge = MAX_AGE)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            DateOnly start = today.AddYears(-maxAge);
            DateOnly end = today.AddYears(-minAge);

            return faker.Date.BetweenDateOnly(start, end);
        }

        public string GenerateValidFirstname(Faker faker) =>
            GenerateValidValue(() => faker.Person.FirstName, FIRSTNAME_REGEX, FIRSTNAME_MAX_LENGTH);

        public string GenerateValidLastname(Faker faker) =>
            GenerateValidValue(() => faker.Person.LastName, LASTNAME_REGEX, LASTNAME_MAX_LENGTH);

        public string GenerateValidAddress(Faker faker) =>
            GenerateValidValue(faker.Address.FullAddress, ADDRESS_REGEX, ADDRESS_MAX_LENGTH);

        public string GenerateValidLogin(Faker faker)
        {
            Func<string> generator = () => MakeUniqueValue(faker.Internet.UserName());

            return GenerateValidValue(generator, LOGIN_REGEX, LOGIN_MAX_LENGTH);
        }

        public string GenerateValidPassword(Faker faker) =>
            TruncateToMaxLength(faker.Internet.Password(), PASSWORD_MAX_LENGTH);

        public string GenerateValidPhoneNumber(Faker faker) =>
            GenerateValidValue(() => faker.Phone.PhoneNumber(), PHONE_NUMBER_REGEX, PHONE_NUMBER_MAX_LENGTH);
    }
}