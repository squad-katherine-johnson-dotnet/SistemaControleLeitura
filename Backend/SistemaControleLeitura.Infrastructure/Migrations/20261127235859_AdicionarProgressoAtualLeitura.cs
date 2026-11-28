using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaControleLeitura.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarProgressoAtualLeitura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgressoAtual",
                table: "Leituras",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressoAtual",
                table: "Leituras");
        }
    }
}
