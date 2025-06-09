using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class UserHasAvatarIdInsteadOfPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_path",
                table: "users");

            migrationBuilder.AddColumn<int>(
                name: "avatar_id",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_users_avatar_id",
                table: "users",
                column: "avatar_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "users_avatar_id_fkey",
                table: "users",
                column: "avatar_id",
                principalTable: "files",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "users_avatar_id_fkey",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_avatar_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "avatar_id",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "avatar_path",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
