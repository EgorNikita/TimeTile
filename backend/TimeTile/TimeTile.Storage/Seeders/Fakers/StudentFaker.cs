using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class StudentFaker : BaseFaker<Student>
    {
        // BirthDate constraints
        private const int MIN_AGE = 6;
        private const int MAX_AGE = 25;

        private readonly UserFaker _userFaker = new UserFaker();

        // Caching for optimization
        private readonly Dictionary<int, List<Group>> _institutionGroups = new();

        public StudentFaker(Role studentRole, List<Institution> institutions, List<Group> groups)
        {
            var suitableInstitutions = institutions
                .Where(i => groups.Any(g => g.InstitutionId == i.Id));

            if (!suitableInstitutions.Any())
                throw new InvalidOperationException("There are no associations between institutions and groups.");

            _faker
                .RuleFor(s => s.AvatarPath, f => "undefined")           // TODO: add avatars
                .RuleFor(s => s.BirthDate, f => _userFaker.GenerateValidBirthDate(f, MIN_AGE, MAX_AGE))
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
    }
}
