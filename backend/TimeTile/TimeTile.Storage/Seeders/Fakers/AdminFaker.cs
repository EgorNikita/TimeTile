using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class AdminFaker : BaseFaker<User>
    {
        private readonly UserFaker _userFaker = new UserFaker();

        // BirthDate constraints
        private const int MIN_AGE = 20;
        private const int MAX_AGE = 80;

        public AdminFaker(Role adminRole)
        {
            _faker
                .RuleFor(s => s.AvatarPath, f => "undefined")           // TODO: add avatars
                .RuleFor(s => s.BirthDate, f => _userFaker.GenerateValidBirthDate(f, MIN_AGE, MAX_AGE))
                .RuleFor(s => s.Firstname, _userFaker.GenerateValidFirstname)
                .RuleFor(s => s.Lastname, _userFaker.GenerateValidLastname)
                .RuleFor(s => s.HomeAddress, _userFaker.GenerateValidAddress)
                .RuleFor(s => s.Login, _userFaker.GenerateValidLogin)
                .RuleFor(s => s.Password, _userFaker.GenerateValidPassword)
                .RuleFor(s => s.PhoneNumber, _userFaker.GenerateValidPhoneNumber)
                .RuleFor(s => s.RoleId, adminRole.Id);
        }
    }
}
