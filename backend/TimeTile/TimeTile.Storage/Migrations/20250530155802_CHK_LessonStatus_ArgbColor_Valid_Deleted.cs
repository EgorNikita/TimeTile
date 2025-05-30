using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CHK_LessonStatus_ArgbColor_Valid_Deleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_ArgbColor_Valid",
                table: "lesson_statuses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_ArgbColor_Valid",
                table: "lesson_statuses",
                sql: "\"argb_color\" >= 0");
        }
    }
}
