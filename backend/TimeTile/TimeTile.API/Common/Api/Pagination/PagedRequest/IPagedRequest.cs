namespace TimeTile.API.Common.Api.Pagination.PagedRequest
{
    public interface IPagedRequest
    {
        public const int MaxPageSize = 100;
        public int? Page { get; }
        public int? PageSize { get; }
    }
}
