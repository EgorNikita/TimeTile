using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auditable_entities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditable_entities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "grades",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<float>(type: "real", nullable: false, defaultValue: 1f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grades", x => x.id);
                    table.ForeignKey(
                        name: "FK_grades_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "institutions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institutions", x => x.id);
                    table.ForeignKey(
                        name: "FK_institutions_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lesson_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson_statuses", x => x.id);
                    table.ForeignKey(
                        name: "FK_lesson_statuses_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_permissions_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.id);
                    table.ForeignKey(
                        name: "FK_subjects_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "classrooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false),
                    AuditableEntityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classrooms", x => x.id);
                    table.ForeignKey(
                        name: "FK_classrooms_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "classrooms_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_groups_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "groups_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_roles_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "roles_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "terms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_terms", x => x.id);
                    table.ForeignKey(
                        name: "FK_terms_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "terms_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "timetable_units",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timetable_units", x => x.id);
                    table.ForeignKey(
                        name: "FK_timetable_units_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "timetable_units_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lesson_status_institutions",
                columns: table => new
                {
                    lesson_status_id = table.Column<int>(type: "integer", nullable: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lesson_status_institution_pkey", x => new { x.lesson_status_id, x.institution_id });
                    table.ForeignKey(
                        name: "lesson_status_institution_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lesson_status_institution_lesson_status_id_fkey",
                        column: x => x.lesson_status_id,
                        principalTable: "lesson_statuses",
                        principalColumn: "id");
                });

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

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("role_permissions_pkey", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "role_permissions_permission_id_fkey",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "role_permissions_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    avatar_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    firstname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    lastname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    home_address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    login = table.Column<string>(type: "character varying(263)", maxLength: 263, nullable: false),
                    password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "users_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "users_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.id);
                    table.ForeignKey(
                        name: "FK_students_users_id",
                        column: x => x.id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "students_group_id_fkey",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "teachers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teachers", x => x.id);
                    table.ForeignKey(
                        name: "FK_teachers_users_id",
                        column: x => x.id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_teachers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_class_teachers", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_teachers_teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "teachers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_teachers_teachers_id",
                        column: x => x.id,
                        principalTable: "teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "class_teachers_group_id_fkey",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    subject_id = table.Column<int>(type: "integer", nullable: false),
                    teacher_id = table.Column<int>(type: "integer", nullable: false),
                    is_advanced = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false),
                    term_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.id);
                    table.ForeignKey(
                        name: "FK_courses_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "courses_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "courses_subject_id_fkey",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "courses_teacher_id_fkey",
                        column: x => x.teacher_id,
                        principalTable: "teachers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "courses_term_id_fkey",
                        column: x => x.term_id,
                        principalTable: "terms",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "courses_students",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    exam_grade_id = table.Column<int>(type: "integer", nullable: true),
                    has_exam = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("courses_students_pkey", x => x.id);
                    table.CheckConstraint("CK_CoursesStudents_HasExam_ExamGrade", "\"has_exam\" = FALSE OR \"exam_grade_id\" IS NOT NULL");
                    table.ForeignKey(
                        name: "courses_students_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "courses_students_exam_grade_id_fkey",
                        column: x => x.exam_grade_id,
                        principalTable: "grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "courses_students_student_id_fkey",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    timetable_unit_id = table.Column<int>(type: "integer", nullable: false),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    classroom_id = table.Column<int>(type: "integer", nullable: false),
                    lesson_status_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    homework_description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessons", x => x.id);
                    table.ForeignKey(
                        name: "FK_lessons_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "lessons_classroom_id_fkey",
                        column: x => x.classroom_id,
                        principalTable: "classrooms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lessons_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lessons_lesson_status_id_fkey",
                        column: x => x.lesson_status_id,
                        principalTable: "lesson_statuses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lessons_timetable_unit_id_fkey",
                        column: x => x.timetable_unit_id,
                        principalTable: "timetable_units",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lessons_students",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lesson_id = table.Column<int>(type: "integer", nullable: false),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    came_at = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    left_at = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    classwork_grade_id = table.Column<int>(type: "integer", nullable: true),
                    homework_grade_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lessons_students_pkey", x => x.id);
                    table.ForeignKey(
                        name: "lessons_students_classwork_grade_id_fkey",
                        column: x => x.classwork_grade_id,
                        principalTable: "grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lessons_students_homework_grade_id_fkey",
                        column: x => x.homework_grade_id,
                        principalTable: "grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lessons_students_lesson_id_fkey",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lessons_students_student_id_fkey",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_class_teachers_group_id",
                table: "class_teachers",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_teachers_TeacherId",
                table: "class_teachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "classrooms_institution_title_key",
                table: "classrooms",
                columns: new[] { "institution_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "courses_title_subject_teacher_institution_term_key",
                table: "courses",
                columns: new[] { "title", "subject_id", "teacher_id", "institution_id", "term_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_courses_institution_id",
                table: "courses",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_courses_subject_id",
                table: "courses",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_courses_teacher_id",
                table: "courses",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_courses_term_id",
                table: "courses",
                column: "term_id");

            migrationBuilder.CreateIndex(
                name: "courses_students_course_id_student_id_key",
                table: "courses_students",
                columns: new[] { "course_id", "student_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_courses_students_exam_grade_id",
                table: "courses_students",
                column: "exam_grade_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_courses_students_student_id",
                table: "courses_students",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "groups_institution_title_key",
                table: "groups",
                columns: new[] { "institution_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "institutions_title_key",
                table: "institutions",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lesson_status_institutions_institution_id",
                table: "lesson_status_institutions",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "lesson_statuses_description_key",
                table: "lesson_statuses",
                column: "description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lessons_classroom_id",
                table: "lessons",
                column: "classroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_lesson_status_id",
                table: "lessons",
                column: "lesson_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_timetable_unit_id",
                table: "lessons",
                column: "timetable_unit_id");

            migrationBuilder.CreateIndex(
                name: "lessons_course_timetable_date_key",
                table: "lessons",
                columns: new[] { "course_id", "timetable_unit_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lessons_students_classwork_grade_id",
                table: "lessons_students",
                column: "classwork_grade_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lessons_students_homework_grade_id",
                table: "lessons_students",
                column: "homework_grade_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lessons_students_student_id",
                table: "lessons_students",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "lessons_students_lesson_id_student_id_key",
                table: "lessons_students",
                columns: new[] { "lesson_id", "student_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "permissions_description_key",
                table: "permissions",
                column: "description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "roles_title_institution_key",
                table: "roles",
                columns: new[] { "institution_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_students_GroupId",
                table: "students",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "students_id_group_key",
                table: "students",
                columns: new[] { "id", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "subjects_title_key",
                table: "subjects",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subjects_institutions_subject_id",
                table: "subjects_institutions",
                column: "subject_id");

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
                name: "IX_users_institution_id",
                table: "users",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "users_login_key",
                table: "users",
                column: "login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "class_teachers");

            migrationBuilder.DropTable(
                name: "courses_students");

            migrationBuilder.DropTable(
                name: "lesson_status_institutions");

            migrationBuilder.DropTable(
                name: "lessons_students");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "subjects_institutions");

            migrationBuilder.DropTable(
                name: "grades");

            migrationBuilder.DropTable(
                name: "lessons");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "classrooms");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "lesson_statuses");

            migrationBuilder.DropTable(
                name: "timetable_units");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropTable(
                name: "teachers");

            migrationBuilder.DropTable(
                name: "terms");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "institutions");

            migrationBuilder.DropTable(
                name: "auditable_entities");
        }
    }
}
