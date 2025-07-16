using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Hubs;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Hubs
{
    public class MessagesHub : AuthenticatedHub
    {
        public const string HUB_PATH = "/messages-hub";

        public MessagesHub(TimetileDbContext db) 
            : base(db)
        { }

        public static string GetGroupName(int courseId)
        {
            return $"course_{courseId}";
        }

        public async Task JoinGroup(int courseId)
        {
            await ValidateCourseId(courseId);

            var groupName = GetGroupName(courseId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(int courseId)
        {
            await ValidateCourseId(courseId);

            var groupName = GetGroupName(courseId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        private async Task ValidateCourseId(int courseId)
        {
            var userId = UserId;

            var courseExists = await _db.Courses
                .AnyAsync(c => c.Id == courseId);

            if (!courseExists)
                throw new HubException("CourseId is invalid.");

            var hasAccess = await _db.Courses
                .AnyAsync(c => c.Id == courseId &&
                    (c.TeacherId == userId || c.CoursesToStudents.Any(cs => cs.StudentId == userId)));

            if (!hasAccess)
                throw new HubException($"User with id '{userId}' does not have access to this course.");
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var userId = UserId;

            // Join all groups of the user
            var courses = await _db.Courses
                .Where(c => c.TeacherId == userId ||
                    c.CoursesToStudents.Any(cs => cs.StudentId == userId))
                .ToListAsync(Context.ConnectionAborted);

            foreach (var course in courses)
            {
                var groupName = GetGroupName(course.Id);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName, Context.ConnectionAborted);
            }
        }
    }
}
