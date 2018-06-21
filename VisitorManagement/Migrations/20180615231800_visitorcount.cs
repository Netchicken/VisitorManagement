using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagement.Migrations
{
    public partial class visitorcount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VisitorCount",
                table: "StaffNames",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitorCount",
                table: "StaffNames");
        }
    }
}
