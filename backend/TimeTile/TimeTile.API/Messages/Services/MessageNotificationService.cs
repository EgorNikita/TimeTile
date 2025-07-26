using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
            var response = await MapToResponse(message, cancellationToken);
            var groupName = MessagesHub.GetGroupName(response.CourseId);

            await _hubContext.Clients
                .Group(groupName)
                .SendAsync(MESSAGE_RECEIVED_EVENT, response, cancellationToken);
        }

        public async Task NotifyMessageEdited(Message message, CancellationToken cancellationToken = default)
        {
            var response = await MapToResponse(message, cancellationToken);
            var groupName = MessagesHub.GetGroupName(response.CourseId);

            await _hubContext.Clients
                .Group(groupName)
                .SendAsync(MESSAGE_EDITED_EVENT, response, cancellationToken);
        }

        private async Task<Response> MapToResponse(Message message, CancellationToken cancellationToken = default)
        {
            return new Response(
                message.Id,
                message.UserId,
                await _db.Users
                    .Include(u => u.Avatar)
                    .Where(u => u.Id == message.UserId)
                    .Select(u => new UserInfo(
                        u.Id,
                        u.Firstname,
                        u.Lastname,
                        u.HomeAddress,
                        u.PhoneNumber,
                        u.BirthDate,
                        u.Login,
                        u.RoleId,
                        u.Avatar.FileGuid.ToString()
                    )).FirstAsync(cancellationToken),
                message.CourseId,
                message.Content,
                message.SentAt,
                message.EditedAt,
                await _db.MessagesFiles
                    .Where(mf => mf.MessageId == message.Id)
                    .Select(mf => mf.File.FileGuid.ToString())
                    .ToArrayAsync(cancellationToken)
            );
        }

        public sealed record Response(
            int Id,
            int UserId,
            UserInfo User,
            int CourseId,
            string? Content,
            DateTimeOffset SentAt,
            DateTimeOffset? EditedAt,
            string[] FileUrls
        );

        public sealed record UserInfo(
            int Id,
            string Firstname,
            string Lastname,
            string HomeAddress,
            string PhoneNumber,
            DateOnly BirthDate,
            string Login,
            int RoleId,
            string AvatarUrl
        );
    }
}
