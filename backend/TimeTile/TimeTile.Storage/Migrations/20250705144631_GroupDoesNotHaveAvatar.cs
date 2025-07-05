using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class GroupDoesNotHaveAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "groups_avatar_id_fkey",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "IX_groups_avatar_id",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "avatar_id",
                table: "groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "avatar_id",
                table: "groups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_groups_avatar_id",
                table: "groups",
                column: "avatar_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "groups_avatar_id_fkey",
                table: "groups",
                column: "avatar_id",
                principalTable: "files",
                principalColumn: "id");
        }
    }
}
