
namespace TimeTile.API.Messages.Hubs.Validators
{
    public interface IMessagesHubValidator
    {
        Task ValidateContent(string content);
        Task ValidateCourseIdByInstitution(int courseId, int institutionId, CancellationToken cancellationToken = default);
        Task ValidateCourseIdByUser(int courseId, int userId, CancellationToken cancellationToken = default);
        Task ValidateMessageId(int messageId, int userId, int institutionId, CancellationToken cancellationToken = default);
        Task ValidateStudentId(int studentId, int institutionId, CancellationToken cancellationToken = default);
        Task ValidateTeacherId(int teacherId, int institutionId, CancellationToken cancellationToken = default);
    }
}
