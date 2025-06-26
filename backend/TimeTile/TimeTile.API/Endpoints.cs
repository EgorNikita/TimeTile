using TimeTile.API.Authentication.Endpoints.Login;
using TimeTile.API.Authentication.Endpoints.Logout;
using TimeTile.API.Authentication.Endpoints.LogoutAll;
using TimeTile.API.Authentication.Endpoints.RefreshToken;
using TimeTile.API.Classrooms.Endpoints.Create;
using TimeTile.API.Classrooms.Endpoints.Get;
using TimeTile.API.Classrooms.Endpoints.GetById;
using TimeTile.API.ClassroomTypes.Endpoints.Create;
using TimeTile.API.ClassroomTypes.Endpoints.Get;
using TimeTile.API.ClassroomTypes.Endpoints.GetById;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Filters;
using TimeTile.API.Files.Endpoints.GetByUrl;
using TimeTile.API.Institutions.Endpoints.Create;
using TimeTile.API.Institutions.Endpoints.GetById;
using TimeTile.API.Institutions.Endpoints.Get;
using TimeTile.API.LessonStatuses.Endpoints.Create;
using TimeTile.API.LessonStatuses.Endpoints.Get;
using TimeTile.API.LessonStatuses.Endpoints.GetById;
using TimeTile.API.Roles.Endpoints.Create;
using TimeTile.API.Students.Endpoints.Create;
using TimeTile.API.Students.Endpoints.GetById;
using TimeTile.API.Students.Endpoints.Get;
using TimeTile.API.Terms.Endpoints.Create;
using TimeTile.API.Terms.Endpoints.Get;
using TimeTile.API.Terms.Endpoints.GetById;
using TimeTile.API.TimetableUnits.Endpoints.Create;
using TimeTile.API.TimetableUnits.Endpoints.Get;
using TimeTile.API.TimetableUnits.Endpoints.GetById;
using TimeTile.Core.Common.Constants;
using TimeTile.API.Users.Endpoints.GetPermissions;
using TimeTile.API.InstitutionMembers.Endpoints.Create;
using TimeTile.API.InstitutionMembers.Endpoints.Get;
using TimeTile.API.InstitutionMembers.Endpoints.GetById;
using TimeTile.API.InstitutionMembers.Endpoints.UpdateSubjects;
using TimeTile.API.InstitutionMembers.Endpoints.UpdateGroups;
using TimeTile.API.Roles.Endpoints.Get;
using TimeTile.API.Roles.Endpoints.GetById;
using TimeTile.API.Roles.Endpoints.GetPermissions;
using TimeTile.API.Roles.Endpoints.UpdatePermissions;
using TimeTile.API.Subjects.Endpoints.Get;
using TimeTile.API.Subjects.Endpoints.GetById;
using TimeTile.API.Subjects.Endpoints.Create;

