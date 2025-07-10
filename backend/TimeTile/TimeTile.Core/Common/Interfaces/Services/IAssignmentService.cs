using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTile.Core.Common.Interfaces.Services
{
    public interface IAssignmentService
    {
        Task<List<int>> SaveFiles(IFormFileCollection files, CancellationToken cancellationToken);
    }
}
