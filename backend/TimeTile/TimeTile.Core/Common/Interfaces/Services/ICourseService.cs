using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Core.Common.Interfaces.Services
{
    public interface ICourseService
    {
        string GetIconUrl(Course course);
        Task<int> SaveIcon(IFormFile? icon, string title, CancellationToken cancellationToken);
        Task<Stream> GenerateDefaultIcon(string title);
    }
}
