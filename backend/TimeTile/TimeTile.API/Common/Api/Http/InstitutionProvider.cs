namespace TimeTile.API.Common.Api.Http
{
    public class InstitutionProvider : IInstitutionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InstitutionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetInstitutionId()
        {
            var context = _httpContextAccessor.HttpContext;

            return context!.GetCurrentUser()?.InstitutionId
                   ?? throw new InvalidOperationException("Current user is not set in the HTTP context.");
        }
    }
}
