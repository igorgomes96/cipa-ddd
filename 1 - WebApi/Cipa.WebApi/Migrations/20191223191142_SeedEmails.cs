using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cipa.WebApi.Migrations
{
    public partial class SeedEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TemplatesEmails",
                columns: new[] { "Id", "Assunto", "ContaId", "Template", "TipoTemplateEmail" },
                values: new object[,]
                {
                    { 1, "[CIPA] Edital de Convocação", 1, @"@DATA_COMPLETA, a empresa @EMPRESA_CNPJ, situada na @ENDERECO, através de seu SESMT – Serviço Especializado em Engenharia de Segurança e Medicina do Trabalho - informa a todos os funcionários que na data de hoje tem início o processo de constituição da CIPA – Comissão Interna de Prevenção de Acidentes – de acordo com o item 5.38 da Norma Regulamentadora – 05, aprovada pela portaria nº3. 214 de 08 de Junho de 1978 com alteração da Portaria SIT n.º 247, de 12 de Julho de 2011.<br>
                Todo o processo atenderá ao disposto na legislação citada acima.<br><br>
                As inscrições serão aceitas <strong>@PERIODO_INSCRICAO</strong>.<br>
                A votação será realizada <strong>@PERIODO_VOTACAO</strong>.<br><br>
                Todos os eventos do processo serão comunicados através de e-mails.<br><br>
                Link de Acesso: <a href=""@LINK"">@LINK</a><br><br><br>
                <strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO", 0 },
                    { 2, "[CIPA] Inscrições Abertas", 1, @"A empresa @EMPRESA_CNPJ, situada na @ENDERECO, convida seus funcionários a realizarem suas inscrições para eleição dos membros representantes dos Empregados da Comissão Interna de Prevenção de Acidentes – CIPA – Gestão @GESTAO, de acordo com o item 5.40, alínea a, b e c, da Norma Regulamentadora – Nº 05, aprovada pela Portaria nº 3.214 de 08 de Junho de 1978, com alteração da Portaria SIT n.º 247, de 12 de julho de 2011.<br>
                <strong>As inscrições poderão ser realizadas @PERIODO_INSCRICAO.</strong><br><br>
                Link de Acesso: <a href=""@LINK"">@LINK</a><br><br><br>
                <strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO", 1 },
                    { 3, "[CIPA] Início da Votação", 1, @"Ficam convocados os funcionários da empresa @EMPRESA_CNPJ, situada na @ENDERECO, para a eleição de seus representantes na Comissão Interna de Prevenção de Acidentes - CIPA Gestão @GESTAO, de acordo com a Norma Regulamentadora - NR 5, aprovada pela Portaria nº 3.214 de 8 de junho de 1978, baixada pelo Ministério do Trabalho, a ser realizada em escrutínio secreto @PERIODO_VOTACAO.<br><br>
                Apresentaram-se e estão aptos para serem votados os seguintes candidatos:<br><br>
                @CANDIDATOS<br><br>
                Link de Acesso: <a href=""@LINK"">@LINK</a><br><br>
                <strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO", 2 },
                    { 4, "[CIPA] Mudança de Etapa Realizada com Sucesso", 1, @"O cronograma da eleição da CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ, foi atualizado com sucesso!<br><br>
                Etapa Anterior: <strong>@ETAPA_ANTERIOR</strong><br>
                Etapa Atual: <strong>@ETAPA_ATUAL</strong>", 10 },
                    { 5, "[CIPA] Erro ao Realizar Mudança de Etapa", 1, @"O cronograma da eleição da CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ, foi atualizado com sucesso!<br><br>
                Etapa Anterior: <strong>@ETAPA_ANTERIOR</strong><br>
                Etapa Atual: <strong>@ETAPA_ATUAL</strong>", 9 },
                    { 6, "[CIPA] Inscrição Realizada", 1, @"Parabéns, @CANDIDATO_NOME, sua inscrição foi registrada com sucesso. Agora ela será submetida à aprovação do SESMT e você será
                notificado quando sua inscrição for aprovada ou reprovada.<br><br>
                Confira seus dados abaixo:<br>
                @CANDIDATO_DADOS<br><br><br>
                <strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO", 5 },
                    { 7, "[CIPA] Inscrição Aprovada", 1, @"Parabéns, @CANDIDATO_NOME, sua inscrição foi aprovada pelo SESMT.<br>
                Dados da inscrição:<br><br>
                @CANDIDATO_DADOS<br><br><br>
                <strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO", 6 },
                    { 8, "[CIPA] Inscrição Reprovada", 1, @"@CANDIDATO_NOME, sua inscrição foi reprovada pelo SESMT. Mas, não se preocupe: verifique o motivo da reprovação e submeta sua inscrição a uma nova aprovação, dentro do prazo de inscrição, que acontece até o dia @FIM_INSCRICAO.<br><br>
                @REPROVACAO_DADOS<br><br><br>
                <strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO", 7 },
                    { 9, "[CIPA] Inscrição - Solicitação de Reanálise", 1, @"@CANDIDATO_NOME, sua inscrição será novamente submetida à aprovação do SESMT para reanálise e você será notificado quando a mesma for aprovada ou reprovada.<br><br>
                Confira seus dados abaixo:<br>
                @CANDIDATO_DADOS<br><br><br>
                <strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO", 8 }
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("dd84f202-ce03-4349-9c70-ec52de51da4d"), new DateTime(2019, 12, 24, 16, 11, 42, 170, DateTimeKind.Local).AddTicks(2617) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TemplatesEmails",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodigoRecuperacao", "ExpiracaoCodigoRecuperacao" },
                values: new object[] { new Guid("2392c60b-088b-45ed-93a0-4da15932e64d"), new DateTime(2019, 12, 24, 14, 47, 40, 615, DateTimeKind.Local).AddTicks(2124) });
        }
    }
}
