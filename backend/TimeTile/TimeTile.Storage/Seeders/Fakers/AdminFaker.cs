using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class AdminFaker : BaseFaker<User>
    {
        // For saving passwords
        private const string LOGIN_DATA_FILE_NAME = "admins_login_data.txt";

        private readonly UserFaker _userFaker;

        // BirthDate constraints
        private const int MIN_AGE = 20;
        private const int MAX_AGE = 80;

        public AdminFaker(Role adminRole, IUserService userService, IFileService fileService)
        {
            _userFaker = new UserFaker(userService, fileService);

            _faker
                .RuleFor(s => s.BirthDate, f => UserFaker.GenerateValidBirthDate(f, MIN_AGE, MAX_AGE))
                .RuleFor(s => s.Firstname, _userFaker.GenerateValidFirstname)
                .RuleFor(s => s.Lastname, _userFaker.GenerateValidLastname)
                .RuleFor(s => s.HomeAddress, _userFaker.GenerateValidAddress)
                .RuleFor(s => s.Login, _userFaker.GenerateValidLogin)
                .RuleFor(s => s.PasswordHash, _userFaker.GenerateValidPassword)
                .RuleFor(s => s.PhoneNumber, _userFaker.GenerateValidPhoneNumber)
                .RuleFor(s => s.RoleId, adminRole.Id);
        }

        public override List<User> Generate(int count)
        {
            var admins = base.Generate(count);

            Task.Run(async () => 
                await _userFaker.GenerateAvatars(admins, CancellationToken.None)).Wait();

            System.IO.File.AppendAllText(FormFullPath(LOGIN_DATA_FILE_NAME), _userFaker.LoginDataFormatted);

            return admins;
        }

        public async Task<List<User>> GenerateAsync(int count, CancellationToken cancellationToken)
        {
            var admins = base.Generate(count);

            await _userFaker.GenerateAvatars(admins, cancellationToken);

            await System.IO.File.AppendAllTextAsync(FormFullPath(LOGIN_DATA_FILE_NAME), _userFaker.LoginDataFormatted, cancellationToken);

            return admins;
        }
    }
}
