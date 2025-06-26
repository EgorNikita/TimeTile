using TimeTile.API.Common.Constants;

namespace TimeTile.API.Common.Api.Http
{
    public static class HttpContextExtensions
    {
        public static int GetInstitutionId(this HttpContext context)
        {
            return (int)context.Items[HttpContextItemKeys.InstitutionId]!;
        }
        
        public static int GetUserId(this HttpContext context)
        {
            return (int)context.Items[HttpContextItemKeys.UserId]!;
        }
        
        public static string? GetUserIp(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString();
        }
    }
}
