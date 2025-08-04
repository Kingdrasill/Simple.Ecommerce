using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple.Ecommerce.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddingCouponToOrderAndOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "PedidosItens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "Pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidosItens_CouponId",
                table: "PedidosItens",
                column: "CouponId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_CouponId",
                table: "Pedidos",
                column: "CouponId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Cupons_CouponId",
                table: "Pedidos",
                column: "CouponId",
                principalTable: "Cupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidosItens_Cupons_CouponId",
                table: "PedidosItens",
                column: "CouponId",
                principalTable: "Cupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Cupons_CouponId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidosItens_Cupons_CouponId",
                table: "PedidosItens");

            migrationBuilder.DropIndex(
                name: "IX_PedidosItens_CouponId",
                table: "PedidosItens");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_CouponId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "PedidosItens");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Pedidos");
        }
    }
}
