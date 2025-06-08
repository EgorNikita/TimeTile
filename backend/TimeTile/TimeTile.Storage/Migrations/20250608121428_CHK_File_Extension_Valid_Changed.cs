using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CHK_File_Extension_Valid_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_File_Extension_Valid",
                table: "files");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_File_Extension_Valid",
                table: "files",
                sql: "LOWER(\"extension\") IN ('pdf', 'docx', 'xlsx', 'png', 'jpg', 'jpeg', 'txt', 'zip')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_File_Extension_Valid",
                table: "files");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_File_Extension_Valid",
                table: "files",
                sql: "\"extension\" IN ('pdf', 'docx', 'xlsx', 'png', 'jpg', 'jpeg', 'txt', 'zip')");
        }
    }
}
