using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CourseToStudentHasTermMarkInsteadOfExamGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Type_Valid",
                table: "grades");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students");

            migrationBuilder.DropColumn(
                name: "has_exam",
                table: "courses_students");

            migrationBuilder.RenameColumn(
                name: "exam_grade_id",
                table: "courses_students",
                newName: "grade_id");

            migrationBuilder.RenameIndex(
                name: "IX_courses_students_exam_grade_id",
                table: "courses_students",
                newName: "IX_courses_students_grade_id");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Type_Valid",
                table: "grades",
                sql: "LOWER(\"type\") IN ('classwork', 'homework', 'termmark')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Type_Valid",
                table: "grades");

            migrationBuilder.RenameColumn(
                name: "grade_id",
                table: "courses_students",
                newName: "exam_grade_id");

            migrationBuilder.RenameIndex(
                name: "IX_courses_students_grade_id",
                table: "courses_students",
                newName: "IX_courses_students_exam_grade_id");

            migrationBuilder.AddColumn<bool>(
                name: "has_exam",
                table: "courses_students",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Type_Valid",
                table: "grades",
                sql: "LOWER(\"type\") IN ('classwork', 'homework', 'exam')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students",
                sql: "\"has_exam\" = TRUE OR \"exam_grade_id\" IS NULL");
        }
    }
}
