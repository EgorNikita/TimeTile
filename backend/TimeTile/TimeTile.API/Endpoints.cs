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
using TimeTile.API.Grades.Endpoints.Get;
using TimeTile.API.Grades.Endpoints.GetById;
using TimeTile.API.Groups.Endpoints.Get;
using TimeTile.API.Groups.Endpoints.GetById;
using TimeTile.API.Groups.Endpoints.Create;
using TimeTile.API.Lessons.Endpoints.Get;
using TimeTile.API.Lessons.Endpoints.GetById;
using TimeTile.API.Lessons.Endpoints.Create;
using TimeTile.API.Lessons.Endpoints.Update;
using TimeTile.API.Lessons.Endpoints.UpdateLessonToStudent;
using TimeTile.API.Courses.Endpoints.Get;
using TimeTile.API.Courses.Endpoints.GetById;
using TimeTile.API.Courses.Endpoints.Create;
using TimeTile.API.Courses.Endpoints.UpdateStudents;
using TimeTile.API.Courses.Endpoints.GetStudents;
using TimeTile.API.Students.Endpoints.GetCourses;
using TimeTile.API.Courses.Endpoints.UpdateCourseToStudent;
using TimeTile.API.Groups.Endpoints.UpdateStudents;
using TimeTile.API.Groups.Endpoints.UpdateInstitutionMembers;
using TimeTile.API.Subjects.Endpoints.GetBulk;
using TimeTile.API.Assignments.Endpoints.Get;
using TimeTile.API.Assignments.Endpoints.GetById;
using TimeTile.API.Assignments.Endpoints.GetFiles;
using TimeTile.API.Assignments.Endpoints.Create;
using TimeTile.API.Assignments.Endpoints.Update;
using TimeTile.API.Submissions.Endpoints.Get;
using TimeTile.API.Submissions.Endpoints.GetById;
using TimeTile.API.Submissions.Endpoints.GetFiles;
using TimeTile.API.Students.Endpoints.GetLessons;
using TimeTile.API.Lessons.Endpoints.GetStudents;
using TimeTile.API.Submissions.Endpoints.Submit;
using TimeTile.API.Submissions.Endpoints.Review;
using TimeTile.API.Students.Endpoints.GetAttendanceCount;
using TimeTile.API.Grades.Endpoints.GetBulk;
using TimeTile.API.Messages.Endpoints.Get;
using TimeTile.API.Messages.Endpoints.GetById;
using TimeTile.API.Messages.Endpoints.Create;
using TimeTile.API.Messages.Endpoints.Update;
using TimeTile.API.Submissions.Endpoints.GetBulk;
using TimeTile.API.Assignments.Endpoints.GetBulk;
using TimeTile.API.Courses.Endpoints.GetBulk;
using TimeTile.API.InstitutionMembers.Endpoints.GetBulk;
using TimeTile.API.Classrooms.Endpoints.GetBulk;
using TimeTile.API.LessonStatuses.Endpoints.GetBulk;
using TimeTile.API.Users.Endpoints.GetBulk;

namespace TimeTile.API;

public static class Endpoints
{
    private static class Tags
    {
        public const string Authentication = "Authentication";
        public const string Assignments = "Assignments";
        public const string ClassroomTypes = "ClassroomTypes";
        public const string LessonStatuses = "LessonStatuses";
        public const string Terms = "Terms";
        public const string TimetableUnits = "TimetableUnits";
        public const string Classrooms = "Classrooms";
        public const string Students = "Students";
        public const string Submissions = "Submissions";
        public const string Roles = "Roles";
        public const string Institutions = "Institutions";
        public const string Files = "Files";
        public const string Users = "Users";
        public const string InstitutionMembers = "InstitutionMembers";
        public const string Subjects = "Subjects";
        public const string Grades = "Grades";
        public const string Groups = "Groups";
        public const string Lessons = "Lessons";
        public const string Courses = "Courses";
        public const string Messages = "Messages";
    }

    public static class Routes
    {
        public const string Auth = "/auth";
        public const string Assignments = "/assignments";
        public const string ClassroomTypes = "/classroom-types";
        public const string LessonStatuses = "/lesson-statuses";
        public const string Terms = "/terms";
        public const string TimetableUnits = "/timetable-units";
        public const string Classrooms = "/classrooms";
        public const string Students = "/students";
        public const string Submissions = "/submissions";
        public const string Roles = "/roles";
        public const string Institutions = "/institutions";
        public const string Files = "/files";
        public const string Users = "/users";
        public const string InstitutionMembers = "/institution-members";
        public const string Subjects = "/subjects";
        public const string Grades = "/grades";
        public const string Groups = "/groups";
        public const string Lessons = "/lessons";
        public const string Courses = "/courses";
        public const string Messages = "/messages";
    }

