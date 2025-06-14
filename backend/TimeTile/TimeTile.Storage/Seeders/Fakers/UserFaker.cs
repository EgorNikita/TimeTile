using Bogus;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    public sealed record LoginCredential(
        string Login,
        string Password
    );

    internal class UserFaker : BaseFaker<User>
    {
        // BirthDate constraints
        private const int MIN_AGE = 6;
        private const int MAX_AGE = 100;

        // Password constraints
        private const int PASSWORD_MAX_LENGTH = 256;

        private HashSet<string> _usedLogins = new();

        // For saving passwords
        public List<LoginCredential> LoginData { get; } = new();
        public string LoginDataFormatted => string.Join(Environment.NewLine, LoginData.Select(x => $"Login: {x.Login}; Password: {x.Password};"));

        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public UserFaker(IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
        }

        public static DateOnly GenerateValidBirthDate(Faker faker, int minAge = MIN_AGE, int maxAge = MAX_AGE)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            DateOnly start = today.AddYears(-maxAge);
            DateOnly end = today.AddYears(-minAge);

            return faker.Date.BetweenDateOnly(start, end);
        }

        public string GenerateValidFirstname(Faker faker) =>
            GenerateValidValue(() => faker.Person.FirstName, RegexPatterns.Pattern.Name);

        public string GenerateValidLastname(Faker faker) =>
            GenerateValidValue(() => faker.Person.LastName, RegexPatterns.Pattern.Name);

        public string GenerateValidAddress(Faker faker) =>
            GenerateValidValue(faker.Address.FullAddress, RegexPatterns.Pattern.Address);

        public string GenerateValidLogin(Faker faker)
        {
            Func<string> generator = () => faker.Internet.Email();

            while (true)
            {
                string login = GenerateValidValue(generator, RegexPatterns.Pattern.Email);

                if (!_usedLogins.Contains(login))
                {
                    _usedLogins.Add(login);

                    return login;
                }
            }
        }

        public string GenerateValidPassword(Faker faker, User user)
        {
            var hasher = new PasswordHasher<User>();
            var password = faker.Internet.Password();

            var loginCredential = new LoginCredential(user.Login, password);
            LoginData.Add(loginCredential);

            var hash = hasher.HashPassword(user, password);

            return TruncateToMaxLength(hash, PASSWORD_MAX_LENGTH);
        }

        public string GenerateValidPhoneNumber(Faker faker)
        {
            Func<string> generator = () => $"+1{faker.Phone.PhoneNumber("##########")}";

            return GenerateValidValue(generator, RegexPatterns.Pattern.PhoneE164);
        }

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public Task GenerateAvatars(List<User> users, CancellationToken cancellationToken)
        {
            return Task.WhenAll(users.Select(async user =>
            {
                var avatarStream = await _userService.GenerateDefaultAvatar(user.Firstname, user.Lastname);

                var fileName = $"{user.Firstname}_{user.Lastname}_{user.BirthDate}_avatar.png";

                int savedFileId;

                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    savedFileId = await _fileService.SaveFile(avatarStream, fileName, cancellationToken);
                }
                finally
                {
                    _semaphore.Release();
                }

                user.AvatarId = savedFileId;
            }));
        }
    }
}