namespace TimeTile.API;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapAuthenticationEndpoints();
        app.MapClassroomTypesEndpoints();
        app.MapLessonStatusesEndpoints();
        app.MapTermsEndpoints();
        app.MapTimetableUnitsEndpoints();
        app.MapClassroomsEndpoints();
        app.MapStudentEndpoints();
        app.MapUsersEndpoints();
        app.MapInstitutionMembersEndpoints();
        app.MapRolesEndpoints();
        app.MapInstitutionEndpoints();
        app.MapFilesEndpoints();
        app.MapSubjectsEndpoints();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        const string authTag = "Authentication";
        const string authBasePath = "/auth";
        const string authRateLimit = "auth";

        var publicEndpoints = app.MapGroup(authBasePath)
            .WithTags(authTag)
            .RequireRateLimiting(authRateLimit);

        publicEndpoints.MapPublicGroup()
            .MapEndpoint<Login>()
            .MapEndpoint<RefreshToken>();
        
        var protectedEndpoints = app.MapGroup(authBasePath)
            .WithTags(authTag)
            .RequireUserId()
            .RequireRateLimiting(authRateLimit);

        protectedEndpoints
            .MapEndpoint<Logout>()
            .MapEndpoint<LogoutAll>();
    }

    private static void MapClassroomTypesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/classroom-types")
            .WithTags("ClassroomTypes")
            .RequireInstitution();

        endpoints.MapEndpoint<GetClassroomTypesEndpoint>()
            .MapEndpoint<GetClassroomTypeByIdEndpoint>()
            .MapEndpoint<CreateClassroomTypeEndpoint>();
    }

    private static void MapLessonStatusesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/lesson-statuses")
            .WithTags("LessonStatuses")
            .RequireInstitution();

        endpoints.MapEndpoint<GetLessonStatusesEndpoint>()
            .MapEndpoint<GetLessonStatusByIdEndpoint>()
            .MapEndpoint<CreateLessonStatusEndpoint>();
    }

    private static void MapTermsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/terms")
            .WithTags("Terms")
            .RequireInstitution();

        endpoints.MapEndpoint<GetTermsEndpoint>()
            .MapEndpoint<GetTermByIdEndpoint>()
            .MapEndpoint<CreateTermEndpoint>();
    }

    private static void MapTimetableUnitsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/timetable-units")
            .WithTags("TimetableUnits")
            .RequireInstitution();

        endpoints.MapEndpoint<GetTimetableUnitsEndpoint>()
            .MapEndpoint<GetTimetableUnitByIdEndpoint>()
            .MapEndpoint<CreateTimetableUnitEndpoint>();
    }

    private static void MapClassroomsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/classrooms")
            .WithTags("Classrooms")
            .RequireInstitution();

        endpoints.MapEndpoint<GetClassroomsEndpoint>()
            .MapEndpoint<GetClassroomByIdEndpoint>()
            .MapEndpoint<CreateClassroomEndpoint>();
    }

    private static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/students")
            .WithTags("Students")
            .RequireInstitution();

        endpoints.MapEndpoint<CreateStudentEndpoint>()
            .RequireAuthorization(Permissions.Students.Create);

        endpoints.MapEndpoint<GetStudentByIdEndpoint>()
            .RequireAuthorization(Permissions.Students.Get);

        endpoints.MapEndpoint<GetStudentsEndpoint>()
            .RequireAuthorization(Permissions.Students.Get);
    }

    private static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/users")
            .WithTags("Users")
            .RequireInstitution();

        endpoints.MapEndpoint<GetUserPermissionsEndpoint>();
    }

    private static void MapInstitutionMembersEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/institution-members")
            .WithTags("InstitutionMembers")
            .RequireInstitution();

        endpoints.MapEndpoint<CreateInstitutionMemberEndpoint>();

        endpoints.MapEndpoint<GetInstitutionMembersEndpoint>();

        endpoints.MapEndpoint<GetInstitutionMemberByIdEndpoint>();

        endpoints.MapEndpoint<UpdateTeacherSubjectsEndpoint>();

        endpoints.MapEndpoint<UpdateInstitutionMemberGroupsEndpoint>();
    }

    private static void MapRolesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/roles")
            .WithTags("Roles")
            .RequireInstitution();

        endpoints.MapEndpoint<CreateRoleEndpoint>()
            .RequireAuthorization("CreateRole");

        endpoints.MapEndpoint<GetRolesEndpoint>();

        endpoints.MapEndpoint<GetRoleByIdEndpoint>();

        endpoints.MapEndpoint<GetRolePermissionsEndpoint>();

        endpoints.MapEndpoint<UpdatePermissionsEndpoint>();
    }

    private static void MapInstitutionEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/institutions")
            .WithTags("Institutions");

        endpoints.MapEndpoint<GetInstitutionsEndpoint>()
            .MapEndpoint<GetInstitutionByIdEndpoint>();

        endpoints.MapEndpoint<CreateInstitutionEndpoint>()
            .RequireAuthorization(Permissions.Institutions.Create);
    }

    private static void MapFilesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/files")
            .WithTags("Files");

        endpoints.MapEndpoint<GetFileByUrlEndpoint>();
    }

    private static void MapSubjectsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/subjects")
            .WithTags("Subjects")
            .RequireInstitution();

        endpoints.MapEndpoint<GetSubjectsEndpoint>();

        endpoints.MapEndpoint<GetSubjectByIdEndpoint>();

        endpoints.MapEndpoint<CreateSubjectEndpoint>();
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }

    private static RouteGroupBuilder RequireInstitution(this RouteGroupBuilder group)
    {
        return group.AddEndpointFilter<RequireInstitutionFilter>();
    }

    private static RouteGroupBuilder RequireUserId(this RouteGroupBuilder group)
    {
        return group.AddEndpointFilter<RequireUserIdFilter>();
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

    private static RouteGroupBuilder MapEndpoint<TEndpoint>(this RouteGroupBuilder group)
        where TEndpoint : IEndpoint
    {
        var mapMethod = typeof(TEndpoint).GetMethod("Map", new[] { typeof(IEndpointRouteBuilder) });
        if (mapMethod == null)
            throw new InvalidOperationException(
                $"Type {typeof(TEndpoint).Name} must have a static Map method with IEndpointRouteBuilder parameter.");

        mapMethod.Invoke(null, new object[] { group });
        return group;
    }
}