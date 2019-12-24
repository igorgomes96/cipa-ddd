using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Correcao_TemplatesEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 5,
                column: "Template",
                value: @" Ocorreu um erro ao finalizar a etapa atual da eleição da CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ. Por favor, verifique.<br><br>
Mensagem de erro: <strong>@ERRO</strong><br>
Etapa Atual: <strong>@ETAPA_ATUAL</strong><br>
Etapa Posterior: <strong>@ETAPA_POSTERIOR</strong><br><br>
Obs.: A etapa atual deverá ser finalizada manualmente. Para isso, clique no botão ""Próxima Etapa"" do cronograma.");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("56298cf1-3f1c-432f-8894-9d479462b5b3"), new DateTime(2019, 12, 24, 17, 14, 28, 836, DateTimeKind.Local).AddTicks(2908) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 5,
                column: "Template",
                value: @"O cronograma da eleição da CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ, foi atualizado com sucesso!<br><br>
Etapa Anterior: <strong>@ETAPA_ANTERIOR</strong><br>
Etapa Atual: <strong>@ETAPA_ATUAL</strong>");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("dd84f202-ce03-4349-9c70-ec52de51da4d"), new DateTime(2019, 12, 24, 16, 11, 42, 170, DateTimeKind.Local).AddTicks(2617) });
        }
    }
}
