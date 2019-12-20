using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class DatafinalizacaoPrevistaEleicao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFinalizacaoPrevista",
                table: "Eleicoes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("f798e45f-0a86-4f87-ba07-c297bb3c3176"), new DateTime(2019, 12, 21, 12, 17, 45, 57, DateTimeKind.Local).AddTicks(5778) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFinalizacaoPrevista",
                table: "Eleicoes");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("309394c4-2adf-4b09-8a1e-c6a4c4adb599"), new DateTime(2019, 12, 19, 17, 0, 44, 620, DateTimeKind.Local).AddTicks(990) });
        }
    }
}
