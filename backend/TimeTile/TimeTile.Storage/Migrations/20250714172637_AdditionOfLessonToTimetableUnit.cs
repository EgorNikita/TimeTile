using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AdditionOfLessonToTimetableUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lessons_timetable_unit_id_fkey",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "IX_lessons_timetable_unit_id",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "lessons_course_timetable_date_deleted_at_key",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "timetable_unit_id",
                table: "lessons");

            migrationBuilder.CreateTable(
                name: "lessons_timetable_units",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lesson_id = table.Column<int>(type: "integer", nullable: false),
                    timetable_unit_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessons_timetable_units", x => x.id);
                    table.ForeignKey(
                        name: "lessons_timetable_units_lesson_id_fkey",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lessons_timetable_units_timetable_unit_id_fkey",
                        column: x => x.timetable_unit_id,
                        principalTable: "timetable_units",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_lessons_course_id",
                table: "lessons",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_timetable_units_timetable_unit_id",
                table: "lessons_timetable_units",
                column: "timetable_unit_id");

            migrationBuilder.CreateIndex(
                name: "lessons_timetable_units_lesson_timetable_unit_deleted_at_key",
                table: "lessons_timetable_units",
                columns: new[] { "lesson_id", "timetable_unit_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lessons_timetable_units");

            migrationBuilder.DropIndex(
                name: "IX_lessons_course_id",
                table: "lessons");

            migrationBuilder.AddColumn<int>(
                name: "timetable_unit_id",
                table: "lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_lessons_timetable_unit_id",
                table: "lessons",
                column: "timetable_unit_id");

            migrationBuilder.CreateIndex(
                name: "lessons_course_timetable_date_deleted_at_key",
                table: "lessons",
                columns: new[] { "course_id", "timetable_unit_id", "date", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.AddForeignKey(
                name: "lessons_timetable_unit_id_fkey",
                table: "lessons",
                column: "timetable_unit_id",
                principalTable: "timetable_units",
                principalColumn: "id");
        }
    }
}
