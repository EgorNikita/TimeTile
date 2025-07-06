namespace TimeTile.API.Common.Api.Requests
{
    public interface IFilterByInstitutionMembersRequest
    {
        int[]? InstitutionMemberIds { get; }
    }
}
