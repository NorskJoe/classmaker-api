using Microsoft.EntityFrameworkCore.Migrations;

namespace classmaker_models.Migrations
{
    public partial class AddStudentClassLockFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LockedInClassroom",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockedInClassroom",
                table: "Students");
        }
    }
}
