using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class PermissionFaker : BaseFaker<Permission>
    {
        // Description constraints
        private const string DESCRIPTION_REGEX = @"^[\w '.-]+$";
        private const int DESCRIPTION_MAX_LENGTH = 255;

        private int _currentId = 0;

        public PermissionFaker()
        {
            _faker
                .RuleFor(p => p.Description, f => Permissions.All.ElementAt((++_currentId - 1) % Permissions.Count));
        }

        public List<Permission> Generate()
        {
            return _faker.Generate(Permissions.Count);
        }

        public override List<Permission> Generate(int count)
        {
            throw new InvalidOperationException("There are certain amount of possible Permissions. Call Generate() without parameters.");
        }
    }
}
