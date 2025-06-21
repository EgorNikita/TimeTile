using Microsoft.AspNetCore.Http;
using TimeTile.Core.Models;

namespace TimeTile.Core.Common.Interfaces.Services;

public interface IUserService
{
    Task<int> SaveAvatar(Stream avatarStream, string firstname, string lastname, DateOnly birthday, CancellationToken cancellationToken);
    string GetAvatarUrl(User user);
    Task<Stream> GenerateDefaultAvatar(string firstname, string lastname);
    Task<T> CreateUser<T>(IFormFile? avatar, string firstname, string lastname, string homeAddress, string phoneNumber, DateOnly birthDate, int institutionId, int roleId, Action<T> configureSpecificProperties, CancellationToken cancellationToken) where T : User, new();
}