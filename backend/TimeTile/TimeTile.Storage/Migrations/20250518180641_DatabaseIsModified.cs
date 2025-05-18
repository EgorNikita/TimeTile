using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseIsModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "courses_teacher_id_fkey",
                table: "courses");

            migrationBuilder.DropTable(
                name: "class_teachers");

            migrationBuilder.DropTable(
                name: "lesson_statuses_institutions");

            migrationBuilder.DropTable(
                name: "teachers");

            migrationBuilder.DropIndex(
                name: "lesson_statuses_description_key",
                table: "lesson_statuses");

            migrationBuilder.RenameColumn(
                name: "teacher_id",
                table: "courses",
                newName: "institution_member_id");

            migrationBuilder.RenameIndex(
                name: "IX_courses_teacher_id",
                table: "courses",
                newName: "IX_courses_institution_member_id");

            migrationBuilder.RenameIndex(
                name: "courses_title_subject_teacher_institution_term_key",
                table: "courses",
                newName: "courses_title_subject_institution_member_institution_term_key");

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

            migrationBuilder.AlterColumn<short>(
                name: "value",
                table: "grades",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<short>(
                name: "position_x",
                table: "courses_students",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "position_y",
                table: "courses_students",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "capacity",
                table: "classrooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "classroom_type_id",
                table: "classrooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    original_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    extension = table.Column<string>(type: "text", nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    storage_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.id);
                    table.CheckConstraint("CHK_File_Extension_Valid", "\"extension\" IN ('pdf', 'docx', 'xlsx', 'png', 'jpg', 'jpeg', 'txt', 'zip')");
                    table.CheckConstraint("CHK_File_Size_Valid", "\"size\" > 0");
                    table.ForeignKey(
                        name: "FK_files_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "institution_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    preferred_classroom_id = table.Column<int>(type: "integer", nullable: true),
                    week_work_hours = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution_members", x => x.id);
                    table.CheckConstraint("CK_InstitutionMember_WeekWorkHours_Positive", "\"week_work_hours\" > 0");
                    table.ForeignKey(
                        name: "FK_institution_members_users_id",
                        column: x => x.id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "institution_members_preferred_classroom_id_fkey",
                        column: x => x.preferred_classroom_id,
                        principalTable: "classrooms",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "classroom_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    institution_id = table.Column<int>(type: "integer", nullable: false),
                    icon_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classroom_types", x => x.id);
                    table.ForeignKey(
                        name: "FK_classroom_types_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "classroom_types_icon_id_fkey",
                        column: x => x.icon_id,
                        principalTable: "files",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "classroom_types_institution_id_fkey",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "institution_members_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    institution_member_id = table.Column<int>(type: "integer", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution_members_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_institution_members_groups_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "institution_members_groups_group_id_fkey",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "institution_members_groups_institution_member_id_fkey",
                        column: x => x.institution_member_id,
                        principalTable: "institution_members",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "teachers_subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    teacher_id = table.Column<int>(type: "integer", nullable: false),
                    subject_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teachers_subjects", x => x.id);
                    table.ForeignKey(
                        name: "FK_teachers_subjects_auditable_entities_id",
                        column: x => x.id,
                        principalTable: "auditable_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "teachers_subjects_subject_id_fkey",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "teachers_subjects_teacher_id_fkey",
                        column: x => x.teacher_id,
                        principalTable: "institution_members",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_lesson_statuses_institution_id",
                table: "lesson_statuses",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "lesson_statuses_description_key",
                table: "lesson_statuses",
                columns: new[] { "description", "institution_id" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_ArgbColor_Valid",
                table: "lesson_statuses",
                sql: "\"argb_color\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_PositionX_Positive",
                table: "courses_students",
                sql: "\"position_x\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CoursesStudents_PositionY_Positive",
                table: "courses_students",
                sql: "\"position_y\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_classrooms_classroom_type_id",
                table: "classrooms",
                column: "classroom_type_id");

            migrationBuilder.CreateIndex(
                name: "classroom_types_description_institution_id_key",
                table: "classroom_types",
                columns: new[] { "description", "institution_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_classroom_types_icon_id",
                table: "classroom_types",
                column: "icon_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_classroom_types_institution_id",
                table: "classroom_types",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "files_storage_path_key",
                table: "files",
                column: "storage_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_institution_members_preferred_classroom_id",
                table: "institution_members",
                column: "preferred_classroom_id");

            migrationBuilder.CreateIndex(
                name: "institution_members_groups_institution_member_id_group_id_key",
                table: "institution_members_groups",
                columns: new[] { "institution_member_id", "group_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_institution_members_groups_group_id",
                table: "institution_members_groups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_teachers_subjects_subject_id",
                table: "teachers_subjects",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "teachers_subjects_teacher_id_subject_id_key",
                table: "teachers_subjects",
                columns: new[] { "teacher_id", "subject_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "classrooms_classroom_type_id_fkey",
                table: "classrooms",
                column: "classroom_type_id",
                principalTable: "classroom_types",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "courses_teacher_id_fkey",
                table: "courses",
                column: "institution_member_id",
                principalTable: "institution_members",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "lesson_statuses_institution_id_fkey",
                table: "lesson_statuses",
                column: "institution_id",
                principalTable: "institutions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "classrooms_classroom_type_id_fkey",
                table: "classrooms");

            migrationBuilder.DropForeignKey(
                name: "courses_teacher_id_fkey",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "lesson_statuses_institution_id_fkey",
                table: "lesson_statuses");

            migrationBuilder.DropTable(
                name: "classroom_types");

            migrationBuilder.DropTable(
                name: "institution_members_groups");

            migrationBuilder.DropTable(
                name: "teachers_subjects");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "institution_members");

            migrationBuilder.DropIndex(
                name: "IX_lesson_statuses_institution_id",
                table: "lesson_statuses");

            migrationBuilder.DropIndex(
                name: "lesson_statuses_description_key",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_ArgbColor_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionX_Positive",
                table: "courses_students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CoursesStudents_PositionY_Positive",
                table: "courses_students");

            migrationBuilder.DropIndex(
                name: "IX_classrooms_classroom_type_id",
                table: "classrooms");

            migrationBuilder.DropColumn(
                name: "argb_color",
                table: "lesson_statuses");

            migrationBuilder.DropColumn(
                name: "institution_id",
                table: "lesson_statuses");

            migrationBuilder.DropColumn(
                name: "position_x",
                table: "courses_students");

            migrationBuilder.DropColumn(
                name: "position_y",
                table: "courses_students");

            migrationBuilder.DropColumn(
                name: "capacity",
                table: "classrooms");

            migrationBuilder.DropColumn(
                name: "classroom_type_id",
                table: "classrooms");

            migrationBuilder.RenameColumn(
                name: "institution_member_id",
                table: "courses",
                newName: "teacher_id");

            migrationBuilder.RenameIndex(
                name: "IX_courses_institution_member_id",
                table: "courses",
                newName: "IX_courses_teacher_id");

            migrationBuilder.RenameIndex(
                name: "courses_title_subject_institution_member_institution_term_key",
                table: "courses",
                newName: "courses_title_subject_teacher_institution_term_key");

            migrationBuilder.AlterColumn<int>(
                name: "value",
                table: "grades",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateTable(
                name: "lesson_statuses_institutions",
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
                    group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_class_teachers", x => x.id);
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

            migrationBuilder.CreateIndex(
                name: "lesson_statuses_description_key",
                table: "lesson_statuses",
                column: "description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_class_teachers_group_id",
                table: "class_teachers",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_statuses_institutions_institution_id",
                table: "lesson_statuses_institutions",
                column: "institution_id");

            migrationBuilder.AddForeignKey(
                name: "courses_teacher_id_fkey",
                table: "courses",
                column: "teacher_id",
                principalTable: "teachers",
                principalColumn: "id");
        }
    }
}
