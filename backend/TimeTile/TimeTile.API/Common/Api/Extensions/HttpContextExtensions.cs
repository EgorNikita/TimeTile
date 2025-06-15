using TimeTile.API.Common.Constants;

namespace TimeTile.API.Common.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static int GetInstitutionId(this HttpContext context)
        {
            return (int) context.Items[HttpContextItemKeys.InstitutionId]!;
        }
    }
}
