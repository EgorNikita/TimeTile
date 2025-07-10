using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionHasNoUniqueKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "submissions_assignment_student_status_deleted_at_key",
                table: "submissions");

            migrationBuilder.CreateIndex(
                name: "IX_submissions_assignment_id",
                table: "submissions",
                column: "assignment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_submissions_assignment_id",
                table: "submissions");

            migrationBuilder.CreateIndex(
                name: "submissions_assignment_student_status_deleted_at_key",
                table: "submissions",
                columns: new[] { "assignment_id", "student_id", "status", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);
        }
    }
}
