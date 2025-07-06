using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CourseCanHaveAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "icon_id",
                table: "courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_courses_icon_id",
                table: "courses",
                column: "icon_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "courses_icon_id_fkey",
                table: "courses",
                column: "icon_id",
                principalTable: "files",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "courses_icon_id_fkey",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "IX_courses_icon_id",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "icon_id",
                table: "courses");
        }
    }
}
