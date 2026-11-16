using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaControleLeitura.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigeDbSetItemEstante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetaAnual_Usuarios_UsuarioId",
                table: "MetaAnual");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetaAnual",
                table: "MetaAnual");

            migrationBuilder.RenameTable(
                name: "MetaAnual",
                newName: "MetasAnuais");

            migrationBuilder.RenameIndex(
                name: "IX_MetaAnual_UsuarioId_Ano",
                table: "MetasAnuais",
                newName: "IX_MetasAnuais_UsuarioId_Ano");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetasAnuais",
                table: "MetasAnuais",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MetasAnuais_Usuarios_UsuarioId",
                table: "MetasAnuais",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetasAnuais_Usuarios_UsuarioId",
                table: "MetasAnuais");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetasAnuais",
                table: "MetasAnuais");

            migrationBuilder.RenameTable(
                name: "MetasAnuais",
                newName: "MetaAnual");

            migrationBuilder.RenameIndex(
                name: "IX_MetasAnuais_UsuarioId_Ano",
                table: "MetaAnual",
                newName: "IX_MetaAnual_UsuarioId_Ano");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetaAnual",
                table: "MetaAnual",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MetaAnual_Usuarios_UsuarioId",
                table: "MetaAnual",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
