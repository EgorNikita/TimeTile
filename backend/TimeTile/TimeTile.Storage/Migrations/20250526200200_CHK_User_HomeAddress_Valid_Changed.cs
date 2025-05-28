using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CHK_User_HomeAddress_Valid_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users",
                sql: "\"home_address\" ~ '^[A-Za-z\\d''\\.\\- ,]+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users",
                sql: "\"home_address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");
        }
    }
}
