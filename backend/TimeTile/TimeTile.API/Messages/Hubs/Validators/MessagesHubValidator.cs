using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Hubs.Validators
{
    public class MessagesHubValidator : IMessagesHubValidator
    {
        private readonly TimetileDbContext _db;

        public MessagesHubValidator(TimetileDbContext db)
        {
            _db = db;
        }

        public async Task ValidateStudentId(int studentId, int institutionId, CancellationToken cancellationToken = default)
        {
            var student = await _db.Students
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);

            if (student is null)
                throw new HubException("StudentId is invalid.");

            if (student.InstitutionId != institutionId)
                throw new HubException("Student does not belong to your institution.");
        }

        public async Task ValidateTeacherId(int teacherId, int institutionId, CancellationToken cancellationToken = default)
        {
            var teacher = await _db.InstitutionMembers
                .FirstOrDefaultAsync(t => t.Id == teacherId, cancellationToken);

            if (teacher is null)
                throw new HubException("TeacherId is invalid.");

            if (teacher.InstitutionId != institutionId)
                throw new HubException("Teacher does not belong to your institution.");
        }

        public async Task ValidateMessageId(int messageId, int userId, int institutionId, CancellationToken cancellationToken = default)
        {
            var messageInfo = await _db.Messages
                .Where(m => m.Id == messageId)
                .Select(m => new { m.UserId, m.Course.InstitutionId })
                .FirstOrDefaultAsync(cancellationToken);

            if (messageInfo == null)
                throw new HubException("MessageId is invalid.");

            if (messageInfo.InstitutionId != institutionId)
                throw new HubException("Message does not belong to your institution.");

            if (messageInfo.UserId != userId)
                throw new HubException($"User with id '{userId}' does not have access to edit this message.");
        }

        public async Task ValidateCourseIdByInstitution(int courseId, int institutionId, CancellationToken cancellationToken = default)
        {
            var course = await _db.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

            if (course is null)
                throw new HubException("CourseId is invalid.");

            if (course.InstitutionId != institutionId)
                throw new HubException("Course does not belong to your institution.");
        }

        public async Task ValidateCourseIdByUser(int courseId, int userId, CancellationToken cancellationToken = default)
        {
            var course = await _db.Courses
                .Include(c => c.CoursesToStudents)
                .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

            if (course is null)
                throw new HubException("CourseId is invalid.");

            var hasAccess = course.TeacherId == userId || course.CoursesToStudents.Any(cs => cs.StudentId == userId);

            if (!hasAccess)
                throw new HubException($"User with id '{userId}' does not have access to this course.");
        }

        public Task ValidateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new HubException("Content cannot be empty.");

            return Task.CompletedTask;
        }
    }
}
