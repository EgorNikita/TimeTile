namespace TimeTile.API.Common.Api.Requests
{
    public interface IFilterByUsersRequest
    {
        int[]? UserIds { get; }
    }
}
