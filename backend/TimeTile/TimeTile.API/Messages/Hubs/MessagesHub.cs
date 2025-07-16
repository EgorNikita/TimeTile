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
