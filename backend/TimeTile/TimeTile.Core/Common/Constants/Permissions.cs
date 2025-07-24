using System.Reflection;

namespace TimeTile.Core.Common.Constants;

public static class Permissions
{
    public static class Students
    {
        public const string Add = "AddStudent";
        public const string Delete = "DeleteStudent";
        public const string Create = "CreateStudent";
        public const string Get = "GetStudents";
    }

    public static class Roles
    {
        public const string Get = "GetRoles";
        public const string Create = "CreateRole";
    }

    public static class Institutions
    {
        public const string Create = "CreateInstitution";
    }

    public static class Groups
    {
        public const string Get = "GetGroups";
        public const string Create = "CreateGroup";
        public const string UpdateStudents = "UpdateStudentsOfGroup";
        public const string UpdateInstitutionMembers = "UpdateInstitutionMembersOfGroup";
    }

    public static class Grades
    {
        public const string Get = "GetGrades";
    }

    public static class Lessons
    {
        public const string Get = "GetLessons";
        public const string Create = "CreateLesson";
        public const string Update = "UpdateLesson";
        public const string UpdateLessonToStudent = "UpdateLessonToStudent";
    }

    public static class Submissions
    {
        public const string Get = "GetSubmissions";
        public const string Submit = "SubmitSubmission";
        public const string Review = "ReviewSubmission";
    }

    public static class Assignments
    {
        public const string Get = "GetAssignments";
    }

    public static class Courses
    {
        public const string Get = "GetCourses";
        public const string Create = "CreateCourse";
        public const string AddStudent = "AddStudentToCourse";
        public const string RemoveStudent = "RemoveStudentFromCourse";
        public const string UpdateTeacher = "UpdateTeacher";
        public const string UpdateCourseToStudent = "UpdateCourseToStudent";
    }

    public static class Messages
    {
        public const string Get = "GetMessages";
    }

    private static readonly Lazy<IReadOnlyList<string>> _allPermissions = new(() => 
        typeof(Permissions)
            .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(fi => fi.GetRawConstantValue()?.ToString())
            .Where(value => !string.IsNullOrEmpty(value))
            .ToList()!);

    public static IReadOnlyList<string> All => _allPermissions.Value;
    public static int Count => All.Count;
}