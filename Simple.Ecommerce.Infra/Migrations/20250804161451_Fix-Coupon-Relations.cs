using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple.Ecommerce.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FixCouponRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop foreign keys first
            migrationBuilder.DropForeignKey(
                name: "FK_PedidosItens_Cupons_CouponId",
                table: "PedidosItens");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Cupons_CouponId",
                table: "Pedidos");

            // 2. Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_PedidosItens_CouponId",
                table: "PedidosItens");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_CouponId",
                table: "Pedidos");

            // 3. Recreate non-unique indexes
            migrationBuilder.CreateIndex(
                name: "IX_PedidosItens_CouponId",
                table: "PedidosItens",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_CouponId",
                table: "Pedidos",
                column: "CouponId");

            // 4. Recreate foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_PedidosItens_Cupons_CouponId",
                table: "PedidosItens",
                column: "CouponId",
                principalTable: "Cupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // ou Cascade se for o caso

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Cupons_CouponId",
                table: "Pedidos",
                column: "CouponId",
                principalTable: "Cupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // ou Cascade
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PedidosItens_CouponId",
                table: "PedidosItens");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_CouponId",
                table: "Pedidos");

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
        }
    }
}
