using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CHK_User_PhoneNumber_Valid_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users",
                sql: "\"phone_number\" ~ '^\\+?[1-9]\\d{1,14}$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users",
                sql: "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");
        }
    }
}
