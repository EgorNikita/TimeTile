using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class RoleToPermissionFaker : BaseFaker<RoleToPermission>
    {
        private readonly HashSet<(int RoleId, int PermissionId)> _fkPairs = new();

        private readonly int _possibleCombinations;

        public RoleToPermissionFaker(List<Role> roles, List<Permission> permissions)
        {
            _possibleCombinations = roles.Count * permissions.Count;

            _faker
                .Rules((f, rp) =>
                {
                    int roleId;
                    int permissionId;

                    do
                    {
                        roleId = f.PickRandom(roles).Id;
                        permissionId = f.PickRandom(permissions).Id;
                    } while (_fkPairs.Contains((roleId, permissionId)));

                    _fkPairs.Add((roleId, permissionId));

                    rp.RoleId = roleId;
                    rp.PermissionId = permissionId;
                });
        }

        public override List<RoleToPermission> Generate(int count)
        {
            if (count > _possibleCombinations)
            {
                return base.Generate(_possibleCombinations);
            }

            return base.Generate(count);
        }
    }
}
