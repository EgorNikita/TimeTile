using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Core.Common.Interfaces.Services
{
    public interface IMessageNotificationService
    {
        Task NotifyMessageCreated(Message message, CancellationToken cancellationToken = default);
        Task NotifyMessageEdited(Message message, CancellationToken cancellationToken = default);
    }
}
