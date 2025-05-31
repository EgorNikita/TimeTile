using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CHK_Permission_Description_Valid_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[\\w .-]+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[\\w -]+$'");
        }
    }
}
