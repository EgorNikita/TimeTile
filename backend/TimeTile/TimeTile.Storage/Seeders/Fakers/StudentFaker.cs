using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class StudentFaker : BaseFaker<Student>
    {
        // For saving passwords
        private const string LOGIN_DATA_FILE_NAME = "students_login_data.txt";

        // BirthDate constraints
        private const int MIN_AGE = 6;
        private const int MAX_AGE = 25;

        private readonly UserFaker _userFaker;

        // Caching for optimization
        private static readonly Dictionary<int, List<Group>> _institutionGroups = new();

        public StudentFaker(Role studentRole, List<Institution> institutions, List<Group> groups, IUserService userService, IFileService fileService)
        {
            var suitableInstitutions = institutions
                .Where(i => groups.Any(g => g.InstitutionId == i.Id));

            if (!suitableInstitutions.Any())
                throw new InvalidOperationException("There are no associations between institutions and groups.");

            _userFaker = new UserFaker(userService, fileService);

            _faker
                .RuleFor(s => s.BirthDate, f => UserFaker.GenerateValidBirthDate(f, MIN_AGE, MAX_AGE))
                .RuleFor(s => s.Firstname, _userFaker.GenerateValidFirstname)
                .RuleFor(s => s.Lastname, _userFaker.GenerateValidLastname)
                .RuleFor(s => s.HomeAddress, _userFaker.GenerateValidAddress)
                .RuleFor(s => s.Login, _userFaker.GenerateValidLogin)
                .RuleFor(s => s.PasswordHash, _userFaker.GenerateValidPassword)
                .RuleFor(s => s.PhoneNumber, _userFaker.GenerateValidPhoneNumber)
                .RuleFor(s => s.RoleId, studentRole.Id)
                .Rules((faker, student) =>
                {
                    student.InstitutionId = faker.PickRandom(suitableInstitutions).Id;

                    student.GroupId = PickAssociatedEntity(
                        faker,
                        (int)student.InstitutionId,
                        groups,
                        _institutionGroups,
                        g => g.InstitutionId == student.InstitutionId
                    ).Id;
                });
        }

        public override List<Student> Generate(int count)
        {
            var students = base.Generate(count);

            Task.Run(async () =>
                await _userFaker.GenerateAvatars(students.Cast<User>().ToList(), CancellationToken.None)).Wait();

            System.IO.File.AppendAllText(FormFullPath(LOGIN_DATA_FILE_NAME), _userFaker.LoginDataFormatted);

            return students;
        }

        public async Task<List<Student>> GenerateAsync(int count, CancellationToken cancellationToken)
        {
            var students = base.Generate(count);

            await _userFaker.GenerateAvatars(students.Cast<User>().ToList(), cancellationToken);

            await System.IO.File.AppendAllTextAsync(FormFullPath(LOGIN_DATA_FILE_NAME), _userFaker.LoginDataFormatted, cancellationToken);

            return students;
        }
    }
}
