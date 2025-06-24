using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple.Ecommerce.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CardInformationOrdere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardFlag",
                table: "Pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardHolderName",
                table: "Pedidos",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "Pedidos",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpirationMonth",
                table: "Pedidos",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpirationYear",
                table: "Pedidos",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Last4Digits",
                table: "Pedidos",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardFlag",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "CardHolderName",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ExpirationMonth",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ExpirationYear",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Last4Digits",
                table: "Pedidos");
        }
    }
}
