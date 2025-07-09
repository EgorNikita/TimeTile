using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class LessonToStudentHasOnlyOneGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lessons_students_classwork_grade_id_fkey",
                table: "lessons_students");

            migrationBuilder.DropForeignKey(
                name: "lessons_students_homework_grade_id_fkey",
                table: "lessons_students");

            migrationBuilder.DropIndex(
                name: "IX_lessons_students_classwork_grade_id",
                table: "lessons_students");

            migrationBuilder.DropColumn(
                name: "classwork_grade_id",
                table: "lessons_students");

            migrationBuilder.RenameColumn(
                name: "homework_grade_id",
                table: "lessons_students",
                newName: "grade_id");

            migrationBuilder.RenameIndex(
                name: "IX_lessons_students_homework_grade_id",
                table: "lessons_students",
                newName: "IX_lessons_students_grade_id");

            migrationBuilder.AddForeignKey(
                name: "lessons_students_grade_id_fkey",
                table: "lessons_students",
                column: "grade_id",
                principalTable: "grades",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lessons_students_grade_id_fkey",
                table: "lessons_students");

            migrationBuilder.RenameColumn(
                name: "grade_id",
                table: "lessons_students",
                newName: "homework_grade_id");

            migrationBuilder.RenameIndex(
                name: "IX_lessons_students_grade_id",
                table: "lessons_students",
                newName: "IX_lessons_students_homework_grade_id");

            migrationBuilder.AddColumn<int>(
                name: "classwork_grade_id",
                table: "lessons_students",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_lessons_students_classwork_grade_id",
                table: "lessons_students",
                column: "classwork_grade_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "lessons_students_classwork_grade_id_fkey",
                table: "lessons_students",
                column: "classwork_grade_id",
                principalTable: "grades",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "lessons_students_homework_grade_id_fkey",
                table: "lessons_students",
                column: "homework_grade_id",
                principalTable: "grades",
                principalColumn: "id");
        }
    }
}
