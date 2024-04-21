using Microsoft.EntityFrameworkCore.Migrations;

namespace Play.Inventory.Service.Data.Migrations
{
    public partial class UpdatedCatalogItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatalogItemId",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatalogItemId",
                table: "CatalogItems");
        }
    }
}
