namespace TimeTile.API.Users.Services.Interfaces;

public interface IUserService
{
    Task<string> GenerateUniqueLoginAsync(string firstname, string lastname, int birthYear, string institutionDomain);
    Task<string> GenerateDefaultPassword(string firstname, string lastname, short birthYear);
    Task<Stream> GenerateDefaultAvatar(string firstname, string lastname);
}