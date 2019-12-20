using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class ProcessamentoEtapa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessamentoEtapa",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HorarioMudancaEtapa = table.Column<DateTime>(nullable: false),
                    StatusProcessamentoEtapa = table.Column<int>(nullable: false),
                    TerminoProcessamento = table.Column<DateTime>(nullable: true),
                    MensagemErro = table.Column<string>(nullable: true),
                    DataCadastro = table.Column<DateTime>(nullable: false),
                    EtapaCronogramaId = table.Column<int>(nullable: true),
                    EtapaCronogramaAnteriorId = table.Column<int>(nullable: true),
                    EleicaoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessamentoEtapa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessamentoEtapa_Eleicoes_EleicaoId",
                        column: x => x.EleicaoId,
                        principalTable: "Eleicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessamentoEtapa_EtapasCronogramas_EtapaCronogramaAnterior~",
                        column: x => x.EtapaCronogramaAnteriorId,
                        principalTable: "EtapasCronogramas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessamentoEtapa_EtapasCronogramas_EtapaCronogramaId",
                        column: x => x.EtapaCronogramaId,
                        principalTable: "EtapasCronogramas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("16591b8f-ea5e-471a-9be5-b6d266a49f79"), new DateTime(2019, 12, 21, 14, 7, 37, 112, DateTimeKind.Local).AddTicks(6382) });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentoEtapa_EleicaoId",
                table: "ProcessamentoEtapa",
                column: "EleicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentoEtapa_EtapaCronogramaAnteriorId",
                table: "ProcessamentoEtapa",
                column: "EtapaCronogramaAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentoEtapa_EtapaCronogramaId",
                table: "ProcessamentoEtapa",
                column: "EtapaCronogramaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessamentoEtapa");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("f798e45f-0a86-4f87-ba07-c297bb3c3176"), new DateTime(2019, 12, 21, 12, 17, 45, 57, DateTimeKind.Local).AddTicks(5778) });
        }
    }
}
