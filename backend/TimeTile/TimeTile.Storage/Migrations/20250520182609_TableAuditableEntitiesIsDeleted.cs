using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class TableAuditableEntitiesIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_classroom_types_auditable_entities_id",
                table: "classroom_types");

            migrationBuilder.DropForeignKey(
                name: "FK_classrooms_auditable_entities_id",
                table: "classrooms");

            migrationBuilder.DropForeignKey(
                name: "FK_courses_auditable_entities_id",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_courses_students_auditable_entities_id",
                table: "courses_students");

            migrationBuilder.DropForeignKey(
                name: "FK_files_auditable_entities_id",
                table: "files");

            migrationBuilder.DropForeignKey(
                name: "FK_grades_auditable_entities_id",
                table: "grades");

            migrationBuilder.DropForeignKey(
                name: "FK_groups_auditable_entities_id",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_members_groups_auditable_entities_id",
                table: "institution_members_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_institutions_auditable_entities_id",
                table: "institutions");

            migrationBuilder.DropForeignKey(
                name: "FK_lesson_statuses_auditable_entities_id",
                table: "lesson_statuses");

            migrationBuilder.DropForeignKey(
                name: "FK_lessons_auditable_entities_id",
                table: "lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_lessons_students_auditable_entities_id",
                table: "lessons_students");

            migrationBuilder.DropForeignKey(
                name: "FK_permissions_auditable_entities_id",
                table: "permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_auditable_entities_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_permissions_auditable_entities_id",
                table: "roles_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_subjects_auditable_entities_id",
                table: "subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_teachers_subjects_auditable_entities_id",
                table: "teachers_subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_terms_auditable_entities_id",
                table: "terms");

            migrationBuilder.DropForeignKey(
                name: "FK_timetable_units_auditable_entities_id",
                table: "timetable_units");

            migrationBuilder.DropForeignKey(
                name: "FK_users_auditable_entities_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "auditable_entities");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_BirthDate_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TimetableUnit_StartTime_LessThan_EndTime",
                table: "timetable_units");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Term_StartDate_LessThan_EndDate",
                table: "terms");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_ArgbColor_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_InstitutionMember_WeekWorkHours_Positive",
                table: "institution_members");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Value_Positive",
                table: "grades");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Weight_Positive",
                table: "grades");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_File_Extension_Valid",
                table: "files");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_File_Size_Valid",
                table: "files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionX_Positive",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionY_Positive",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "timetable_units",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "timetable_units",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "timetable_units",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "timetable_units",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "terms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "terms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "terms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "terms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "teachers_subjects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "teachers_subjects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "teachers_subjects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "teachers_subjects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "subjects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "subjects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "subjects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "subjects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "roles_permissions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "roles_permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "roles_permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "roles_permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "roles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "permissions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "lessons_students",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "lessons_students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "lessons_students",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "lessons_students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "lessons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "lessons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "lessons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "lessons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "lesson_statuses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "lesson_statuses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "lesson_statuses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "lesson_statuses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "institutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "institutions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "institutions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "institutions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "institution_members_groups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "institution_members_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "institution_members_groups",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "institution_members_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "groups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "groups",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "grades",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "grades",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "grades",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "grades",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "files",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "files",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "files",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "files",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "courses_students",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "courses_students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "courses_students",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "courses_students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "courses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "courses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "classrooms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "classrooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "classrooms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "classrooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "classroom_types",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "classroom_types",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "classroom_types",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "classroom_types",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_BirthDate_Valid",
                table: "users",
                sql: "\"birth_date\" <= NOW()");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users",
                sql: "\"firstname\" ~ '^[a-zA-Z ,.''-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users",
                sql: "\"home_address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users",
                sql: "\"lastname\" ~ '^[a-zA-Z ,.''-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users",
                sql: "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TimetableUnit_StartTime_LessThan_EndTime",
                table: "timetable_units",
                sql: "\"start_time\" < \"end_time\"");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units",
                sql: "\"title\" ~ '^[\\w ]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Term_StartDate_LessThan_EndDate",
                table: "terms",
                sql: "\"start_date\" < \"end_date\"");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms",
                sql: "\"title\" ~ '^[\\w -.*+,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects",
                sql: "\"title\" ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles",
                sql: "\"title\"  ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students",
                sql: "(\"came_at\" IS NULL AND \"left_at\" IS NULL) OR (\"came_at\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students",
                sql: "(\"came_at\" < \"left_at\") OR (\"left_at\" IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_ArgbColor_Valid",
                table: "lesson_statuses",
                sql: "\"argb_color\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses",
                sql: "\"description\"  ~ '^[a-zA-Z\\d ]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions",
                sql: "\"address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions",
                sql: "\"email\" ~ '^[A-Za-z\\d._%+-]+@[A-Za-z\\d.-]+\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions",
                sql: "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions",
                sql: "\"title\" ~ '^[\\w \\-.*&\"'',\\/\\\\|]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InstitutionMember_WeekWorkHours_Positive",
                table: "institution_members",
                sql: "\"week_work_hours\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups",
                sql: "\"title\"  ~ '^[\\w -.*]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Value_Positive",
                table: "grades",
                sql: "\"value\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Weight_Positive",
                table: "grades",
                sql: "\"weight\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_File_Extension_Valid",
                table: "files",
                sql: "\"extension\" IN ('pdf', 'docx', 'xlsx', 'png', 'jpg', 'jpeg', 'txt', 'zip')");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_File_Size_Valid",
                table: "files",
                sql: "\"size\" > 0");

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

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses",
                sql: "\"title\"  ~ '^[\\w -.*+,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms",
                sql: "\"title\" ~ '^[a-zA-Z \\d-]+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_BirthDate_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TimetableUnit_StartTime_LessThan_EndTime",
                table: "timetable_units");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Term_StartDate_LessThan_EndDate",
                table: "terms");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_ArgbColor_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_InstitutionMember_WeekWorkHours_Positive",
                table: "institution_members");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Value_Positive",
                table: "grades");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Weight_Positive",
                table: "grades");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_File_Extension_Valid",
                table: "files");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_File_Size_Valid",
                table: "files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_HasExam_ExamGrade",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionX_Positive",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionY_Positive",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "timetable_units");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "timetable_units");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "timetable_units");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "terms");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "terms");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "terms");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "teachers_subjects");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "teachers_subjects");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "teachers_subjects");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "roles_permissions");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "roles_permissions");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "roles_permissions");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "lessons_students");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "lessons_students");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "lessons_students");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "lesson_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "lesson_statuses");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "lesson_statuses");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "institutions");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "institutions");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "institutions");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "institution_members_groups");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "institution_members_groups");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "institution_members_groups");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "grades");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "grades");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "grades");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "files");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "files");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "files");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "courses_students");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "courses_students");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "courses_students");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "classrooms");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "classrooms");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "classrooms");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "classroom_types");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "classroom_types");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "classroom_types");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "timetable_units",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "terms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "teachers_subjects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "subjects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "roles_permissions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "roles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "permissions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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
                table: "lessons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "lesson_statuses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "institutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "institution_members_groups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "groups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "grades",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "files",
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

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "courses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "classrooms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "classroom_types",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "auditable_entities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditable_entities", x => x.id);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_BirthDate_Valid",
                table: "users",
                sql: "\"birth_date\" <= NOW()");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users",
                sql: "\"firstname\" ~ '^[a-zA-Z ,.''-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users",
                sql: "\"home_address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users",
                sql: "\"lastname\" ~ '^[a-zA-Z ,.''-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users",
                sql: "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TimetableUnit_StartTime_LessThan_EndTime",
                table: "timetable_units",
                sql: "\"start_time\" < \"end_time\"");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units",
                sql: "\"title\" ~ '^[\\w ]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Term_StartDate_LessThan_EndDate",
                table: "terms",
                sql: "\"start_date\" < \"end_date\"");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms",
                sql: "\"title\" ~ '^[\\w -.*+,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects",
                sql: "\"title\" ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles",
                sql: "\"title\"  ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students",
                sql: "(\"came_at\" IS NULL AND \"left_at\" IS NULL) OR (\"came_at\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students",
                sql: "(\"came_at\" < \"left_at\") OR (\"left_at\" IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_ArgbColor_Valid",
                table: "lesson_statuses",
                sql: "\"argb_color\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses",
                sql: "\"description\"  ~ '^[a-zA-Z\\d ]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions",
                sql: "\"address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions",
                sql: "\"email\" ~ '^[A-Za-z\\d._%+-]+@[A-Za-z\\d.-]+\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions",
                sql: "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions",
                sql: "\"title\" ~ '^[\\w \\-.*&\"'',\\/\\\\|]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InstitutionMember_WeekWorkHours_Positive",
                table: "institution_members",
                sql: "\"week_work_hours\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups",
                sql: "\"title\"  ~ '^[\\w -.*]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Value_Positive",
                table: "grades",
                sql: "\"value\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Weight_Positive",
                table: "grades",
                sql: "\"weight\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_File_Extension_Valid",
                table: "files",
                sql: "\"extension\" IN ('pdf', 'docx', 'xlsx', 'png', 'jpg', 'jpeg', 'txt', 'zip')");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_File_Size_Valid",
                table: "files",
                sql: "\"size\" > 0");

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

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses",
                sql: "\"title\"  ~ '^[\\w -.*+,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms",
                sql: "\"title\" ~ '^[a-zA-Z \\d-]+$'");

            migrationBuilder.AddForeignKey(
                name: "FK_classroom_types_auditable_entities_id",
                table: "classroom_types",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_classrooms_auditable_entities_id",
                table: "classrooms",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_courses_auditable_entities_id",
                table: "courses",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_courses_students_auditable_entities_id",
                table: "courses_students",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_auditable_entities_id",
                table: "files",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_grades_auditable_entities_id",
                table: "grades",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_auditable_entities_id",
                table: "groups",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_members_groups_auditable_entities_id",
                table: "institution_members_groups",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_institutions_auditable_entities_id",
                table: "institutions",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_statuses_auditable_entities_id",
                table: "lesson_statuses",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lessons_auditable_entities_id",
                table: "lessons",
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
                name: "FK_permissions_auditable_entities_id",
                table: "permissions",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_auditable_entities_id",
                table: "roles",
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
                name: "FK_subjects_auditable_entities_id",
                table: "subjects",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teachers_subjects_auditable_entities_id",
                table: "teachers_subjects",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_terms_auditable_entities_id",
                table: "terms",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_timetable_units_auditable_entities_id",
                table: "timetable_units",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_auditable_entities_id",
                table: "users",
                column: "id",
                principalTable: "auditable_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
