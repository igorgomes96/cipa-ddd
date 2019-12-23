using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Rename_Processamentos_Templates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateEmails_Contas_ContaId",
                table: "TemplateEmails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateEmails",
                table: "TemplateEmails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessamentoEtapas",
                table: "ProcessamentoEtapas");

            migrationBuilder.RenameTable(
                name: "TemplateEmails",
                newName: "TemplatesEmails");

            migrationBuilder.RenameTable(
                name: "ProcessamentoEtapas",
                newName: "ProcessamentosEtapas");

            migrationBuilder.RenameIndex(
                name: "IX_TemplateEmails_ContaId",
                table: "TemplatesEmails",
                newName: "IX_TemplatesEmails_ContaId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessamentoEtapas_EtapaCronogramaId",
                table: "ProcessamentosEtapas",
                newName: "IX_ProcessamentosEtapas_EtapaCronogramaId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessamentoEtapas_EtapaCronogramaAnteriorId",
                table: "ProcessamentosEtapas",
                newName: "IX_ProcessamentosEtapas_EtapaCronogramaAnteriorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessamentoEtapas_EleicaoId",
                table: "ProcessamentosEtapas",
                newName: "IX_ProcessamentosEtapas_EleicaoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplatesEmails",
                table: "TemplatesEmails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessamentosEtapas",
                table: "ProcessamentosEtapas",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("a8cd15d4-7998-4a2c-a821-9ee655bbe1ae"), new DateTime(2019, 12, 24, 14, 18, 38, 345, DateTimeKind.Local).AddTicks(3482) });

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentosEtapas_Eleicoes_EleicaoId",
                table: "ProcessamentosEtapas",
                column: "EleicaoId",
                principalTable: "Eleicoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentosEtapas_EtapasCronogramas_EtapaCronogramaAnteri~",
                table: "ProcessamentosEtapas",
                column: "EtapaCronogramaAnteriorId",
                principalTable: "EtapasCronogramas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessamentosEtapas_EtapasCronogramas_EtapaCronogramaId",
                table: "ProcessamentosEtapas",
                column: "EtapaCronogramaId",
                principalTable: "EtapasCronogramas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplatesEmails_Contas_ContaId",
                table: "TemplatesEmails",
                column: "ContaId",
                principalTable: "Contas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentosEtapas_Eleicoes_EleicaoId",
                table: "ProcessamentosEtapas");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentosEtapas_EtapasCronogramas_EtapaCronogramaAnteri~",
                table: "ProcessamentosEtapas");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessamentosEtapas_EtapasCronogramas_EtapaCronogramaId",
                table: "ProcessamentosEtapas");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplatesEmails_Contas_ContaId",
                table: "TemplatesEmails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplatesEmails",
                table: "TemplatesEmails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessamentosEtapas",
                table: "ProcessamentosEtapas");

            migrationBuilder.RenameTable(
                name: "TemplatesEmails",
                newName: "TemplateEmails");

            migrationBuilder.RenameTable(
                name: "ProcessamentosEtapas",
                newName: "ProcessamentoEtapas");

            migrationBuilder.RenameIndex(
                name: "IX_TemplatesEmails_ContaId",
                table: "TemplateEmails",
                newName: "IX_TemplateEmails_ContaId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessamentosEtapas_EtapaCronogramaId",
                table: "ProcessamentoEtapas",
                newName: "IX_ProcessamentoEtapas_EtapaCronogramaId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessamentosEtapas_EtapaCronogramaAnteriorId",
                table: "ProcessamentoEtapas",
                newName: "IX_ProcessamentoEtapas_EtapaCronogramaAnteriorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessamentosEtapas_EleicaoId",
                table: "ProcessamentoEtapas",
                newName: "IX_ProcessamentoEtapas_EleicaoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateEmails",
                table: "TemplateEmails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessamentoEtapas",
                table: "ProcessamentoEtapas",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("e66536e5-488e-4faa-aa02-ccfe2798ef33"), new DateTime(2019, 12, 24, 14, 11, 35, 87, DateTimeKind.Local).AddTicks(2120) });

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

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateEmails_Contas_ContaId",
                table: "TemplateEmails",
                column: "ContaId",
                principalTable: "Contas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
