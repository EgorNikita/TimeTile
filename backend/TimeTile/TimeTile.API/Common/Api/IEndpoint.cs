namespace TimeTile.API.Common.Api;

public interface IEndpoint
{
    static abstract IEndpointConventionBuilder Map(IEndpointRouteBuilder app);
}