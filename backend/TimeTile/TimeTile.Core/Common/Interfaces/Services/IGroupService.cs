using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Core.Common.Interfaces.Services
{
    public interface IGroupService
    {
        string? GetAvatarUrl(Group group);
        Task<int> SaveAvatar(IFormFile avatar, CancellationToken cancellationToken);
    }
}
