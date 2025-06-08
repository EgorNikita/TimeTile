namespace TimeTile.Core.Common.Constants;

public static class Permissions
{
    public const string AddStudent = "AddStudent";
    public const string DeleteStudent = "DeleteStudent";
    public const string CreateStudent = "CreateStudent";
    public const string GetStudents = "CreateStudent";
    public const string CreateRole = "CreateRole";
    public const string CreateInstitution = "CreateInstitution";

    public static readonly IReadOnlyList<string> All = new List<string>
    {
        AddStudent,
        DeleteStudent,
        CreateStudent,
        GetStudents,
        CreateRole,
        CreateInstitution
    };

    public static int Count => All.Count;
}