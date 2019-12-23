using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class TemplatesEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentoEtapa_Eleicoes_EleicaoId",
                table: "ProcessamentoEtapa");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentoEtapa_EtapasCronogramas_EtapaCronogramaAnterior~",
                table: "ProcessamentoEtapa");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentoEtapa_EtapasCronogramas_EtapaCronogramaId",
                table: "ProcessamentoEtapa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessamentoEtapa",
                table: "ProcessamentoEtapa");

            migrationBuilder.DropIndex(
                name: "IX_ProcessamentoEtapa_EtapaCronogramaAnteriorId",
                table: "ProcessamentoEtapa");

            migrationBuilder.DropIndex(
                name: "IX_ProcessamentoEtapa_EtapaCronogramaId",
                table: "ProcessamentoEtapa");

            migrationBuilder.RenameTable(
                name: "ProcessamentoEtapa",
                newName: "ProcessamentoEtapas");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessamentoEtapa_EleicaoId",
                table: "ProcessamentoEtapas",
                newName: "IX_ProcessamentoEtapas_EleicaoId");

            migrationBuilder.AlterColumn<int>(
                name: "EtapaCronogramaId",
                table: "ProcessamentoEtapas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EtapaCronogramaAnteriorId",
                table: "ProcessamentoEtapas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EleicaoId",
                table: "ProcessamentoEtapas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessamentoEtapas",
                table: "ProcessamentoEtapas",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TemplateEmails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TipoTemplateEmail = table.Column<int>(nullable: false),
                    Assunto = table.Column<string>(maxLength: 255, nullable: false),
                    Template = table.Column<string>(nullable: true),
                    ContaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateEmails_Contas_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("e66536e5-488e-4faa-aa02-ccfe2798ef33"), new DateTime(2019, 12, 24, 14, 11, 35, 87, DateTimeKind.Local).AddTicks(2120) });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentoEtapas_EtapaCronogramaAnteriorId",
                table: "ProcessamentoEtapas",
                column: "EtapaCronogramaAnteriorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentoEtapas_EtapaCronogramaId",
                table: "ProcessamentoEtapas",
                column: "EtapaCronogramaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateEmails_ContaId",
                table: "TemplateEmails",
                column: "ContaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentoEtapas_Eleicoes_EleicaoId",
                table: "ProcessamentoEtapas",
                column: "EleicaoId",
                principalTable: "Eleicoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentoEtapas_EtapasCronogramas_EtapaCronogramaAnterio~",
                table: "ProcessamentoEtapas",
                column: "EtapaCronogramaAnteriorId",
                principalTable: "EtapasCronogramas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentoEtapas_EtapasCronogramas_EtapaCronogramaId",
                table: "ProcessamentoEtapas",
                column: "EtapaCronogramaId",
                principalTable: "EtapasCronogramas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentoEtapas_Eleicoes_EleicaoId",
                table: "ProcessamentoEtapas");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentoEtapas_EtapasCronogramas_EtapaCronogramaAnterio~",
                table: "ProcessamentoEtapas");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentoEtapas_EtapasCronogramas_EtapaCronogramaId",
                table: "ProcessamentoEtapas");

            migrationBuilder.DropTable(
                name: "TemplateEmails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessamentoEtapas",
                table: "ProcessamentoEtapas");

            migrationBuilder.DropIndex(
                name: "IX_ProcessamentoEtapas_EtapaCronogramaAnteriorId",
                table: "ProcessamentoEtapas");

            migrationBuilder.DropIndex(
                name: "IX_ProcessamentoEtapas_EtapaCronogramaId",
                table: "ProcessamentoEtapas");

            migrationBuilder.RenameTable(
                name: "ProcessamentoEtapas",
                newName: "ProcessamentoEtapa");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessamentoEtapas_EleicaoId",
                table: "ProcessamentoEtapa",
                newName: "IX_ProcessamentoEtapa_EleicaoId");

            migrationBuilder.AlterColumn<int>(
                name: "EtapaCronogramaId",
                table: "ProcessamentoEtapa",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "EtapaCronogramaAnteriorId",
                table: "ProcessamentoEtapa",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "EleicaoId",
                table: "ProcessamentoEtapa",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessamentoEtapa",
                table: "ProcessamentoEtapa",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("16591b8f-ea5e-471a-9be5-b6d266a49f79"), new DateTime(2019, 12, 21, 14, 7, 37, 112, DateTimeKind.Local).AddTicks(6382) });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentoEtapa_EtapaCronogramaAnteriorId",
                table: "ProcessamentoEtapa",
                column: "EtapaCronogramaAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentoEtapa_EtapaCronogramaId",
                table: "ProcessamentoEtapa",
                column: "EtapaCronogramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentoEtapa_Eleicoes_EleicaoId",
                table: "ProcessamentoEtapa",
                column: "EleicaoId",
                principalTable: "Eleicoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentoEtapa_EtapasCronogramas_EtapaCronogramaAnterior~",
                table: "ProcessamentoEtapa",
                column: "EtapaCronogramaAnteriorId",
                principalTable: "EtapasCronogramas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentoEtapa_EtapasCronogramas_EtapaCronogramaId",
                table: "ProcessamentoEtapa",
                column: "EtapaCronogramaId",
                principalTable: "EtapasCronogramas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
