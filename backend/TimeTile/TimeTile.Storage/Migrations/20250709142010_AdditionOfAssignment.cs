using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AdditionOfAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "homework_description",
                table: "lessons");

            migrationBuilder.AddColumn<int>(
                name: "assignment_id",
                table: "lessons",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "assignments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deadline = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    upload_after_deadline = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignments", x => x.id);
                    table.CheckConstraint("CHK_Assignment_Deadline_Valid", "\"deadline\"  > \"published_at\"");
                    table.CheckConstraint("CHK_Assignment_Description_Valid", "\"description\"  ~ '^[[:alpha:]\\d\\s.,!?]+$'");
                    table.CheckConstraint("CHK_Assignment_Title_Valid", "\"title\"  ~ '^[A-Za-z0-9\\s\\-.,_&()]+$'");
                });

            migrationBuilder.CreateIndex(
                name: "IX_lessons_assignment_id",
                table: "lessons",
                column: "assignment_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "lessons_assignment_id_fkey",
                table: "lessons",
                column: "assignment_id",
                principalTable: "assignments",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lessons_assignment_id_fkey",
                table: "lessons");

            migrationBuilder.DropTable(
                name: "assignments");

            migrationBuilder.DropIndex(
                name: "IX_lessons_assignment_id",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "assignment_id",
                table: "lessons");

            migrationBuilder.AddColumn<string>(
                name: "homework_description",
                table: "lessons",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
