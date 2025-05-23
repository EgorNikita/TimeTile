using TimeTile.Core.Models;

namespace TimeTile.API.Users;

public interface IUserService
{
    Task<Institution> GetInstitutionId(string institutionDomain);
    Task<string> GenerateLogin(string firstname, string lastname, int birthYear, string institutionDomain);
    Task<string> GenerateDefaultPassword(string firstname, string lastname, short birthYear);
    Task<string> GenerateDefaultAvatar(string firstname, string lastname);
}