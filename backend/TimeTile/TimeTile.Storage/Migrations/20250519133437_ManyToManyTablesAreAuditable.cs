using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyTablesAreAuditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "role_permissions_permission_id_fkey",
                table: "roles_permissions");

            migrationBuilder.DropForeignKey(
                name: "role_permissions_role_id_fkey",
                table: "roles_permissions");

            migrationBuilder.DropTable(
                name: "subjects_institutions");

            migrationBuilder.DropPrimaryKey(
                name: "role_permissions_pkey",
                table: "roles_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "lessons_students_pkey",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students");

            migrationBuilder.DropPrimaryKey(
                name: "courses_students_pkey",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionX_Positive",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionY_Positive",
                table: "courses_students");

            migrationBuilder.AddColumn<int>(
                name: "institution_id",
                table: "subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "roles_permissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "lessons_students",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "courses_students",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles_permissions",
                table: "roles_permissions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lessons_students",
                table: "lessons_students",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_courses_students",
                table: "courses_students",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_subjects_institution_id",
                table: "subjects",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "roles_permissions_role_id_permission_id_key",
                table: "roles_permissions",
                columns: new[] { "role_id", "permission_id" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students",
                sql: "(\"came_at\" IS NULL AND \"left_at\" IS NULL) OR (\"came_at\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students",
                sql: "(\"came_at\" < \"left_at\") OR (\"left_at\" IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students",
                sql: "\"has_exam\" = FALSE OR \"exam_grade_id\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_PositionX_Positive",
                table: "courses_students",
                sql: "\"position_x\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_PositionY_Positive",
                table: "courses_students",
                sql: "\"position_y\" >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_students_auditable_entities_id",
                table: "courses_students",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lessons_students_auditable_entities_id",
                table: "lessons_students",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_permissions_auditable_entities_id",
                table: "roles_permissions",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "roles_permissions_permission_id_fkey",
                table: "roles_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "roles_permissions_role_id_fkey",
                table: "roles_permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "subjects_institution_id_fkey",
                table: "subjects",
                column: "institution_id",
                principalTable: "institutions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_students_auditable_entities_id",
                table: "courses_students");

            migrationBuilder.DropForeignKey(
                name: "FK_lessons_students_auditable_entities_id",
                table: "lessons_students");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_permissions_auditable_entities_id",
                table: "roles_permissions");

            migrationBuilder.DropForeignKey(
                name: "roles_permissions_permission_id_fkey",
                table: "roles_permissions");

            migrationBuilder.DropForeignKey(
                name: "roles_permissions_role_id_fkey",
                table: "roles_permissions");

            migrationBuilder.DropForeignKey(
                name: "subjects_institution_id_fkey",
                table: "subjects");

            migrationBuilder.DropIndex(
                name: "IX_subjects_institution_id",
                table: "subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles_permissions",
                table: "roles_permissions");

            migrationBuilder.DropIndex(
                name: "roles_permissions_role_id_permission_id_key",
                table: "roles_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lessons_students",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_courses_students",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionX_Positive",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionY_Positive",
                table: "courses_students");

            migrationBuilder.DropColumn(
                name: "institution_id",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "id",
                table: "roles_permissions");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "lessons_students",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "courses_students",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "role_permissions_pkey",
                table: "roles_permissions",
                columns: new[] { "role_id", "permission_id" });

            migrationBuilder.AddPrimaryKey(
                name: "lessons_students_pkey",
                table: "lessons_students",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "courses_students_pkey",
                table: "courses_students",
                column: "id");

            migrationBuilder.CreateTable(
                name: "subjects_institutions",
                columns: table => new
                {
                    institution_id = table.Column<int>(type: "integer", nullable: false),
                    subject_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjects_institutions_pkey", x => new { x.institution_id, x.subject_id });
                    table.ForeignKey(
                        name: "subjects_institutions_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "subjects_institutions_subject_id_fkey",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id");
                });

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students",
                sql: "(\"came_at\" IS NULL AND \"left_at\" IS NULL) OR (\"came_at\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students",
                sql: "(\"came_at\" < \"left_at\") OR (\"left_at\" IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students",
                sql: "\"has_exam\" = FALSE OR \"exam_grade_id\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_PositionX_Positive",
                table: "courses_students",
                sql: "\"position_x\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_PositionY_Positive",
                table: "courses_students",
                sql: "\"position_y\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_subjects_institutions_subject_id",
                table: "subjects_institutions",
                column: "subject_id");

            migrationBuilder.AddForeignKey(
                name: "role_permissions_permission_id_fkey",
                table: "roles_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "role_permissions_role_id_fkey",
                table: "roles_permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id");
        }
    }
}
