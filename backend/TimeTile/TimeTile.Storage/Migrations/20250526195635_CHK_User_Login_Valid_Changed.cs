using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CHK_User_Login_Valid_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^[A-Za-z\\d._%+-]+@[A-Za-z\\d.-]+\\.[A-Za-z]{2,}$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^[\\w -]+$'");
        }
    }
}
