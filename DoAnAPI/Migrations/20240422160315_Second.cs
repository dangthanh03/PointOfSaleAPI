using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnAPI.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProduct_Invoices_InvoiceId",
                table: "InvoiceProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProduct_Products_ProductId",
                table: "InvoiceProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceProduct",
                table: "InvoiceProduct");

            migrationBuilder.RenameTable(
                name: "InvoiceProduct",
                newName: "InvoiceProducts");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceProduct_ProductId",
                table: "InvoiceProducts",
                newName: "IX_InvoiceProducts_ProductId");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "InvoiceProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceProducts",
                table: "InvoiceProducts",
                columns: new[] { "InvoiceId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProducts_Invoices_InvoiceId",
                table: "InvoiceProducts",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProducts_Products_ProductId",
                table: "InvoiceProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProducts_Invoices_InvoiceId",
                table: "InvoiceProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProducts_Products_ProductId",
                table: "InvoiceProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceProducts",
                table: "InvoiceProducts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InvoiceProducts");

            migrationBuilder.RenameTable(
                name: "InvoiceProducts",
                newName: "InvoiceProduct");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceProducts_ProductId",
                table: "InvoiceProduct",
                newName: "IX_InvoiceProduct_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceProduct",
                table: "InvoiceProduct",
                columns: new[] { "InvoiceId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProduct_Invoices_InvoiceId",
                table: "InvoiceProduct",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProduct_Products_ProductId",
                table: "InvoiceProduct",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
