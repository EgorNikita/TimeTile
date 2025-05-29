namespace TimeTile.API.Common.Constants;

public static class Permissions
{
    public const string AddStudent = "AddStudent";
    public const string DeleteStudent = "DeleteStudent";
    public const string CreateStudent = "CreateStudent";
    public const string CreateRole = "CreateRole";
    public const string CreateInstitution = "CreateInstitution";

    public static readonly IReadOnlyList<string> All = new List<string>
    {
        AddStudent,
        DeleteStudent,
        CreateStudent,
        CreateRole,
        CreateInstitution
    };
}