using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class LessonStatusesAreGlobalUsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lesson_statuses_institution_id_fkey",
                table: "lesson_statuses");

            migrationBuilder.DropIndex(
                name: "IX_lesson_statuses_institution_id",
                table: "lesson_statuses");

            migrationBuilder.DropIndex(
                name: "lesson_statuses_description_institution_deleted_at_key",
                table: "lesson_statuses");

            migrationBuilder.DropColumn(
                name: "argb_color",
                table: "lesson_statuses");

            migrationBuilder.DropColumn(
                name: "institution_id",
                table: "lesson_statuses");

            migrationBuilder.CreateIndex(
                name: "lesson_statuses_description_deleted_at_key",
                table: "lesson_statuses",
                columns: new[] { "description", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "lesson_statuses_description_deleted_at_key",
                table: "lesson_statuses");

            migrationBuilder.AddColumn<int>(
                name: "argb_color",
                table: "lesson_statuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "institution_id",
                table: "lesson_statuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_lesson_statuses_institution_id",
                table: "lesson_statuses",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "lesson_statuses_description_institution_deleted_at_key",
                table: "lesson_statuses",
                columns: new[] { "description", "institution_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.AddForeignKey(
                name: "lesson_statuses_institution_id_fkey",
                table: "lesson_statuses",
                column: "institution_id",
                principalTable: "institutions",
                principalColumn: "id");
        }
    }
}
