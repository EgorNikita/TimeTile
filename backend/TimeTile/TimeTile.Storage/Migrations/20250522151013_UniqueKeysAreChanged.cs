using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class UniqueKeysAreChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "users_login_key",
                table: "users");

            migrationBuilder.DropIndex(
                name: "timetable_units_institution_title_key",
                table: "timetable_units");

            migrationBuilder.DropIndex(
                name: "timetable_units_institution_title_start_end_key",
                table: "timetable_units");

            migrationBuilder.DropIndex(
                name: "terms_institution_start_end_key",
                table: "terms");

            migrationBuilder.DropIndex(
                name: "terms_institution_title_key",
                table: "terms");

            migrationBuilder.DropIndex(
                name: "teachers_subjects_teacher_id_subject_id_key",
                table: "teachers_subjects");

            migrationBuilder.DropIndex(
                name: "subjects_title_key",
                table: "subjects");

            migrationBuilder.DropIndex(
                name: "students_id_group_key",
                table: "students");

            migrationBuilder.DropIndex(
                name: "roles_permissions_role_id_permission_id_key",
                table: "roles_permissions");

            migrationBuilder.DropIndex(
                name: "roles_title_institution_key",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "permissions_description_key",
                table: "permissions");

            migrationBuilder.DropIndex(
                name: "lessons_students_lesson_id_student_id_key",
                table: "lessons_students");

            migrationBuilder.DropIndex(
                name: "lessons_course_timetable_date_key",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "lesson_statuses_description_key",
                table: "lesson_statuses");

            migrationBuilder.DropIndex(
                name: "institutions_title_key",
                table: "institutions");

            migrationBuilder.DropIndex(
                name: "institution_members_groups_institution_member_id_group_id_key",
                table: "institution_members_groups");

            migrationBuilder.DropIndex(
                name: "groups_institution_title_key",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "files_storage_path_key",
                table: "files");

            migrationBuilder.DropIndex(
                name: "courses_students_course_id_student_id_key",
                table: "courses_students");

            migrationBuilder.DropIndex(
                name: "courses_title_subject_teacher_institution_term_key",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "classrooms_institution_title_key",
                table: "classrooms");

            migrationBuilder.DropIndex(
                name: "classroom_types_description_institution_id_key",
                table: "classroom_types");

            migrationBuilder.CreateIndex(
                name: "users_login_deleted_at_key",
                table: "users",
                columns: new[] { "login", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "timetable_units_institution_start_end_deleted_at_key",
                table: "timetable_units",
                columns: new[] { "institution_id", "start_time", "end_time", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "timetable_units_institution_title_deleted_at_key",
                table: "timetable_units",
                columns: new[] { "institution_id", "title", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "terms_institution_start_end_deleted_at_key",
                table: "terms",
                columns: new[] { "start_date", "end_date", "institution_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "terms_institution_title_deleted_at_key",
                table: "terms",
                columns: new[] { "institution_id", "title", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "teachers_subjects_teacher_subject_deleted_at_key",
                table: "teachers_subjects",
                columns: new[] { "teacher_id", "subject_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "subjects_title_institution_deleted_at_key",
                table: "subjects",
                columns: new[] { "title", "institution_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "roles_permissions_role_permission_deleted_at_key",
                table: "roles_permissions",
                columns: new[] { "role_id", "permission_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "roles_title_institution_deleted_at_key",
                table: "roles",
                columns: new[] { "institution_id", "title", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "permissions_description_deleted_at_key",
                table: "permissions",
                columns: new[] { "description", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "lessons_students_lesson_student_deleted_at_key",
                table: "lessons_students",
                columns: new[] { "lesson_id", "student_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "lessons_course_timetable_date_deleted_at_key",
                table: "lessons",
                columns: new[] { "course_id", "timetable_unit_id", "date", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "lesson_statuses_description_institution_deleted_at_key",
                table: "lesson_statuses",
                columns: new[] { "description", "institution_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "institutions_title_deleted_at_key",
                table: "institutions",
                columns: new[] { "title", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "institution_members_groups_institution_member_group_deleted_at_key",
                table: "institution_members_groups",
                columns: new[] { "institution_member_id", "group_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "groups_institution_title_deleted_at_key",
                table: "groups",
                columns: new[] { "institution_id", "title", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "files_storage_path_deleted_at_key",
                table: "files",
                columns: new[] { "storage_path", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "courses_students_course_student_deleted_at_key",
                table: "courses_students",
                columns: new[] { "course_id", "student_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "courses_title_subject_teacher_institution_term_deleted_at_key",
                table: "courses",
                columns: new[] { "title", "subject_id", "teacher_id", "institution_id", "term_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "classrooms_institution_title_deleted_at_key",
                table: "classrooms",
                columns: new[] { "institution_id", "title", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "classroom_types_description_institution_deleted_at_key",
                table: "classroom_types",
                columns: new[] { "description", "institution_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "users_login_deleted_at_key",
                table: "users");

            migrationBuilder.DropIndex(
                name: "timetable_units_institution_start_end_deleted_at_key",
                table: "timetable_units");

            migrationBuilder.DropIndex(
                name: "timetable_units_institution_title_deleted_at_key",
                table: "timetable_units");

            migrationBuilder.DropIndex(
                name: "terms_institution_start_end_deleted_at_key",
                table: "terms");

            migrationBuilder.DropIndex(
                name: "terms_institution_title_deleted_at_key",
                table: "terms");

            migrationBuilder.DropIndex(
                name: "teachers_subjects_teacher_subject_deleted_at_key",
                table: "teachers_subjects");

            migrationBuilder.DropIndex(
                name: "subjects_title_institution_deleted_at_key",
                table: "subjects");

            migrationBuilder.DropIndex(
                name: "roles_permissions_role_permission_deleted_at_key",
                table: "roles_permissions");

            migrationBuilder.DropIndex(
                name: "roles_title_institution_deleted_at_key",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "permissions_description_deleted_at_key",
                table: "permissions");

            migrationBuilder.DropIndex(
                name: "lessons_students_lesson_student_deleted_at_key",
                table: "lessons_students");

            migrationBuilder.DropIndex(
                name: "lessons_course_timetable_date_deleted_at_key",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "lesson_statuses_description_institution_deleted_at_key",
                table: "lesson_statuses");

            migrationBuilder.DropIndex(
                name: "institutions_title_deleted_at_key",
                table: "institutions");

            migrationBuilder.DropIndex(
                name: "institution_members_groups_institution_member_group_deleted_at_key",
                table: "institution_members_groups");

            migrationBuilder.DropIndex(
                name: "groups_institution_title_deleted_at_key",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "files_storage_path_deleted_at_key",
                table: "files");

            migrationBuilder.DropIndex(
                name: "courses_students_course_student_deleted_at_key",
                table: "courses_students");

            migrationBuilder.DropIndex(
                name: "courses_title_subject_teacher_institution_term_deleted_at_key",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "classrooms_institution_title_deleted_at_key",
                table: "classrooms");

            migrationBuilder.DropIndex(
                name: "classroom_types_description_institution_deleted_at_key",
                table: "classroom_types");

            migrationBuilder.CreateIndex(
                name: "users_login_key",
                table: "users",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "timetable_units_institution_title_key",
                table: "timetable_units",
                columns: new[] { "institution_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "timetable_units_institution_title_start_end_key",
                table: "timetable_units",
                columns: new[] { "institution_id", "title", "start_time", "end_time" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "terms_institution_start_end_key",
                table: "terms",
                columns: new[] { "start_date", "end_date", "institution_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "terms_institution_title_key",
                table: "terms",
                columns: new[] { "institution_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "teachers_subjects_teacher_id_subject_id_key",
                table: "teachers_subjects",
                columns: new[] { "teacher_id", "subject_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "subjects_title_key",
                table: "subjects",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "students_id_group_key",
                table: "students",
                columns: new[] { "id", "group_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "roles_permissions_role_id_permission_id_key",
                table: "roles_permissions",
                columns: new[] { "role_id", "permission_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "roles_title_institution_key",
                table: "roles",
                columns: new[] { "institution_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "permissions_description_key",
                table: "permissions",
                column: "description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "lessons_students_lesson_id_student_id_key",
                table: "lessons_students",
                columns: new[] { "lesson_id", "student_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "lessons_course_timetable_date_key",
                table: "lessons",
                columns: new[] { "course_id", "timetable_unit_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "lesson_statuses_description_key",
                table: "lesson_statuses",
                columns: new[] { "description", "institution_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "institutions_title_key",
                table: "institutions",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "institution_members_groups_institution_member_id_group_id_key",
                table: "institution_members_groups",
                columns: new[] { "institution_member_id", "group_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "groups_institution_title_key",
                table: "groups",
                columns: new[] { "institution_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "files_storage_path_key",
                table: "files",
                column: "storage_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "courses_students_course_id_student_id_key",
                table: "courses_students",
                columns: new[] { "course_id", "student_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "courses_title_subject_teacher_institution_term_key",
                table: "courses",
                columns: new[] { "title", "subject_id", "teacher_id", "institution_id", "term_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "classrooms_institution_title_key",
                table: "classrooms",
                columns: new[] { "institution_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "classroom_types_description_institution_id_key",
                table: "classroom_types",
                columns: new[] { "description", "institution_id" },
                unique: true);
        }
    }
}
