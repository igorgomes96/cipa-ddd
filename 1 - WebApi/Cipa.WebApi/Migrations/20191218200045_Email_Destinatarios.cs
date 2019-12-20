using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Email_Destinatarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Destinatarios",
                table: "Emails",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("309394c4-2adf-4b09-8a1e-c6a4c4adb599"), new DateTime(2019, 12, 19, 17, 0, 44, 620, DateTimeKind.Local).AddTicks(990) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destinatarios",
                table: "Emails");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("444bdc06-1a84-42a6-babe-e63682fe2341"), new DateTime(2019, 12, 19, 16, 38, 47, 584, DateTimeKind.Local).AddTicks(9496) });
        }
    }
}
