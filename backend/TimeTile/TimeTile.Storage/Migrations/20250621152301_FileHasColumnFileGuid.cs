using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class FileHasColumnFileGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "file_guid",
                table: "files",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "files_file_guid_key",
                table: "files",
                column: "file_guid",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_File_File_Guid_Valid",
                table: "files",
                sql: "\"storage_path\" LIKE '%' || \"file_guid\"::text || '.%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "files_file_guid_key",
                table: "files");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_File_File_Guid_Valid",
                table: "files");

            migrationBuilder.DropColumn(
                name: "file_guid",
                table: "files");
        }
    }
}
