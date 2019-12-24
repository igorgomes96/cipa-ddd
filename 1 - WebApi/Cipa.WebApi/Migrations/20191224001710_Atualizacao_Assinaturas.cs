using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class Atualizacao_Assinaturas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 1,
                column: "Template",
                value: @"@DATA_COMPLETA, a empresa @EMPRESA_CNPJ, situada na @ENDERECO, através de seu SESMT – Serviço Especializado em Engenharia de Segurança e Medicina do Trabalho - informa a todos os funcionários que na data de hoje tem início o processo de constituição da CIPA – Comissão Interna de Prevenção de Acidentes – de acordo com o item 5.38 da Norma Regulamentadora – 05, aprovada pela portaria nº3. 214 de 08 de Junho de 1978 com alteração da Portaria SIT n.º 247, de 12 de Julho de 2011.<br>
            Todo o processo atenderá ao disposto na legislação citada acima.<br><br>
            As inscrições serão aceitas <strong>@PERIODO_INSCRICAO</strong>.<br>
            A votação será realizada <strong>@PERIODO_VOTACAO</strong>.<br><br>
            Todos os eventos do processo serão comunicados através de e-mails.<br><br>
            Link de Acesso: <a href=""@LINK"">@LINK</a><br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 2,
                column: "Template",
                value: @"A empresa @EMPRESA_CNPJ, situada na @ENDERECO, convida seus funcionários a realizarem suas inscrições para eleição dos membros representantes dos Empregados da Comissão Interna de Prevenção de Acidentes – CIPA – Gestão @GESTAO, de acordo com o item 5.40, alínea a, b e c, da Norma Regulamentadora – Nº 05, aprovada pela Portaria nº 3.214 de 08 de Junho de 1978, com alteração da Portaria SIT n.º 247, de 12 de julho de 2011.<br>
            <strong>As inscrições poderão ser realizadas @PERIODO_INSCRICAO.</strong><br><br>
            Link de Acesso: <a href=""@LINK"">@LINK</a><br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 3,
                column: "Template",
                value: @"Ficam convocados os funcionários da empresa @EMPRESA_CNPJ, situada na @ENDERECO, para a eleição de seus representantes na Comissão Interna de Prevenção de Acidentes - CIPA Gestão @GESTAO, de acordo com a Norma Regulamentadora - NR 5, aprovada pela Portaria nº 3.214 de 8 de junho de 1978, baixada pelo Ministério do Trabalho, a ser realizada em escrutínio secreto @PERIODO_VOTACAO.<br><br>
            Apresentaram-se e estão aptos para serem votados os seguintes candidatos:<br><br>
            @CANDIDATOS<br><br>
            Link de Acesso: <a href=""@LINK"">@LINK</a><br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 4,
                column: "Template",
                value: @"O cronograma da eleição da CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ, foi atualizado com sucesso!<br><br>
            Etapa Anterior: <strong>@ETAPA_ANTERIOR</strong><br>
            Etapa Atual: <strong>@ETAPA_ATUAL</strong>");

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
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 6,
                column: "Template",
                value: @"Parabéns, @CANDIDATO_NOME, sua inscrição foi registrada com sucesso. Agora ela será submetida à aprovação do SESMT e você será
            notificado quando sua inscrição for aprovada ou reprovada.<br><br>
            Confira seus dados abaixo:<br>
            @CANDIDATO_DADOS<br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 7,
                column: "Template",
                value: @"Parabéns, @CANDIDATO_NOME, sua inscrição foi aprovada pelo SESMT.<br>
            Dados da inscrição:<br><br>
            @CANDIDATO_DADOS<br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 8,
                column: "Template",
                value: @"@CANDIDATO_NOME, sua inscrição foi reprovada pelo SESMT. Mas, não se preocupe: verifique o motivo da reprovação e submeta sua inscrição a uma nova aprovação, dentro do prazo de inscrição, que acontece até o dia @FIM_INSCRICAO.<br><br>
            @REPROVACAO_DADOS<br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 9,
                column: "Template",
                value: @"@CANDIDATO_NOME, sua inscrição será novamente submetida à aprovação do SESMT para reanálise e você será notificado quando a mesma for aprovada ou reprovada.<br><br>
            Confira seus dados abaixo:<br>
            @CANDIDATO_DADOS<br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("096c22bd-51f9-4100-a789-1576c85eb4cb"), new DateTime(2019, 12, 24, 21, 17, 9, 585, DateTimeKind.Local).AddTicks(7680) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 1,
                column: "Template",
                value: @"@DATA_COMPLETA, a empresa @EMPRESA_CNPJ, situada na @ENDERECO, através de seu SESMT – Serviço Especializado em Engenharia de Segurança e Medicina do Trabalho - informa a todos os funcionários que na data de hoje tem início o processo de constituição da CIPA – Comissão Interna de Prevenção de Acidentes – de acordo com o item 5.38 da Norma Regulamentadora – 05, aprovada pela portaria nº3. 214 de 08 de Junho de 1978 com alteração da Portaria SIT n.º 247, de 12 de Julho de 2011.<br>
