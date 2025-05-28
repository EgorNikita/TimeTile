using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CHK_Institution_Domain_Valid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Domain_Valid",
                table: "institutions",
                sql: "\"domain\" ~ '^(?!-)[A-Za-z0-9-]{1,63}(?<!-)(\\.[A-Za-z]{2,})+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Domain_Valid",
                table: "institutions");
        }
    }
}
