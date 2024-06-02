using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnAPI.Migrations
{
    public partial class QuanityProductCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderQuanity",
                table: "CartProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderQuanity",
                table: "CartProducts");
        }
    }
}
