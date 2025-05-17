using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTile.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CheckConstraintsWereAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_BirthDate_Valid",
                table: "users",
                sql: "\"birth_date\" <= NOW()");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users",
                sql: "\"firstname\" ~ '^[a-zA-Z ,.''-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users",
                sql: "\"home_address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users",
                sql: "\"lastname\" ~ '^[a-zA-Z ,.''-]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users",
                sql: "\"login\" ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users",
                sql: "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TimetableUnit_StartTime_LessThan_EndTime",
                table: "timetable_units",
                sql: "\"start_time\" < \"end_time\"");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units",
                sql: "\"title\" ~ '^[\\w ]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Term_StartDate_LessThan_EndDate",
                table: "terms",
                sql: "\"start_date\" < \"end_date\"");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms",
                sql: "\"title\" ~ '^[\\w -.*+,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects",
                sql: "\"title\" ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles",
                sql: "\"title\"  ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions",
                sql: "\"description\"  ~ '^[\\w -]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students",
                sql: "(\"came_at\" IS NULL AND \"left_at\" IS NULL) OR (\"came_at\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students",
                sql: "(\"came_at\" < \"left_at\") OR (\"left_at\" IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses",
                sql: "\"description\"  ~ '^[a-zA-Z\\d ]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions",
                sql: "\"address\" ~ '^[A-Za-z\\d''\\.\\- \\,]$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions",
                sql: "\"email\" ~ '^[A-Za-z\\d._%+-]+@[A-Za-z\\d.-]+\\.[A-Za-z]{2,}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions",
                sql: "\"phone_number\" ~ '^(\\+\\d{1,2} )?\\(?\\d{3}\\)?[ .-]\\d{3}[ .-]\\d{4}$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions",
                sql: "\"title\" ~ '^[\\w \\-.*&\"'',\\/\\\\|]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups",
                sql: "\"title\"  ~ '^[\\w -.*]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Value_Positive",
                table: "grades",
                sql: "\"value\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Grade_Weight_Positive",
                table: "grades",
                sql: "\"weight\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses",
                sql: "\"title\"  ~ '^[\\w -.*+,]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms",
                sql: "\"title\" ~ '^[a-zA-Z \\d-]+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_BirthDate_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Firstname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_HomeAddress_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Lastname_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_Login_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_User_PhoneNumber_Valid",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TimetableUnit_StartTime_LessThan_EndTime",
                table: "timetable_units");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TimetableUnit_Title_Valid",
                table: "timetable_units");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Term_StartDate_LessThan_EndDate",
                table: "terms");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Term_Title_Valid",
                table: "terms");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Subject_Title_Valid",
                table: "subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Role_Title_Valid",
                table: "roles");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Permission_Description_Valid",
                table: "permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_IsNotNull_OR_CameAt_LeftAt_IsNull",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonToStudent_CameAt_LessThan_LeftAt",
                table: "lessons_students");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_LessonStatus_Description_Valid",
                table: "lesson_statuses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Address_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Email_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Phone_Valid",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Institution_Title_NotEmpty",
                table: "institutions");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Group_Title_Valid",
                table: "groups");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Value_Positive",
                table: "grades");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Grade_Weight_Positive",
                table: "grades");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Course_Title_Valid",
                table: "courses");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Classroom_Title_Valid",
                table: "classrooms");
        }
    }
}
