using TimeTile.API.Common.Constants;
using TimeTile.Core.Models;

namespace TimeTile.API.Common.Api.Http
{
    public static class HttpContextExtensions
    {
        public static User? GetCurrentUser(this HttpContext context)
        {
            return context.Items[HttpContextItemKeys.CurrentUser]! as User;
        }
    }
}
