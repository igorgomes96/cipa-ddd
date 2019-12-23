using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Processamento_EtapaAnterior_Nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentosEtapas_EtapasCronogramas_EtapaCronogramaAnteri~",
                table: "ProcessamentosEtapas");

            migrationBuilder.AlterColumn<int>(
                name: "EtapaCronogramaAnteriorId",
                table: "ProcessamentosEtapas",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("2392c60b-088b-45ed-93a0-4da15932e64d"), new DateTime(2019, 12, 24, 14, 47, 40, 615, DateTimeKind.Local).AddTicks(2124) });

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentosEtapas_EtapasCronogramas_EtapaCronogramaAnteri~",
                table: "ProcessamentosEtapas",
                column: "EtapaCronogramaAnteriorId",
                principalTable: "EtapasCronogramas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentosEtapas_EtapasCronogramas_EtapaCronogramaAnteri~",
                table: "ProcessamentosEtapas");

            migrationBuilder.AlterColumn<int>(
                name: "EtapaCronogramaAnteriorId",
                table: "ProcessamentosEtapas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("a8cd15d4-7998-4a2c-a821-9ee655bbe1ae"), new DateTime(2019, 12, 24, 14, 18, 38, 345, DateTimeKind.Local).AddTicks(3482) });

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentosEtapas_EtapasCronogramas_EtapaCronogramaAnteri~",
                table: "ProcessamentosEtapas",
                column: "EtapaCronogramaAnteriorId",
                principalTable: "EtapasCronogramas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
