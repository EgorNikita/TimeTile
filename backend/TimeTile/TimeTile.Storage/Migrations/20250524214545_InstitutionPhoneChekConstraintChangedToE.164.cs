using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class InstitutionPhoneChekConstraintChangedToE164 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "users",
                newName: "password_hash");

            migrationBuilder.AddColumn<string>(
                name: "domain",
                table: "institutions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "institutions_domain_deleted_at_constraint",
                table: "institutions",
                columns: new[] { "domain", "deleted_at" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions",
                sql: "\"phone_number\" ~ '^\\+?[1-9]\\d{1,14}$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "institutions_domain_deleted_at_constraint",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions");

            migrationBuilder.DropColumn(
                name: "domain",
                table: "institutions");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "password");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions",
                sql: "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");
        }
    }
}
