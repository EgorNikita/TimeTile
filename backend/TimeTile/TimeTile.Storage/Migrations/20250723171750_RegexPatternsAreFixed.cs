using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class RegexPatternsAreFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Assignment_Description_Valid",
                table: "assignments");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^[A-Za-z0-9][A-Za-z0-9._%+-]*[A-Za-z0-9]@[A-Za-z0-9-]+(?:\\.[A-Za-z0-9-]+)*\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[[:alpha:][:digit:][:space:].,!?:-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses",
                sql: "\"description\"  ~ '^[[:alpha:][:digit:][:space:].,!?:-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions",
                sql: "\"email\" ~ '^[A-Za-z0-9][A-Za-z0-9._%+-]*[A-Za-z0-9]@[A-Za-z0-9-]+(?:\\.[A-Za-z0-9-]+)*\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Assignment_Description_Valid",
                table: "assignments",
                sql: "\"description\"  ~ '^[[:alpha:][:digit:][:space:].,!?:-]+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Assignment_Description_Valid",
                table: "assignments");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^(?!\\.)[A-Za-z0-9._%+-]+(?<!\\.)@[A-Za-z0-9-]+(?:\\.[A-Za-z0-9-]+)*\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[[:alpha:]\\d\\s.,!?]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses",
                sql: "\"description\"  ~ '^[[:alpha:]\\d\\s.,!?]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions",
                sql: "\"email\" ~ '^(?!\\.)[A-Za-z0-9._%+-]+(?<!\\.)@[A-Za-z0-9-]+(?:\\.[A-Za-z0-9-]+)*\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Assignment_Description_Valid",
                table: "assignments",
                sql: "\"description\"  ~ '^[[:alpha:]\\d\\s.,!?]+$'");
        }
    }
}
