using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaControleLeitura.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterarIdUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Usuarios",
                newName: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Usuarios",
                newName: "Id");
        }
    }
}
