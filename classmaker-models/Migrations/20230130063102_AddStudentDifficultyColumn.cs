using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace classmaker_models.Migrations
{
    public partial class AddStudentDifficultyColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DifficultyRating",
                table: "Students",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DifficultyRating",
                table: "Students");
        }
    }
}
