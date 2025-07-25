using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple.Ecommerce.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CredentialUsedATAdition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                table: "VerificacaoCredenciais",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedAt",
                table: "VerificacaoCredenciais");
        }
    }
}