Todo o processo atenderá ao disposto na legislação citada acima.<br><br>
As inscrições serão aceitas <strong>@PERIODO_INSCRICAO</strong>.<br>
A votação será realizada <strong>@PERIODO_VOTACAO</strong>.<br><br>
Todos os eventos do processo serão comunicados através de e-mails.<br><br>
Link de Acesso: <a href=""@LINK"">@LINK</a><br><br><br>
<strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 2,
                column: "Template",
                value: @"A empresa @EMPRESA_CNPJ, situada na @ENDERECO, convida seus funcionários a realizarem suas inscrições para eleição dos membros representantes dos Empregados da Comissão Interna de Prevenção de Acidentes – CIPA – Gestão @GESTAO, de acordo com o item 5.40, alínea a, b e c, da Norma Regulamentadora – Nº 05, aprovada pela Portaria nº 3.214 de 08 de Junho de 1978, com alteração da Portaria SIT n.º 247, de 12 de julho de 2011.<br>
<strong>As inscrições poderão ser realizadas @PERIODO_INSCRICAO.</strong><br><br>
Link de Acesso: <a href=""@LINK"">@LINK</a><br><br><br>
<strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 3,
                column: "Template",
                value: @"Ficam convocados os funcionários da empresa @EMPRESA_CNPJ, situada na @ENDERECO, para a eleição de seus representantes na Comissão Interna de Prevenção de Acidentes - CIPA Gestão @GESTAO, de acordo com a Norma Regulamentadora - NR 5, aprovada pela Portaria nº 3.214 de 8 de junho de 1978, baixada pelo Ministério do Trabalho, a ser realizada em escrutínio secreto @PERIODO_VOTACAO.<br><br>
Apresentaram-se e estão aptos para serem votados os seguintes candidatos:<br><br>
@CANDIDATOS<br><br>
Link de Acesso: <a href=""@LINK"">@LINK</a><br><br>
<strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 4,
                column: "Template",
                value: @"O cronograma da eleição da CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ, foi atualizado com sucesso!<br><br>
Etapa Anterior: <strong>@ETAPA_ANTERIOR</strong><br>
Etapa Atual: <strong>@ETAPA_ATUAL</strong>");

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
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 6,
                column: "Template",
                value: @"Parabéns, @CANDIDATO_NOME, sua inscrição foi registrada com sucesso. Agora ela será submetida à aprovação do SESMT e você será
notificado quando sua inscrição for aprovada ou reprovada.<br><br>
Confira seus dados abaixo:<br>
@CANDIDATO_DADOS<br><br><br>
<strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 7,
                column: "Template",
                value: @"Parabéns, @CANDIDATO_NOME, sua inscrição foi aprovada pelo SESMT.<br>
Dados da inscrição:<br><br>
@CANDIDATO_DADOS<br><br><br>
<strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 8,
                column: "Template",
                value: @"@CANDIDATO_NOME, sua inscrição foi reprovada pelo SESMT. Mas, não se preocupe: verifique o motivo da reprovação e submeta sua inscrição a uma nova aprovação, dentro do prazo de inscrição, que acontece até o dia @FIM_INSCRICAO.<br><br>
@REPROVACAO_DADOS<br><br><br>
<strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO");

            migrationBuilder.UpdateData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 9,
                column: "Template",
                value: @"@CANDIDATO_NOME, sua inscrição será novamente submetida à aprovação do SESMT para reanálise e você será notificado quando a mesma for aprovada ou reprovada.<br><br>
Confira seus dados abaixo:<br>
@CANDIDATO_DADOS<br><br><br>
<strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("56298cf1-3f1c-432f-8894-9d479462b5b3"), new DateTime(2019, 12, 24, 17, 14, 28, 836, DateTimeKind.Local).AddTicks(2908) });
        }
    }
}
