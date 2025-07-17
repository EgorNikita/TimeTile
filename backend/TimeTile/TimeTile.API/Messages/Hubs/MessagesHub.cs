using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TimeTile.API.Common.Api.Hubs;
using TimeTile.API.Messages.Hubs.Validators;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Hubs
{
    public class MessagesHub : AuthenticatedHub
    {
        public const string HUB_PATH = "/messages-hub";

        private readonly IMessageNotificationService _messageNotificationService;
        private readonly IMessagesHubValidator _messagesHubValidator;

        public MessagesHub(TimetileDbContext db, IMessageNotificationService messageNotificationService, IMessagesHubValidator messagesHubValidator)
            : base(db)
        {
            _messageNotificationService = messageNotificationService;
            _messagesHubValidator = messagesHubValidator;
        }

        public static string GetGroupName(int courseId)
        {
            return $"course_{courseId}";
        }

        public async Task SendMessage(int courseId, string content)
        {
            var cancellationToken = Context.ConnectionAborted;

            await _messagesHubValidator.ValidateCourseIdByInstitution(courseId, InstitutionId, cancellationToken);
            await _messagesHubValidator.ValidateCourseIdByUser(courseId, UserId, cancellationToken);
            await _messagesHubValidator.ValidateContent(content);

            var message = new Message
            {
                UserId = UserId,
                CourseId = courseId,
                Content = content,
                SentAt = DateTimeOffset.UtcNow
            };

            await _db.Messages.AddAsync(message, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _messageNotificationService.NotifyMessageCreated(message, cancellationToken);
        }

        public async Task EditMessage(int messageId, string content)
        {
            var cancellationToken = Context.ConnectionAborted;

            await _messagesHubValidator.ValidateMessageId(messageId, UserId, InstitutionId, cancellationToken);
            await _messagesHubValidator.ValidateContent(content);

            var message = await _db.Messages
                .FirstAsync(m => m.Id == messageId, cancellationToken);

            var trimmedContent = content.Trim();

            if (message.Content != trimmedContent)
            {
                message.Content = trimmedContent;
                message.EditedAt = DateTimeOffset.UtcNow;
            }

            await _db.SaveChangesAsync(cancellationToken);

            await _messageNotificationService.NotifyMessageEdited(message, cancellationToken);
        }

        [Authorize(Permissions.Courses.AddStudent)]
        public async Task JoinGroupAsStudent(int courseId, int studentId)
        {
            var cancellationToken = Context.ConnectionAborted;

            await _messagesHubValidator.ValidateStudentId(studentId, InstitutionId, cancellationToken);
            await JoinGroup(courseId, cancellationToken);
        }

        [Authorize(Permissions.Courses.UpdateTeacher)]
        public async Task JoinGroupAsTeacher(int courseId, int teacherId)
        {
            var cancellationToken = Context.ConnectionAborted;

            await _messagesHubValidator.ValidateTeacherId(teacherId, InstitutionId, cancellationToken);
            await JoinGroup(courseId, cancellationToken);
        }

        private async Task JoinGroup(int courseId, CancellationToken cancellationToken = default)
        {
            await _messagesHubValidator.ValidateCourseIdByInstitution(courseId, InstitutionId, cancellationToken);

            var groupName = GetGroupName(courseId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName, cancellationToken);
        }

        [Authorize(Permissions.Courses.RemoveStudent)]
        public async Task LeaveGroupAsStudent(int courseId, int studentId)
        {
            var cancellationToken = Context.ConnectionAborted;

            await _messagesHubValidator.ValidateStudentId(studentId, InstitutionId, cancellationToken);
            await LeaveGroup(courseId, cancellationToken);
        }

        [Authorize(Permissions.Courses.UpdateTeacher)]
        public async Task LeaveGroupAsTeacher(int courseId, int teacherId)
        {
            var cancellationToken = Context.ConnectionAborted;

            await _messagesHubValidator.ValidateTeacherId(teacherId, InstitutionId, cancellationToken);
            await LeaveGroup(courseId, cancellationToken);
        }

        private async Task LeaveGroup(int courseId, CancellationToken cancellationToken = default)
        {
            await _messagesHubValidator.ValidateCourseIdByInstitution(courseId, InstitutionId, cancellationToken);

            var groupName = GetGroupName(courseId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName, cancellationToken);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var userId = UserId;

            // Join all groups of the user
            var courseIds = await _db.Courses
                .Where(c => c.TeacherId == userId ||
                    c.CoursesToStudents.Any(cs => cs.StudentId == userId))
                .Select(c => c.Id)
                .ToListAsync(Context.ConnectionAborted);

            var tasks = courseIds.Select(async courseId =>
            {
                var groupName = GetGroupName(courseId);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName, Context.ConnectionAborted);
            });

            await Task.WhenAll(tasks);
        }
    }
}
