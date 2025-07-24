using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionStatusExpiredIsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Submission_Status_Valid",
                table: "submissions");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Submission_Status_Valid",
                table: "submissions",
                sql: "LOWER(\"status\") IN ('notsubmitted', 'submitted', 'submittedlate', 'accepted', 'rejected', 'expired')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Submission_Status_Valid",
                table: "submissions");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Submission_Status_Valid",
                table: "submissions",
                sql: "LOWER(\"status\") IN ('notsubmitted', 'submitted', 'submittedlate', 'accepted', 'rejected')");
        }
    }
}
