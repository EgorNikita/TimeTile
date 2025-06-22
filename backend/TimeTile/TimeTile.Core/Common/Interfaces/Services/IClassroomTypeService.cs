using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Core.Common.Interfaces.Services
{
    public interface IClassroomTypeService
    {
        string? GetIconUrl(ClassroomType classroomType);
        Task<int> SaveIcon(IFormFile icon, CancellationToken cancellationToken);
    }
}
