using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AdditionOfCourseToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "courses_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    order_number = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses_users", x => x.id);
                    table.CheckConstraint("CK_CoursesUsers_OrderNumber_Positive", "\"order_number\" > 0");
                    table.ForeignKey(
                        name: "courses_users_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "courses_users_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "courses_users_course_user_deleted_at_key",
                table: "courses_users",
                columns: new[] { "course_id", "user_id", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "courses_users_user_order_number_deleted_at_key",
                table: "courses_users",
                columns: new[] { "user_id", "order_number", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "courses_users");
        }
    }
}
