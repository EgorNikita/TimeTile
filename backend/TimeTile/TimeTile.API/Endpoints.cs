using TimeTile.API.Authentication.Endpoints;
using TimeTile.API.ClassroomTypes.Endpoints.Create;
using TimeTile.API.ClassroomTypes.Endpoints.Get;
using TimeTile.API.ClassroomTypes.Endpoints.GetById;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Filters;
using TimeTile.API.Institutions.Endpoints.CreateInstitution;
using TimeTile.API.Institutions.Endpoints.GetInstitutionById;
using TimeTile.API.Institutions.Endpoints.GetInstitutions;
using TimeTile.API.Roles.Endpoints.CreateRole;
using TimeTile.API.Students.Endpoints.CreateStudent;
using TimeTile.API.Students.Endpoints.GetStudentById;
using TimeTile.API.Students.Endpoints.GetStudents;
using TimeTile.API.Terms.Endpoints.Create;
using TimeTile.API.Terms.Endpoints.Get;
using TimeTile.API.Terms.Endpoints.GetById;
using TimeTile.Core.Common.Constants;

namespace TimeTile.API;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapAuthenticationEndpoints();
        app.MapClassroomTypesEndpoints();
        app.MapTermsEndpoints();
        app.MapStudentEndpoints();
        app.MapRolesEndpoints();
        app.MapInstitutionEndpoints();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Authentication")
            .RequireRateLimiting("login");

        endpoints.MapPublicGroup()
            .MapEndpoint<Login>();
    }

    private static void MapClassroomTypesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/classroom-types")
            .WithTags("ClassroomTypes")
            .RequireInstitution();

        endpoints.MapEndpoint<GetClassroomTypesEndpoint>();

        endpoints.MapEndpoint<GetClassroomTypeByIdEndpoint>();

        endpoints.MapEndpoint<CreateClassroomTypeEndpoint>();
    }

    private static void MapTermsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/terms")
            .WithTags("Terms")
            .RequireInstitution();

        endpoints.MapEndpoint<GetTermsEndpoint>();

        endpoints.MapEndpoint<GetTermByIdEndpoint>();

        endpoints.MapEndpoint<CreateTermEndpoint>();
    }

    private static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/students")
            .WithTags("Students")
            .RequireInstitution();

        endpoints.MapEndpoint<CreateStudentEndpoint>()
            .RequireAuthorization(Permissions.CreateStudent);

        endpoints.MapEndpoint<GetStudentByIdEndpoint>()
            .RequireAuthorization(Permissions.GetStudents);

        endpoints.MapEndpoint<GetStudentsEndpoint>()
            .RequireAuthorization(Permissions.GetStudents);
    }

    private static void MapRolesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/roles")
            .WithTags("Roles")
            .RequireInstitution();

        endpoints.MapEndpoint<CreateRoleEndpoint>()
            .RequireAuthorization("CreateRole");
    }

    private static void MapInstitutionEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/institutions")
            .WithTags("Institutions");

        endpoints.MapEndpoint<GetInstitutionsEndpoint>();
        endpoints.MapEndpoint<GetInstitutionByIdEndpoint>();

        endpoints.MapEndpoint<CreateInstitutionEndpoint>()
            .RequireAuthorization(Permissions.CreateInstitution);
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }

    public static RouteGroupBuilder RequireInstitution(this RouteGroupBuilder group)
    {
        return group.AddEndpointFilter<RequireInstitutionFilter>();
    }

    private static IEndpointConventionBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        var mapMethod = typeof(TEndpoint).GetMethod("Map", new[] { typeof(IEndpointRouteBuilder) });
        if (mapMethod == null)
            throw new InvalidOperationException(
                $"Type {typeof(TEndpoint).Name} must have a static Map method with IEndpointRouteBuilder parameter.");

        var result = mapMethod.Invoke(null, new object[] { app });
        return result as IEndpointConventionBuilder ??
               throw new InvalidOperationException("Map method must return IEndpointConventionBuilder");
    }
}