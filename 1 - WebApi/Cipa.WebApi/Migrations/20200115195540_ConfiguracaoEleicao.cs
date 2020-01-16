using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class ConfiguracaoEleicao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Estabelecimentos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Empresas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "EnvioConviteInscricao",
                table: "Eleicoes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnvioConviteVotacao",
                table: "Eleicoes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnvioEditalConvocao",
                table: "Eleicoes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("2f108d22-122e-4ca2-b616-52c43723fca6"), new DateTime(2020, 1, 16, 16, 55, 39, 742, DateTimeKind.Local).AddTicks(4132) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnvioConviteInscricao",
                table: "Eleicoes");

            migrationBuilder.DropColumn(
                name: "EnvioConviteVotacao",
                table: "Eleicoes");

            migrationBuilder.DropColumn(
                name: "EnvioEditalConvocao",
                table: "Eleicoes");

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "Ativa", "Cnpj", "ContaId", "DataCadastro", "InformacoesGerais", "RazaoSocial" },
                values: new object[] { 1, true, "01540533000390", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Empresa Teste" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("0fe6cc34-b5cf-4f7e-850c-e527cc6c8464"), new DateTime(2020, 1, 16, 11, 48, 53, 383, DateTimeKind.Local).AddTicks(7045) });

            migrationBuilder.InsertData(
                table: "Estabelecimentos",
                columns: new[] { "Id", "Ativo", "Cidade", "DataCadastro", "Descricao", "EmpresaId", "Endereco", "GrupoId" },
                values: new object[] { 1, true, "Uberlândia", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Teste", 1, "Teste", 1 });
        }
    }
}
