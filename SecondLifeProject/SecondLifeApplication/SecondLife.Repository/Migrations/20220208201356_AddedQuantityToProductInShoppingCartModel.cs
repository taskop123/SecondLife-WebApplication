using Microsoft.EntityFrameworkCore.Migrations;

namespace SecondLife.Repository.Migrations
{
    public partial class AddedQuantityToProductInShoppingCartModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductsInShoppingCarts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductsInShoppingCarts");
        }
    }
}
