using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTile.Core.Common.Interfaces.Services
{
    public interface ISubmissionService
    {
        Task ExpireOverdueSubmissions(CancellationToken cancellationToken = default);
        Task<DateTimeOffset?> GetNextDeadline(CancellationToken cancellationToken = default);
    }
}
