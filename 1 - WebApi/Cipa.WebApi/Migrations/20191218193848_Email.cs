using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Email : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Copias = table.Column<string>(nullable: true),
                    Assunto = table.Column<string>(maxLength: 255, nullable: false),
                    Mensagem = table.Column<string>(nullable: false),
                    StatusEnvio = table.Column<int>(nullable: false),
                    MensagemErro = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("444bdc06-1a84-42a6-babe-e63682fe2341"), new DateTime(2019, 12, 19, 16, 38, 47, 584, DateTimeKind.Local).AddTicks(9496) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("45a5b620-cdde-426d-90e3-422cd5272e26"), new DateTime(2019, 12, 10, 13, 50, 44, 796, DateTimeKind.Local).AddTicks(370) });
        }
    }
}
