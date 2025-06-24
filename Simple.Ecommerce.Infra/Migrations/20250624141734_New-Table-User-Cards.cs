using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Simple.Ecommerce.Infra.Migrations
{
    /// <inheritdoc />
    public partial class NewTableUserCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuariosCartoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CardHolderName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CardNumber = table.Column<string>(type: "longtext", nullable: false),
                    ExpirationMonth = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    ExpirationYear = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    CardFlag = table.Column<int>(type: "int", nullable: false),
                    Last4Digits = table.Column<string>(type: "longtext", nullable: false),
                    Deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosCartoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosCartoes_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosCartoes_UserId",
                table: "UsuariosCartoes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuariosCartoes");
        }
    }
}
