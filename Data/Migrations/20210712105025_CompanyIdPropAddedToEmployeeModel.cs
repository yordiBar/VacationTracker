using Microsoft.EntityFrameworkCore.Migrations;

namespace VacationTracker.Data.Migrations
{
    public partial class CompanyIdPropAddedToEmployeeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Employees");
        }
    }
}
