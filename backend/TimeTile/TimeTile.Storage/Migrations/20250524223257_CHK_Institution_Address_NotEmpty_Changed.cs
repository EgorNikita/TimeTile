using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CHK_Institution_Address_NotEmpty_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions",
                sql: "\"address\" ~ '^[A-Za-z\\d''\\.\\- ,]+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions",
                sql: "\"address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");
        }
    }
}
