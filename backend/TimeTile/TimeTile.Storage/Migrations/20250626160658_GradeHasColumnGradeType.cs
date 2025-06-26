using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class GradeHasColumnGradeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "grades",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Type_Valid",
                table: "grades",
                sql: "LOWER(\"type\") IN ('classwork', 'homework', 'exam')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Type_Valid",
                table: "grades");

            migrationBuilder.DropColumn(
                name: "type",
                table: "grades");
        }
    }
}
