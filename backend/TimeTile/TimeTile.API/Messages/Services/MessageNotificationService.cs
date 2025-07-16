using Microsoft.AspNetCore.SignalR;
using TimeTile.API.Messages.Hubs;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Services
{
    public class MessageNotificationService : IMessageNotificationService
    {
        private const string MESSAGE_RECEIVED_EVENT = "MessageReceived";
        private const string MESSAGE_EDITED_EVENT = "MessageEdited";

        private readonly IHubContext<MessagesHub> _hubContext;
        private readonly TimetileDbContext _db;

        public MessageNotificationService(IHubContext<MessagesHub> hubContext, TimetileDbContext db)
        {
            _hubContext = hubContext;
            _db = db;
        }

        public async Task NotifyMessageCreated(Message message, CancellationToken cancellationToken = default)
        {
            var response = MapToResponse(message);
            var groupName = MessagesHub.GetGroupName(response.CourseId);

            await _hubContext.Clients
                .Group(groupName)
                .SendAsync(MESSAGE_RECEIVED_EVENT, response, cancellationToken);
        }

        public async Task NotifyMessageEdited(Message message, CancellationToken cancellationToken = default)
        {
            var response = MapToResponse(message);
            var groupName = MessagesHub.GetGroupName(response.CourseId);

            await _hubContext.Clients
                .Group(groupName)
                .SendAsync(MESSAGE_EDITED_EVENT, response, cancellationToken);
        }

        private Response MapToResponse(Message message)
        {
            return new Response(
                message.Id,
                message.UserId,
                message.CourseId,
                message.Content,
                message.SentAt,
                message.EditedAt,
                _db.MessagesFiles
                    .Where(mf => mf.MessageId == message.Id)
                    .Select(mf => mf.File.FileGuid.ToString())
                    .ToArray()
            );
        }

        public sealed record Response(
            int Id,
            int UserId,
            int CourseId,
            string? Content,
            DateTimeOffset SentAt,
            DateTimeOffset? EditedAt,
            string[] FileUrls
        );
    }
}
