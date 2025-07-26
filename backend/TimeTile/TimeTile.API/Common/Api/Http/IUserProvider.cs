using TimeTile.Core.Models;

namespace TimeTile.API.Common.Api.Http;

public interface IUserProvider
{
    User GetUser();
}