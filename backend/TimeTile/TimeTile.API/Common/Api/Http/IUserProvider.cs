using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;

namespace TimeTile.API.Common.Api.Http;

public interface IUserProvider
{
    Result<User> GetUser();
}