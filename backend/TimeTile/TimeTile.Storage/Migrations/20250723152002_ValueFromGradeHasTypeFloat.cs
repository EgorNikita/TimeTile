using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class ValueFromGradeHasTypeFloat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "value",
                table: "grades",
                type: "real",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "value",
                table: "grades",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
