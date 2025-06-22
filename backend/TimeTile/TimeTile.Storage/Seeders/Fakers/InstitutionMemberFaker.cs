using Bogus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class InstitutionMemberFaker : BaseFaker<InstitutionMember>
    {
        // For saving passwords
        private const string LOGIN_DATA_FILE_NAME = "institution_members_login_data.txt";

        // WeekWorkHours constraints
        private const int MIN_WEEK_WORK_HOURS = 10;
        private const int MAX_WEEK_WORK_HOURS = 40;

        // BirthDate constraints
        private const int MIN_AGE = 20;
        private const int MAX_AGE = 80;

        private readonly UserFaker _userFaker;

        // Cashing for optimization
        private readonly Dictionary<int, List<Role>> _institutionRoles = new();
        private readonly Dictionary<int, List<Classroom>> _institutionClassrooms = new();

        public InstitutionMemberFaker(List<Role> roles, List<Institution> institutions, List<Classroom> classrooms, IUserService userService, IFileService fileService)
        {
            var suitableInstitutions = institutions
                .Where(i =>
                    roles.Any(r => r.InstitutionId == i.Id) &&
                    classrooms.Any(c => c.InstitutionId == i.Id))
                .ToList();

            if (suitableInstitutions.Count == 0)
                throw new InvalidOperationException("There are no associations between institutions, roles and classrooms.");

            _userFaker = new UserFaker(userService, fileService);

            _faker
                .RuleFor(m => m.BirthDate, f => UserFaker.GenerateValidBirthDate(f, MIN_AGE, MAX_AGE))
                .RuleFor(m => m.Firstname, _userFaker.GenerateValidFirstname)
                .RuleFor(m => m.Lastname, _userFaker.GenerateValidLastname)
                .RuleFor(m => m.HomeAddress, _userFaker.GenerateValidAddress)
                .RuleFor(m => m.Login, _userFaker.GenerateValidLogin)
                .RuleFor(m => m.PasswordHash, _userFaker.GenerateValidPassword)
                .RuleFor(m => m.PhoneNumber, _userFaker.GenerateValidPhoneNumber)
                .RuleFor(m => m.WeekWorkHours, f => f.Random.Int(MIN_WEEK_WORK_HOURS, MAX_WEEK_WORK_HOURS))
                .Rules((faker, member) =>
                {
                    member.InstitutionId = faker.PickRandom(suitableInstitutions).Id;

                    member.Role = PickAssociatedEntity(
                        faker,
                        (int)member.InstitutionId,
                        roles,
                        _institutionRoles,
                        r => r.InstitutionId == member.InstitutionId
                    );

                    member.PreferredClassroomId = PickAssociatedEntity(
                        faker,
                        (int)member.InstitutionId,
                        classrooms,
                        _institutionClassrooms,
                        c => c.InstitutionId == member.InstitutionId
                    ).Id;
                });
        }

        public override List<InstitutionMember> Generate(int count)
        {
            var institutionMembers = base.Generate(count);

            Task.Run(async () =>
                await _userFaker.GenerateAvatars(institutionMembers.Cast<User>().ToList(), CancellationToken.None)).Wait();

            System.IO.File.AppendAllText(FormFullPath(LOGIN_DATA_FILE_NAME), _userFaker.LoginDataFormatted);

            return institutionMembers;
        }

        public async Task<List<InstitutionMember>> GenerateAsync(int count, CancellationToken cancellationToken)
        {
            var institutionMembers = base.Generate(count);

            await _userFaker.GenerateAvatars(institutionMembers.Cast<User>().ToList(), cancellationToken);

            await System.IO.File.AppendAllTextAsync(
                FormFullPath(LOGIN_DATA_FILE_NAME), 
                FormatLoginDataContent(institutionMembers), 
                cancellationToken
            );

            return institutionMembers;
        }

        private string FormatLoginDataContent(List<InstitutionMember> institutionMembers)
        {
            var dictionary = institutionMembers.ToDictionary(x => x.Login);

            return string.Join(Environment.NewLine, _userFaker.LoginData.Select(loginData =>
            {
                var institutionMember = dictionary[loginData.Login];

                var loginDataFormatted = $"Login: {loginData.Login}; Password: {loginData.Password};";
                var permissions = string.Join(Environment.NewLine, institutionMember.Role.Permissions.Select(p => '\t' + p.Description));

                return loginDataFormatted + '\n' +
                    "Permissions:" + '\n' +
                    permissions + '\n';
            }));
        }
    }
}
