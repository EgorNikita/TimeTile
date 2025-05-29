using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class RegexPatternsMaxLengthConstraintsWereUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Domain_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms");

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "users",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "login",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(263)",
                oldMaxLength: 263);

            migrationBuilder.AlterColumn<string>(
                name: "lastname",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "home_address",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "firstname",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "timetable_units",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "subjects",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "roles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "permissions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "homework_description",
                table: "lessons",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lessons",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lesson_statuses",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "institutions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "institutions",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "institutions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "domain",
                table: "institutions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "institutions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "groups",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "courses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "classrooms",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "classroom_types",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users",
                sql: "\"firstname\" ~ '^[[:alpha:]]+(?:[\\s''-][[:alpha:]]+)*$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users",
                sql: "\"home_address\" ~ '^[[:alpha:]\\d\\s''.,#/()-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users",
                sql: "\"lastname\" ~ '^[[:alpha:]]+(?:[\\s''-][[:alpha:]]+)*$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^(?!\\.)[A-Za-z0-9._%+-]+(?<!\\.)@[A-Za-z0-9-]+(?:\\.[A-Za-z0-9-]+)*\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users",
                sql: "\"phone_number\" ~ '^\\+[1-9]\\d{6,14}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units",
                sql: "\"title\" ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms",
                sql: "\"title\" ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects",
                sql: "\"title\" ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles",
                sql: "\"title\"  ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[[:alpha:]\\d\\s.,!?]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses",
                sql: "\"description\"  ~ '^[[:alpha:]\\d\\s.,!?]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions",
                sql: "\"address\" ~ '^[[:alpha:]\\d\\s''.,#/()-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Domain_Valid",
                table: "institutions",
                sql: "\"domain\" ~ '^(?:[[:alpha:]0-9-]{1,63}\\.)+[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions",
                sql: "\"email\" ~ '^(?!\\.)[A-Za-z0-9._%+-]+(?<!\\.)@[A-Za-z0-9-]+(?:\\.[A-Za-z0-9-]+)*\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions",
                sql: "\"phone_number\" ~ '^\\+[1-9]\\d{6,14}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions",
                sql: "\"title\" ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups",
                sql: "\"title\"  ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses",
                sql: "\"title\"  ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms",
                sql: "\"title\" ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Domain_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms");

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "login",
                table: "users",
                type: "character varying(263)",
                maxLength: 263,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "lastname",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "home_address",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "firstname",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "timetable_units",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "subjects",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "roles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "permissions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "homework_description",
                table: "lessons",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lessons",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lesson_statuses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "institutions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "institutions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "institutions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "domain",
                table: "institutions",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "institutions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "groups",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "courses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "classrooms",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "classroom_types",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users",
                sql: "\"firstname\" ~ '^[a-zA-Z ,.''-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users",
                sql: "\"home_address\" ~ '^[A-Za-z\\d''\\.\\- ,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users",
                sql: "\"lastname\" ~ '^[a-zA-Z ,.''-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^[A-Za-z\\d._%+-]+@[A-Za-z\\d.-]+\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users",
                sql: "\"phone_number\" ~ '^\\+?[1-9]\\d{1,14}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units",
                sql: "\"title\" ~ '^[\\w ]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms",
                sql: "\"title\" ~ '^[\\w -.*+,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects",
                sql: "\"title\" ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles",
                sql: "\"title\"  ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses",
                sql: "\"description\"  ~ '^[a-zA-Z\\d ]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions",
                sql: "\"address\" ~ '^[A-Za-z\\d''\\.\\- ,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Domain_Valid",
                table: "institutions",
                sql: "\"domain\" ~ '^(?!-)[A-Za-z0-9-]{1,63}(?<!-)(\\.[A-Za-z]{2,})+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions",
                sql: "\"email\" ~ '^[A-Za-z\\d._%+-]+@[A-Za-z\\d.-]+\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions",
                sql: "\"phone_number\" ~ '^\\+?[1-9]\\d{1,14}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions",
                sql: "\"title\" ~ '^[\\w \\-.*&\"'',\\/\\\\|]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups",
                sql: "\"title\"  ~ '^[\\w -.*]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses",
                sql: "\"title\"  ~ '^[\\w -.*+,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms",
                sql: "\"title\" ~ '^[a-zA-Z \\d-]+$'");
        }
    }
}