    private static class RateLimits
    {
        public const string Auth = "auth";
        public const string Default = "default";
    }
    
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
        app.MapGradesEndpoints();
        app.MapSubjectsEndpoints();
        app.MapGroupsEndpoints();
        app.MapLessonsEndpoints();
        app.MapCoursesEndpoints();
        app.MapAssignmentsEndpoints();
        app.MapSubmissionsEndpoints();
        app.MapMessagesEndpoints();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var publicEndpoints = app.MapGroup(Routes.Auth)
            .WithTags(Tags.Authentication)
            .RequireRateLimiting(RateLimits.Auth);

        publicEndpoints.MapPublicGroup()
            .MapEndpoint<Login>()
            .MapEndpoint<RefreshToken>();
        
        var protectedEndpoints = app.MapGroup(Routes.Auth)
            .WithTags(Tags.Authentication)
            .RequireUserId()
            .RequireRateLimiting(RateLimits.Auth);

        protectedEndpoints
            .MapEndpoint<Logout>()
            .MapEndpoint<LogoutAll>();
    }

    private static void MapClassroomTypesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.ClassroomTypes, Tags.ClassroomTypes);

        endpoints
            .MapEndpoint<GetClassroomTypesEndpoint>()
            .MapEndpoint<GetClassroomTypeByIdEndpoint>()
            .MapEndpoint<CreateClassroomTypeEndpoint>();
    }

    private static void MapLessonStatusesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.LessonStatuses, Tags.LessonStatuses);

        endpoints
            .MapEndpoint<GetLessonStatusesEndpoint>()
            .MapEndpoint<GetLessonStatusByIdEndpoint>()
            .MapEndpoint<GetLessonStatusesBulkEndpoint>();
    }

    private static void MapTermsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Terms, Tags.Terms);

        endpoints
            .MapEndpoint<GetTermsEndpoint>()
            .MapEndpoint<GetTermByIdEndpoint>()
            .MapEndpoint<CreateTermEndpoint>();
    }

    private static void MapTimetableUnitsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.TimetableUnits, Tags.TimetableUnits);

        endpoints
            .MapEndpoint<GetTimetableUnitsEndpoint>()
            .MapEndpoint<GetTimetableUnitByIdEndpoint>()
            .MapEndpoint<CreateTimetableUnitEndpoint>();
    }

    private static void MapClassroomsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Classrooms, Tags.Classrooms);

        endpoints
            .MapEndpoint<GetClassroomsEndpoint>()
            .MapEndpoint<GetClassroomByIdEndpoint>()
            .MapEndpoint<CreateClassroomEndpoint>()
            .MapEndpoint<GetClassroomsBulkEndpoint>();
    }

    private static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Students, Tags.Students);

        endpoints
            .MapEndpoint<CreateStudentEndpoint>();
            //.RequireAuthorization(Permissions.Students.Create);

        endpoints
            .MapEndpoint<GetStudentByIdEndpoint>();
            //.RequireAuthorization(Permissions.Students.Get);

        endpoints
            .MapEndpoint<GetStudentsEndpoint>();
            //.RequireAuthorization(Permissions.Students.Get);

        endpoints
            .MapEndpoint<GetStudentCoursesEndpoint>()
            .MapEndpoint<GetStudentLessonsEndpoint>()
            .MapEndpoint<GetAttendanceCountEndpoint>();
    }

    private static void MapRolesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Roles, Tags.Roles);

        endpoints
            .MapEndpoint<CreateRoleEndpoint>()
            .RequireAuthorization(Permissions.Roles.Create);
        
        endpoints.MapEndpoint<GetRolesEndpoint>()
            .MapEndpoint<GetRoleByIdEndpoint>()
            .MapEndpoint<GetRolePermissionsEndpoint>()
            .MapEndpoint<UpdatePermissionsEndpoint>();
    }
    
    private static void MapInstitutionEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup(Routes.Institutions)
            .WithTags(Tags.Institutions)
            .RequireRateLimiting(RateLimits.Default);
        
        endpoints
            .MapEndpoint<GetInstitutionsEndpoint>()
            .MapEndpoint<GetInstitutionByIdEndpoint>();

        endpoints
            .MapEndpoint<CreateInstitutionEndpoint>();
        //.RequireAuthorization(Permissions.Institutions.Create);
    }
    
    private static void MapFilesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup(Routes.Files)
            .WithTags(Tags.Files)
            .RequireRateLimiting(RateLimits.Default);

        endpoints.MapEndpoint<GetFileByUrlEndpoint>();
    }
    
    private static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Users, Tags.Users);

        endpoints.MapEndpoint<GetUserPermissionsEndpoint>()
            .MapEndpoint<GetUsersBulkEndpoint>();
    }

    private static void MapInstitutionMembersEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.InstitutionMembers, Tags.InstitutionMembers);

        endpoints.MapEndpoint<CreateInstitutionMemberEndpoint>()
            .MapEndpoint<GetInstitutionMembersEndpoint>()
            .MapEndpoint<GetInstitutionMemberByIdEndpoint>()
            .MapEndpoint<UpdateTeacherSubjectsEndpoint>()
            .MapEndpoint<UpdateInstitutionMemberGroupsEndpoint>()
            .MapEndpoint<GetInstitutionMembersBulkEndpoint>();
    }

    private static void MapGradesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Grades, Tags.Grades);

        endpoints.MapEndpoint<GetGradesEndpoint>()
            .MapEndpoint<GetGradeByIdEndpoint>()
            .MapEndpoint<GetGradesBulkEndpoint>();
    }

    private static void MapSubjectsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Subjects, Tags.Subjects);

        endpoints.MapEndpoint<GetSubjectsEndpoint>()
            .MapEndpoint<GetSubjectByIdEndpoint>()
            .MapEndpoint<CreateSubjectEndpoint>()
            .MapEndpoint<GetSubjectsBulkEndpoint>();
    }

    private static void MapGroupsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Groups, Tags.Groups);

        endpoints.MapEndpoint<GetGroupsEndpoint>()
            .MapEndpoint<GetGroupByIdEndpoint>()
            .MapEndpoint<CreateGroupEndpoint>()
            .MapEndpoint<UpdateGroupStudentsEndpoint>()
            .MapEndpoint<UpdateGroupInstitutionMembersEndpoint>();
    }

    private static void MapLessonsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Lessons, Tags.Lessons);

        endpoints
            .MapEndpoint<GetLessonsEndpoint>()
            .MapEndpoint<GetLessonByIdEndpoint>()
            .MapEndpoint<CreateLessonEndpoint>()
            .MapEndpoint<UpdateLessonEndpoint>()
            .MapEndpoint<UpdateLessonToStudentEndpoint>()
            .MapEndpoint<GetLessonStudentsEndpoint>();
    }

    private static void MapCoursesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Courses, Tags.Courses);

        endpoints.MapEndpoint<GetCoursesEndpoint>()
            .MapEndpoint<GetCourseByIdEndpoint>()
            .MapEndpoint<CreateCourseEndpoint>()
            .MapEndpoint<UpdateCourseStudentsEndpoint>()
            .MapEndpoint<GetCourseStudentsEndpoint>()
            .MapEndpoint<UpdateCourseToStudentEndpoint>()
            .MapEndpoint<GetCoursesBulkEndpoint>();
    }

    private static void MapAssignmentsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Assignments, Tags.Assignments);

        endpoints.MapEndpoint<GetAssignmentsEndpoint>()
            .MapEndpoint<GetAssignmentByIdEndpoint>()
            .MapEndpoint<GetAssignmentFilesEndpoint>()
            .MapEndpoint<CreateAssignmentEndpoint>()
            .MapEndpoint<UpdateAssignmentEndpoint>()
            .MapEndpoint<GetAssignmentsBulkEndpoint>();
    }

    private static void MapSubmissionsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Submissions, Tags.Submissions);

        endpoints.MapEndpoint<GetSubmissionsEndpoint>()
            .MapEndpoint<GetSubmissionByIdEndpoint>()
            .MapEndpoint<GetSubmissionFilesEndpoint>()
            .MapEndpoint<SubmitSubmissionEndpoint>()
            .MapEndpoint<ReviewSubmissionEndpoint>()
            .MapEndpoint<GetSubmissionsBulkEndpoint>();
    }

    private static void MapMessagesEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.CreateInstitutionGroup(Routes.Messages, Tags.Messages);

        endpoints.MapEndpoint<GetMessagesEndpoint>()
            .MapEndpoint<GetMessageByIdEndpoint>()
            .MapEndpoint<CreateMessageEndpoint>()
            .MapEndpoint<UpdateMessageEndpoint>();
    }


    #region Helper Extensions

    private static RouteGroupBuilder CreateInstitutionGroup(this IEndpointRouteBuilder app, string route, string tag)
    {
        return app.MapGroup(route)
            .WithTags(tag)
            .RequireInstitution()
            .RequireRateLimiting(RateLimits.Default);
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

    public static RouteHandlerBuilder RequireUserId(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<RequireUserIdFilter>();
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
    
    #endregion
}