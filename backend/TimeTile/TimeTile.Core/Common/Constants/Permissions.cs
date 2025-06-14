using System.Reflection;

namespace TimeTile.Core.Common.Constants;

public static class Permissions
{
    public const string AddStudent = "AddStudent";
    public const string DeleteStudent = "DeleteStudent";
    public const string CreateStudent = "CreateStudent";
    public const string GetStudents = "GetStudents";
    public const string CreateRole = "CreateRole";
    public const string CreateInstitution = "CreateInstitution";
    public const string GetSchedule = "GetSchedule";
    public const string GetOwnGroup = "GetOwnGroup";

    private static readonly Lazy<IReadOnlyList<string>> _allPermissions = new(() => 
        typeof(Permissions)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(fi => fi.GetRawConstantValue()?.ToString())
            .ToList()!);

    public static IReadOnlyList<string> All => _allPermissions.Value;
    public static int Count => All.Count;
}