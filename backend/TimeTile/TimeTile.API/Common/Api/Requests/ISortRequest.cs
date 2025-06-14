namespace TimeTile.API.Common.Api.Requests
{
    public interface ISortRequest
    {
        string? SortBy { get; }
        bool Descending { get; }
    }
}
