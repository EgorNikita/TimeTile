using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CK_CoursesStudents_HasExam_ExamGrade_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students",
                sql: "\"has_exam\" = TRUE OR \"exam_grade_id\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students",
                sql: "\"has_exam\" = FALSE OR \"exam_grade_id\" IS NOT NULL");
        }
    }
}
