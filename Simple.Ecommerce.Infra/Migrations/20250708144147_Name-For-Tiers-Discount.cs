using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple.Ecommerce.Infra.Migrations
{
    /// <inheritdoc />
    public partial class NameForTiersDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DescontosTiers",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "DescontosTiers");
        }
    }
}
