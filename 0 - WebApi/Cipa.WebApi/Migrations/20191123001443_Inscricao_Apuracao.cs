using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Inscricao_Apuracao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResultadoApuracao",
                table: "Inscricoes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("8b492226-3293-464a-8473-e4f3ff830701"), new DateTime(2019, 11, 23, 21, 14, 43, 199, DateTimeKind.Local).AddTicks(9789) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultadoApuracao",
                table: "Inscricoes");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("d6afe93c-e9ac-406e-9c76-e1cdf045f1a5"), new DateTime(2019, 11, 23, 18, 24, 12, 975, DateTimeKind.Local).AddTicks(7150) });
        }
    }
}
