using TimeTile.API.Common.Api.Filters;

namespace TimeTile.API.Common.Api.Extensions
{
    public static class RouteGroupBuilderExtensions
    {
        public static RouteGroupBuilder RequireInstitution(this RouteGroupBuilder group)
        {
            return group.AddEndpointFilter<RequireInstitutionFilter>();
        }
    }
}
