using TimeTile.API.Authentication.Endpoints;
using TimeTile.API.Common.Api;
using TimeTile.API.Roles.Endpoints.CreateRole;
using TimeTile.API.Students.Endpoints;

namespace TimeTile.API;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapAuthenticationEndpoints();
        app.MapStudentEndpoints();
        app.MapRolesEndpoints();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Authentication");

        endpoints.MapPublicGroup()
            .MapEndpoint<Login>();
    }

    private static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/students")
            .WithTags("Students");

        endpoints.MapEndpoint<CreateStudent>()
            .RequireAuthorization("CreateStudent");
    }
    
    private static void MapRolesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/roles")
            .WithTags("Roles");

        endpoints.MapEndpoint<CreateRoleEndpoint>();
        //.RequireAuthorization("CreateRole");
    }
    
    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }
    
    private static IEndpointConventionBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        var mapMethod = typeof(TEndpoint).GetMethod("Map", new[] { typeof(IEndpointRouteBuilder) });
        if (mapMethod == null)
            throw new InvalidOperationException($"Type {typeof(TEndpoint).Name} must have a static Map method with IEndpointRouteBuilder parameter.");

        var result = mapMethod.Invoke(null, new object[] { app });
        return result as IEndpointConventionBuilder ?? throw new InvalidOperationException("Map method must return IEndpointConventionBuilder");
    }
    
}