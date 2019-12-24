using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cipa.Infra.Data.Context
{

    public static class ModelBuilderExtensions
    {

        private static string mensagemEditalConvocacao =
            @"@DATA_COMPLETA, a empresa @EMPRESA_CNPJ, situada na @ENDERECO, através de seu SESMT – Serviço Especializado em Engenharia de Segurança e Medicina do Trabalho - informa a todos os funcionários que na data de hoje tem início o processo de constituição da CIPA – Comissão Interna de Prevenção de Acidentes – de acordo com o item 5.38 da Norma Regulamentadora – 05, aprovada pela portaria nº3. 214 de 08 de Junho de 1978 com alteração da Portaria SIT n.º 247, de 12 de Julho de 2011.<br>
            Todo o processo atenderá ao disposto na legislação citada acima.<br><br>
            As inscrições serão aceitas <strong>@PERIODO_INSCRICAO</strong>.<br>
            A votação será realizada <strong>@PERIODO_VOTACAO</strong>.<br><br>
            Todos os eventos do processo serão comunicados através de e-mails.<br><br>
            Link de Acesso: <a href=""@LINK"">@LINK</a><br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>";

        private static string mensagemConviteInscricao =
            @"A empresa @EMPRESA_CNPJ, situada na @ENDERECO, convida seus funcionários a realizarem suas inscrições para eleição dos membros representantes dos Empregados da Comissão Interna de Prevenção de Acidentes – CIPA – Gestão @GESTAO, de acordo com o item 5.40, alínea a, b e c, da Norma Regulamentadora – Nº 05, aprovada pela Portaria nº 3.214 de 08 de Junho de 1978, com alteração da Portaria SIT n.º 247, de 12 de julho de 2011.<br>
            <strong>As inscrições poderão ser realizadas @PERIODO_INSCRICAO.</strong><br><br>
            Link de Acesso: <a href=""@LINK"">@LINK</a><br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>";

        private static string mensagemConviteVotacao =
            @"Ficam convocados os funcionários da empresa @EMPRESA_CNPJ, situada na @ENDERECO, para a eleição de seus representantes na Comissão Interna de Prevenção de Acidentes - CIPA Gestão @GESTAO, de acordo com a Norma Regulamentadora - NR 5, aprovada pela Portaria nº 3.214 de 8 de junho de 1978, baixada pelo Ministério do Trabalho, a ser realizada em escrutínio secreto @PERIODO_VOTACAO.<br><br>
            Apresentaram-se e estão aptos para serem votados os seguintes candidatos:<br><br>
            @CANDIDATOS<br><br>
            Link de Acesso: <a href=""@LINK"">@LINK</a><br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>";

        private static string mensagemMudancaEtapaSucesso = 
            @"O cronograma da eleição da CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ, foi atualizado com sucesso!<br><br>
            Etapa Anterior: <strong>@ETAPA_ANTERIOR</strong><br>
            Etapa Atual: <strong>@ETAPA_ATUAL</strong>";

        private static string mensagemMudancaEtapaErro = 
            @" Ocorreu um erro ao finalizar a etapa atual da eleição da CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ. Por favor, verifique.<br><br>
            Mensagem de erro: <strong>@ERRO</strong><br>
            Etapa Atual: <strong>@ETAPA_ATUAL</strong><br>
            Etapa Posterior: <strong>@ETAPA_POSTERIOR</strong><br><br>
            Obs.: A etapa atual deverá ser finalizada manualmente. Para isso, clique no botão ""Próxima Etapa"" do cronograma.";

        private static string mensagemInscricaoRealizada =
            @"Parabéns, @CANDIDATO_NOME, sua inscrição foi registrada com sucesso. Agora ela será submetida à aprovação do SESMT e você será
            notificado quando sua inscrição for aprovada ou reprovada.<br><br>
            Confira seus dados abaixo:<br>
            @CANDIDATO_DADOS<br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>";

        private static string mensagemInscricaoAprovada = 
            @"Parabéns, @CANDIDATO_NOME, sua inscrição foi aprovada pelo SESMT.<br>
            Dados da inscrição:<br><br>
            @CANDIDATO_DADOS<br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>";

        private static string mensagemInscricaoReprovada = 
            @"@CANDIDATO_NOME, sua inscrição foi reprovada pelo SESMT. Mas, não se preocupe: verifique o motivo da reprovação e submeta sua inscrição a uma nova aprovação, dentro do prazo de inscrição, que acontece até o dia @FIM_INSCRICAO.<br><br>
            @REPROVACAO_DADOS<br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>";

        private static string mensagemReanaliseInscricao = 
            @"@CANDIDATO_NOME, sua inscrição será novamente submetida à aprovação do SESMT para reanálise e você será notificado quando a mesma for aprovada ou reprovada.<br><br>
            Confira seus dados abaixo:<br>
            @CANDIDATO_DADOS<br>
            <div class=""assinatura""><strong>@TECNICO_SESMT</strong><br>@TECNICO_CARGO</div>";

        public static void Seed(this ModelBuilder modelBuilder)
        {
            Conta conta = new Conta
            {
                Id = 1,
                Ativa = true,
                DataInicio = new DateTime(2019, 1, 1),
                DataFim = new DateTime(2020, 12, 31),
                PlanoId = null,
                QtdaEstabelecimentos = 2
            };
            
            TemplateEmail templateEditalConvocao = 
                new TemplateEmail(ETipoTemplateEmail.EditalConvocacao, "[CIPA] Edital de Convocação") {
                    ContaId = 1,
                    Template = mensagemEditalConvocacao,
                    Id = 1
                };

            TemplateEmail templateConviteInscricao = 
                new TemplateEmail(ETipoTemplateEmail.ConviteParaInscricao, "[CIPA] Inscrições Abertas") {
                    ContaId = 1,
                    Template = mensagemConviteInscricao,
                    Id = 2
                };

            TemplateEmail templateConviteVotacao = 
                new TemplateEmail(ETipoTemplateEmail.ConviteParaVotacao, "[CIPA] Início da Votação") {
                    ContaId = 1,
                    Template = mensagemConviteVotacao,
                    Id = 3
                };

            TemplateEmail templateMudancaEtapaSucesso = 
                new TemplateEmail(ETipoTemplateEmail.SucessoMudancaEtapaCronograma, "[CIPA] Mudança de Etapa Realizada com Sucesso") {
                    ContaId = 1,
                    Template = mensagemMudancaEtapaSucesso,
                    Id = 4
                };

            TemplateEmail templateMudancaEtapaErro = 
                new TemplateEmail(ETipoTemplateEmail.ErroMudancaEtapaCronograma, "[CIPA] Erro ao Realizar Mudança de Etapa") {
                    ContaId = 1,
                    Template = mensagemMudancaEtapaErro,
                    Id = 5
                };


            TemplateEmail templateInscricaoRealizada = 
                new TemplateEmail(ETipoTemplateEmail.InscricaoRealizada, "[CIPA] Inscrição Realizada") {
                    ContaId = 1,
                    Template = mensagemInscricaoRealizada,
                    Id = 6
                };

            TemplateEmail templateInscricaoAprovada = 
                new TemplateEmail(ETipoTemplateEmail.InscricaoAprovada, "[CIPA] Inscrição Aprovada") {
                    ContaId = 1,
                    Template = mensagemInscricaoAprovada,
                    Id = 7
                };

            TemplateEmail templateInscricaoReprovada = 
                new TemplateEmail(ETipoTemplateEmail.InscricaoReprovada, "[CIPA] Inscrição Reprovada") {
                    ContaId = 1,
                    Template = mensagemInscricaoReprovada,
                    Id = 8
                };

            TemplateEmail templateReanaliseInscricao = 
                new TemplateEmail(ETipoTemplateEmail.ReanaliseInscricao, "[CIPA] Inscrição - Solicitação de Reanálise") {
                    ContaId = 1,
                    Template = mensagemReanaliseInscricao,
                    Id = 9
                };

            var templatesEmails = new TemplateEmail[9] {
                templateEditalConvocao,
                templateConviteInscricao,
                templateConviteVotacao,
                templateMudancaEtapaSucesso,
                templateMudancaEtapaErro,
                templateInscricaoRealizada,
                templateInscricaoAprovada,
                templateInscricaoReprovada,
                templateReanaliseInscricao
            };

            Grupo[] grupos = new Grupo[45] {
                new Grupo("C-1") { Id = 1 },
                new Grupo("C-1a") { Id = 2 },
                new Grupo("C-2") { Id = 3 },
                new Grupo("C-3") { Id = 4 },
                new Grupo("C-3a") { Id = 5 },
                new Grupo("C-4") { Id = 6 },
                new Grupo("C-5") { Id = 7 },
                new Grupo("C-5a") { Id = 8 },
                new Grupo("C-6") { Id = 9 },
                new Grupo("C-7") { Id = 10 },
                new Grupo("C-7a") { Id = 11 },
                new Grupo("C-8") { Id = 12 },
                new Grupo("C-9") { Id = 13 },
                new Grupo("C-10") { Id = 14 },
                new Grupo("C-11") { Id = 15 },
                new Grupo("C-12") { Id = 16 },
                new Grupo("C-13") { Id = 17 },
                new Grupo("C-14") { Id = 18 },
                new Grupo("C-14a") { Id = 19 },
                new Grupo("C-15") { Id = 20 },
                new Grupo("C-16") { Id = 21 },
                new Grupo("C-17") { Id = 22 },
                new Grupo("C-18") { Id = 23 },
                new Grupo("C-18a") { Id = 24 },
                new Grupo("C-19") { Id = 25 },
                new Grupo("C-20") { Id = 26 },
                new Grupo("C-21") { Id = 27 },
                new Grupo("C-22") { Id = 28 },
                new Grupo("C-23") { Id = 29 },
                new Grupo("C-24") { Id = 30 },
                new Grupo("C-24a") { Id = 31 },
                new Grupo("C-24b") { Id = 32 },
                new Grupo("C-24c") { Id = 33 },
                new Grupo("C-24d") { Id = 34 },
                new Grupo("C-25") { Id = 35 },
                new Grupo("C-26") { Id = 36 },
                new Grupo("C-27") { Id = 37 },
                new Grupo("C-28") { Id = 38 },
                new Grupo("C-29") { Id = 39 },
                new Grupo("C-30") { Id = 40 },
                new Grupo("C-31") { Id = 41 },
                new Grupo("C-32") { Id = 42 },
                new Grupo("C-33") { Id = 43 },
                new Grupo("C-34") { Id = 44 },
                new Grupo("C-35") { Id = 45 }
            };

            LimiteDimensionamento[] limitesDimensionamentos = new LimiteDimensionamento[45] {
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 1 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 2 },
                new LimiteDimensionamento(10000, 2500, 2, 1) { Id = 3 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 4 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 5 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 6 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 7 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 8 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 9 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 10 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 11 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 12 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 13 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 14 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 15 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 16 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 17 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 18 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 19 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 20 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 21 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 22 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 23 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 24 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 25 },
                new LimiteDimensionamento(10000, 2500, 2, 1) { Id = 26 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 27 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 28 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 29 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 30 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 31 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 32 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 33 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 34 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 35 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 36 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 37 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 38 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 39 },
                new LimiteDimensionamento(10000, 2500, 2, 1) { Id = 40 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 41 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 42 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 43 },
                new LimiteDimensionamento(10000, 2500, 2, 2) { Id = 44 },
                new LimiteDimensionamento(10000, 2500, 1, 1) { Id = 45 }
            };

            LinhaDimensionamento[] linhaDimensionamentos = new LinhaDimensionamento[366] {
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 1, GrupoId = 1 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 2, GrupoId = 1 },
                new LinhaDimensionamento(100, 51, 3, 3) { Id = 3, GrupoId = 1 },
                new LinhaDimensionamento(500, 101, 4, 3) { Id = 4, GrupoId = 1 },
                new LinhaDimensionamento(1000, 501, 6, 4) { Id = 5, GrupoId = 1 },
                new LinhaDimensionamento(2500, 1001, 9, 7) { Id = 6, GrupoId = 1 },
                new LinhaDimensionamento(5000, 2501, 12, 9) { Id = 7, GrupoId = 1 },
                new LinhaDimensionamento(10000, 5001, 15, 12) { Id = 8, GrupoId = 1 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 9, GrupoId = 2 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 10, GrupoId = 2 },
                new LinhaDimensionamento(100, 51, 3, 3) { Id = 11, GrupoId = 2 },
                new LinhaDimensionamento(300, 101, 4, 3) { Id = 12, GrupoId = 2 },
                new LinhaDimensionamento(500, 301, 4, 4) { Id = 13, GrupoId = 2 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 14, GrupoId = 2 },
                new LinhaDimensionamento(2500, 1001, 9, 8) { Id = 15, GrupoId = 2 },
                new LinhaDimensionamento(5000, 2501, 12, 9) { Id = 16, GrupoId = 2 },
                new LinhaDimensionamento(10000, 5001, 15, 12) { Id = 17, GrupoId = 2 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 18, GrupoId = 3 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 19, GrupoId = 3 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 20, GrupoId = 3 },
                new LinhaDimensionamento(120, 101, 3, 3) { Id = 21, GrupoId = 3 },
                new LinhaDimensionamento(140, 121, 4, 3) { Id = 22, GrupoId = 3 },
                new LinhaDimensionamento(300, 141, 4, 4) { Id = 23, GrupoId = 3 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 24, GrupoId = 3 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 25, GrupoId = 3 },
                new LinhaDimensionamento(2500, 1001, 7, 6) { Id = 26, GrupoId = 3 },
                new LinhaDimensionamento(5000, 2501, 10, 7) { Id = 27, GrupoId = 3 },
                new LinhaDimensionamento(10000, 5001, 11, 9) { Id = 28, GrupoId = 3 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 29, GrupoId = 4 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 30, GrupoId = 4 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 31, GrupoId = 4 },
                new LinhaDimensionamento(140, 101, 3, 3) { Id = 32, GrupoId = 4 },
                new LinhaDimensionamento(300, 141, 4, 4) { Id = 33, GrupoId = 4 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 34, GrupoId = 4 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 35, GrupoId = 4 },
                new LinhaDimensionamento(2500, 1001, 7, 6) { Id = 36, GrupoId = 4 },
                new LinhaDimensionamento(10000, 2501, 10, 8) { Id = 37, GrupoId = 4 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 38, GrupoId = 5 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 39, GrupoId = 5 },
                new LinhaDimensionamento(300, 101, 2, 2) { Id = 40, GrupoId = 5 },
                new LinhaDimensionamento(1000, 301, 3, 3) { Id = 41, GrupoId = 5 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 42, GrupoId = 5 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 43, GrupoId = 5 },
                new LinhaDimensionamento(10000, 5001, 6, 5) { Id = 44, GrupoId = 5 },
                new LinhaDimensionamento(29, 0, 0, 0) { Id = 45, GrupoId = 6 },
                new LinhaDimensionamento(140, 30, 1, 1) { Id = 46, GrupoId = 6 },
                new LinhaDimensionamento(1000, 141, 2, 2) { Id = 47, GrupoId = 6 },
                new LinhaDimensionamento(2500, 1001, 3, 3) { Id = 48, GrupoId = 6 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 49, GrupoId = 6 },
                new LinhaDimensionamento(10000, 5001, 6, 4) { Id = 50, GrupoId = 6 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 51, GrupoId = 7 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 52, GrupoId = 7 },
                new LinhaDimensionamento(80, 51, 2, 2) { Id = 53, GrupoId = 7 },
                new LinhaDimensionamento(120, 81, 3, 3) { Id = 54, GrupoId = 7 },
                new LinhaDimensionamento(140, 121, 4, 3) { Id = 55, GrupoId = 7 },
                new LinhaDimensionamento(500, 141, 4, 4) { Id = 56, GrupoId = 7 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 57, GrupoId = 7 },
                new LinhaDimensionamento(5000, 1001, 9, 7) { Id = 58, GrupoId = 7 },
                new LinhaDimensionamento(10000, 5001, 11, 9) { Id = 59, GrupoId = 7 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 60, GrupoId = 8 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 61, GrupoId = 8 },
                new LinhaDimensionamento(300, 101, 2, 2) { Id = 62, GrupoId = 8 },
                new LinhaDimensionamento(1000, 301, 3, 3) { Id = 63, GrupoId = 8 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 64, GrupoId = 8 },
                new LinhaDimensionamento(5000, 2501, 6, 4) { Id = 65, GrupoId = 8 },
                new LinhaDimensionamento(10000, 5001, 7, 5) { Id = 66, GrupoId = 8 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 67, GrupoId = 9 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 68, GrupoId = 9 },
                new LinhaDimensionamento(80, 51, 2, 2) { Id = 69, GrupoId = 9 },
                new LinhaDimensionamento(120, 81, 3, 3) { Id = 70, GrupoId = 9 },
                new LinhaDimensionamento(140, 121, 4, 3) { Id = 71, GrupoId = 9 },
                new LinhaDimensionamento(500, 141, 5, 4) { Id = 72, GrupoId = 9 },
                new LinhaDimensionamento(1000, 501, 6, 4) { Id = 73, GrupoId = 9 },
                new LinhaDimensionamento(2500, 1001, 8, 6) { Id = 74, GrupoId = 9 },
                new LinhaDimensionamento(5000, 2501, 10, 8) { Id = 75, GrupoId = 9 },
                new LinhaDimensionamento(10000, 5001, 12, 10) { Id = 76, GrupoId = 9 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 77, GrupoId = 10 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 78, GrupoId = 10 },
                new LinhaDimensionamento(500, 101, 2, 2) { Id = 79, GrupoId = 10 },
                new LinhaDimensionamento(1000, 501, 3, 3) { Id = 80, GrupoId = 10 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 81, GrupoId = 10 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 82, GrupoId = 10 },
                new LinhaDimensionamento(10000, 5001, 6, 4) { Id = 83, GrupoId = 10 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 84, GrupoId = 11 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 85, GrupoId = 11 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 86, GrupoId = 11 },
                new LinhaDimensionamento(140, 101, 3, 3) { Id = 87, GrupoId = 11 },
                new LinhaDimensionamento(300, 141, 4, 3) { Id = 88, GrupoId = 11 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 89, GrupoId = 11 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 90, GrupoId = 11 },
                new LinhaDimensionamento(2500, 1001, 8, 7) { Id = 91, GrupoId = 11 },
                new LinhaDimensionamento(5000, 2501, 9, 8) { Id = 92, GrupoId = 11 },
                new LinhaDimensionamento(10000, 5001, 10, 8) { Id = 93, GrupoId = 11 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 94, GrupoId = 12 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 95, GrupoId = 12 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 96, GrupoId = 12 },
                new LinhaDimensionamento(140, 101, 3, 3) { Id = 97, GrupoId = 12 },
                new LinhaDimensionamento(300, 141, 4, 3) { Id = 98, GrupoId = 12 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 99, GrupoId = 12 },
                new LinhaDimensionamento(1000, 501, 6, 4) { Id = 100, GrupoId = 12 },
                new LinhaDimensionamento(2500, 1001, 7, 5) { Id = 101, GrupoId = 12 },
                new LinhaDimensionamento(5000, 2501, 8, 6) { Id = 102, GrupoId = 12 },
                new LinhaDimensionamento(10000, 5001, 10, 8) { Id = 103, GrupoId = 12 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 104, GrupoId = 13 },
                new LinhaDimensionamento(120, 51, 1, 1) { Id = 105, GrupoId = 13 },
                new LinhaDimensionamento(500, 121, 2, 2) { Id = 106, GrupoId = 13 },
                new LinhaDimensionamento(1000, 501, 3, 3) { Id = 107, GrupoId = 13 },
                new LinhaDimensionamento(2500, 1001, 5, 4) { Id = 108, GrupoId = 13 },
                new LinhaDimensionamento(5000, 2501, 6, 4) { Id = 109, GrupoId = 13 },
                new LinhaDimensionamento(10000, 5001, 7, 5) { Id = 110, GrupoId = 13 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 111, GrupoId = 14 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 112, GrupoId = 14 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 113, GrupoId = 14 },
                new LinhaDimensionamento(140, 101, 3, 3) { Id = 114, GrupoId = 14 },
                new LinhaDimensionamento(300, 141, 4, 3) { Id = 115, GrupoId = 14 },
                new LinhaDimensionamento(500, 301, 4, 4) { Id = 116, GrupoId = 14 },
                new LinhaDimensionamento(1000, 501, 5, 4) { Id = 117, GrupoId = 14 },
                new LinhaDimensionamento(2500, 1001, 8, 6) { Id = 118, GrupoId = 14 },
                new LinhaDimensionamento(5000, 2501, 9, 7) { Id = 119, GrupoId = 14 },
                new LinhaDimensionamento(10000, 5001, 10, 8) { Id = 120, GrupoId = 14 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 121, GrupoId = 15 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 122, GrupoId = 15 },
                new LinhaDimensionamento(80, 51, 2, 2) { Id = 123, GrupoId = 15 },
                new LinhaDimensionamento(120, 81, 3, 3) { Id = 124, GrupoId = 15 },
                new LinhaDimensionamento(300, 121, 4, 3) { Id = 125, GrupoId = 15 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 126, GrupoId = 15 },
                new LinhaDimensionamento(1000, 501, 6, 4) { Id = 127, GrupoId = 15 },
                new LinhaDimensionamento(2500, 1001, 9, 7) { Id = 128, GrupoId = 15 },
                new LinhaDimensionamento(5000, 2501, 10, 8) { Id = 129, GrupoId = 15 },
                new LinhaDimensionamento(10000, 5001, 12, 10) { Id = 130, GrupoId = 15 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 131, GrupoId = 16 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 132, GrupoId = 16 },
                new LinhaDimensionamento(80, 51, 2, 2) { Id = 133, GrupoId = 16 },
                new LinhaDimensionamento(120, 81, 3, 3) { Id = 134, GrupoId = 16 },
                new LinhaDimensionamento(300, 121, 4, 3) { Id = 135, GrupoId = 16 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 136, GrupoId = 16 },
                new LinhaDimensionamento(1000, 501, 7, 6) { Id = 137, GrupoId = 16 },
                new LinhaDimensionamento(2500, 1001, 8, 6) { Id = 138, GrupoId = 16 },
                new LinhaDimensionamento(5000, 2501, 9, 7) { Id = 139, GrupoId = 16 },
                new LinhaDimensionamento(10000, 5001, 10, 8) { Id = 140, GrupoId = 16 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 141, GrupoId = 17 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 142, GrupoId = 17 },
                new LinhaDimensionamento(140, 51, 3, 3) { Id = 143, GrupoId = 17 },
                new LinhaDimensionamento(300, 141, 4, 3) { Id = 144, GrupoId = 17 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 145, GrupoId = 17 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 146, GrupoId = 17 },
                new LinhaDimensionamento(2500, 1001, 9, 7) { Id = 147, GrupoId = 17 },
                new LinhaDimensionamento(5000, 2501, 11, 8) { Id = 148, GrupoId = 17 },
                new LinhaDimensionamento(10000, 5001, 13, 10) { Id = 149, GrupoId = 17 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 150, GrupoId = 18 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 151, GrupoId = 18 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 152, GrupoId = 18 },
                new LinhaDimensionamento(120, 101, 3, 3) { Id = 153, GrupoId = 18 },
                new LinhaDimensionamento(140, 121, 4, 3) { Id = 154, GrupoId = 18 },
                new LinhaDimensionamento(300, 141, 4, 4) { Id = 155, GrupoId = 18 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 156, GrupoId = 18 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 157, GrupoId = 18 },
                new LinhaDimensionamento(2500, 1001, 9, 7) { Id = 158, GrupoId = 18 },
                new LinhaDimensionamento(10000, 2501, 11, 9) { Id = 159, GrupoId = 18 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 160, GrupoId = 19 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 161, GrupoId = 19 },
                new LinhaDimensionamento(300, 101, 2, 2) { Id = 162, GrupoId = 19 },
                new LinhaDimensionamento(1000, 301, 3, 3) { Id = 163, GrupoId = 19 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 164, GrupoId = 19 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 165, GrupoId = 19 },
                new LinhaDimensionamento(10000, 5001, 6, 4) { Id = 166, GrupoId = 19 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 167, GrupoId = 20 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 168, GrupoId = 20 },
                new LinhaDimensionamento(100, 51, 3, 3) { Id = 169, GrupoId = 20 },
                new LinhaDimensionamento(300, 101, 4, 3) { Id = 170, GrupoId = 20 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 171, GrupoId = 20 },
                new LinhaDimensionamento(1000, 501, 6, 4) { Id = 172, GrupoId = 20 },
                new LinhaDimensionamento(2500, 1001, 8, 6) { Id = 173, GrupoId = 20 },
                new LinhaDimensionamento(5000, 2501, 10, 8) { Id = 174, GrupoId = 20 },
                new LinhaDimensionamento(10000, 5001, 12, 10) { Id = 175, GrupoId = 20 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 176, GrupoId = 21 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 177, GrupoId = 21 },
                new LinhaDimensionamento(80, 51, 2, 2) { Id = 178, GrupoId = 21 },
                new LinhaDimensionamento(140, 81, 3, 3) { Id = 179, GrupoId = 21 },
                new LinhaDimensionamento(300, 141, 4, 3) { Id = 180, GrupoId = 21 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 181, GrupoId = 21 },
                new LinhaDimensionamento(1000, 501, 6, 4) { Id = 182, GrupoId = 21 },
                new LinhaDimensionamento(2500, 1001, 8, 6) { Id = 183, GrupoId = 21 },
                new LinhaDimensionamento(5000, 2501, 10, 7) { Id = 184, GrupoId = 21 },
                new LinhaDimensionamento(10000, 5001, 12, 9) { Id = 185, GrupoId = 21 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 186, GrupoId = 22 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 187, GrupoId = 22 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 188, GrupoId = 22 },
                new LinhaDimensionamento(300, 101, 4, 3) { Id = 189, GrupoId = 22 },
                new LinhaDimensionamento(500, 301, 4, 4) { Id = 190, GrupoId = 22 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 191, GrupoId = 22 },
                new LinhaDimensionamento(2500, 1001, 8, 7) { Id = 192, GrupoId = 22 },
                new LinhaDimensionamento(5000, 2501, 10, 8) { Id = 193, GrupoId = 22 },
                new LinhaDimensionamento(10000, 5001, 12, 10) { Id = 194, GrupoId = 22 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 195, GrupoId = 23 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 196, GrupoId = 23 },
                new LinhaDimensionamento(300, 101, 4, 3) { Id = 197, GrupoId = 23 },
                new LinhaDimensionamento(500, 301, 4, 4) { Id = 198, GrupoId = 23 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 199, GrupoId = 23 },
                new LinhaDimensionamento(2500, 1001, 8, 7) { Id = 200, GrupoId = 23 },
                new LinhaDimensionamento(5000, 2501, 10, 8) { Id = 201, GrupoId = 23 },
                new LinhaDimensionamento(10000, 5001, 12, 10) { Id = 202, GrupoId = 23 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 203, GrupoId = 24 },
                new LinhaDimensionamento(100, 51, 3, 3) { Id = 204, GrupoId = 24 },
                new LinhaDimensionamento(300, 101, 4, 3) { Id = 205, GrupoId = 24 },
                new LinhaDimensionamento(500, 301, 4, 4) { Id = 206, GrupoId = 24 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 207, GrupoId = 24 },
                new LinhaDimensionamento(2500, 1001, 9, 7) { Id = 208, GrupoId = 24 },
                new LinhaDimensionamento(5000, 2501, 12, 9) { Id = 209, GrupoId = 24 },
                new LinhaDimensionamento(10000, 5001, 15, 12) { Id = 210, GrupoId = 24 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 211, GrupoId = 25 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 212, GrupoId = 25 },
                new LinhaDimensionamento(300, 101, 2, 2) { Id = 213, GrupoId = 25 },
                new LinhaDimensionamento(1000, 301, 3, 3) { Id = 214, GrupoId = 25 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 215, GrupoId = 25 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 216, GrupoId = 25 },
                new LinhaDimensionamento(10000, 5001, 6, 4) { Id = 217, GrupoId = 25 },
                new LinhaDimensionamento(29, 0, 0, 0) { Id = 218, GrupoId = 26 },
                new LinhaDimensionamento(80, 30, 1, 1) { Id = 219, GrupoId = 26 },
                new LinhaDimensionamento(300, 81, 3, 3) { Id = 220, GrupoId = 26 },
                new LinhaDimensionamento(500, 301, 4, 3) { Id = 221, GrupoId = 26 },
                new LinhaDimensionamento(2500, 501, 5, 4) { Id = 222, GrupoId = 26 },
                new LinhaDimensionamento(5000, 2501, 6, 5) { Id = 223, GrupoId = 26 },
                new LinhaDimensionamento(10000, 5001, 8, 6) { Id = 224, GrupoId = 26 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 225, GrupoId = 27 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 226, GrupoId = 27 },
                new LinhaDimensionamento(300, 101, 2, 2) { Id = 227, GrupoId = 27 },
                new LinhaDimensionamento(1000, 301, 3, 3) { Id = 228, GrupoId = 27 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 229, GrupoId = 27 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 230, GrupoId = 27 },
                new LinhaDimensionamento(10000, 5001, 6, 5) { Id = 231, GrupoId = 27 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 232, GrupoId = 28 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 233, GrupoId = 28 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 234, GrupoId = 28 },
                new LinhaDimensionamento(140, 101, 3, 3) { Id = 235, GrupoId = 28 },
                new LinhaDimensionamento(500, 141, 4, 3) { Id = 236, GrupoId = 28 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 237, GrupoId = 28 },
                new LinhaDimensionamento(2500, 1001, 8, 6) { Id = 238, GrupoId = 28 },
                new LinhaDimensionamento(5000, 2501, 10, 8) { Id = 239, GrupoId = 28 },
                new LinhaDimensionamento(10000, 5001, 12, 9) { Id = 240, GrupoId = 28 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 241, GrupoId = 29 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 242, GrupoId = 29 },
                new LinhaDimensionamento(500, 101, 2, 2) { Id = 243, GrupoId = 29 },
                new LinhaDimensionamento(1000, 501, 3, 3) { Id = 244, GrupoId = 29 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 245, GrupoId = 29 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 246, GrupoId = 29 },
                new LinhaDimensionamento(10000, 5001, 6, 5) { Id = 247, GrupoId = 29 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 248, GrupoId = 30 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 249, GrupoId = 30 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 250, GrupoId = 30 },
                new LinhaDimensionamento(140, 101, 4, 3) { Id = 251, GrupoId = 30 },
                new LinhaDimensionamento(500, 141, 4, 4) { Id = 252, GrupoId = 30 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 253, GrupoId = 30 },
                new LinhaDimensionamento(2500, 1001, 8, 7) { Id = 254, GrupoId = 30 },
                new LinhaDimensionamento(5000, 2501, 10, 8) { Id = 255, GrupoId = 30 },
                new LinhaDimensionamento(10000, 5001, 12, 10) { Id = 256, GrupoId = 30 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 257, GrupoId = 31 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 258, GrupoId = 31 },
                new LinhaDimensionamento(500, 101, 2, 2) { Id = 259, GrupoId = 31 },
                new LinhaDimensionamento(1000, 501, 3, 3) { Id = 260, GrupoId = 31 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 261, GrupoId = 31 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 262, GrupoId = 31 },
                new LinhaDimensionamento(10000, 5001, 6, 4) { Id = 263, GrupoId = 31 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 264, GrupoId = 32 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 265, GrupoId = 32 },
                new LinhaDimensionamento(100, 51, 3, 3) { Id = 266, GrupoId = 32 },
                new LinhaDimensionamento(500, 101, 4, 3) { Id = 267, GrupoId = 32 },
                new LinhaDimensionamento(1000, 501, 6, 4) { Id = 268, GrupoId = 32 },
                new LinhaDimensionamento(2500, 1001, 9, 7) { Id = 269, GrupoId = 32 },
                new LinhaDimensionamento(5000, 2501, 12, 9) { Id = 270, GrupoId = 32 },
                new LinhaDimensionamento(10000, 5001, 15, 12) { Id = 271, GrupoId = 32 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 272, GrupoId = 33 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 273, GrupoId = 33 },
                new LinhaDimensionamento(140, 101, 2, 1) { Id = 274, GrupoId = 33 },
                new LinhaDimensionamento(500, 141, 2, 2) { Id = 275, GrupoId = 33 },
                new LinhaDimensionamento(1000, 501, 4, 4) { Id = 276, GrupoId = 33 },
                new LinhaDimensionamento(2500, 1001, 5, 5) { Id = 277, GrupoId = 33 },
                new LinhaDimensionamento(10000, 2501, 7, 7) { Id = 278, GrupoId = 33 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 279, GrupoId = 34 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 280, GrupoId = 34 },
                new LinhaDimensionamento(140, 101, 2, 1) { Id = 281, GrupoId = 34 },
                new LinhaDimensionamento(300, 141, 2, 2) { Id = 282, GrupoId = 34 },
                new LinhaDimensionamento(500, 301, 3, 2) { Id = 283, GrupoId = 34 },
                new LinhaDimensionamento(1000, 501, 4, 4) { Id = 284, GrupoId = 34 },
                new LinhaDimensionamento(2500, 1001, 5, 5) { Id = 285, GrupoId = 34 },
                new LinhaDimensionamento(5000, 2501, 7, 7) { Id = 286, GrupoId = 34 },
                new LinhaDimensionamento(10000, 5001, 9, 9) { Id = 287, GrupoId = 34 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 288, GrupoId = 35 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 289, GrupoId = 35 },
                new LinhaDimensionamento(500, 101, 2, 2) { Id = 290, GrupoId = 35 },
                new LinhaDimensionamento(1000, 501, 3, 3) { Id = 291, GrupoId = 35 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 292, GrupoId = 35 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 293, GrupoId = 35 },
                new LinhaDimensionamento(10000, 5001, 6, 5) { Id = 294, GrupoId = 35 },
                new LinhaDimensionamento(300, 0, 0, 0) { Id = 295, GrupoId = 36 },
                new LinhaDimensionamento(500, 301, 1, 1) { Id = 296, GrupoId = 36 },
                new LinhaDimensionamento(1000, 501, 2, 2) { Id = 297, GrupoId = 36 },
                new LinhaDimensionamento(2500, 1001, 3, 3) { Id = 298, GrupoId = 36 },
                new LinhaDimensionamento(5000, 2501, 4, 3) { Id = 299, GrupoId = 36 },
                new LinhaDimensionamento(10000, 5001, 5, 4) { Id = 300, GrupoId = 36 },
                new LinhaDimensionamento(100, 0, 0, 0) { Id = 301, GrupoId = 37 },
                new LinhaDimensionamento(140, 101, 1, 1) { Id = 302, GrupoId = 37 },
                new LinhaDimensionamento(300, 141, 2, 2) { Id = 303, GrupoId = 37 },
                new LinhaDimensionamento(500, 301, 3, 3) { Id = 304, GrupoId = 37 },
                new LinhaDimensionamento(1000, 501, 4, 3) { Id = 305, GrupoId = 37 },
                new LinhaDimensionamento(2500, 1001, 5, 4) { Id = 306, GrupoId = 37 },
                new LinhaDimensionamento(10000, 2501, 6, 5) { Id = 307, GrupoId = 37 },
                new LinhaDimensionamento(100, 0, 0, 0) { Id = 308, GrupoId = 38 },
                new LinhaDimensionamento(140, 101, 1, 1) { Id = 309, GrupoId = 38 },
                new LinhaDimensionamento(300, 141, 2, 2) { Id = 310, GrupoId = 38 },
                new LinhaDimensionamento(500, 301, 3, 3) { Id = 311, GrupoId = 38 },
                new LinhaDimensionamento(1000, 501, 4, 4) { Id = 312, GrupoId = 38 },
                new LinhaDimensionamento(2500, 1001, 5, 5) { Id = 313, GrupoId = 38 },
                new LinhaDimensionamento(10000, 2501, 6, 5) { Id = 314, GrupoId = 38 },
                new LinhaDimensionamento(300, 0, 0, 0) { Id = 315, GrupoId = 39 },
                new LinhaDimensionamento(500, 301, 1, 1) { Id = 316, GrupoId = 39 },
                new LinhaDimensionamento(1000, 501, 2, 2) { Id = 317, GrupoId = 39 },
                new LinhaDimensionamento(2500, 1001, 3, 3) { Id = 318, GrupoId = 39 },
                new LinhaDimensionamento(5000, 2501, 4, 3) { Id = 319, GrupoId = 39 },
                new LinhaDimensionamento(10000, 5001, 5, 4) { Id = 320, GrupoId = 39 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 321, GrupoId = 40 },
                new LinhaDimensionamento(80, 20, 1, 1) { Id = 322, GrupoId = 40 },
                new LinhaDimensionamento(100, 81, 2, 2) { Id = 323, GrupoId = 40 },
                new LinhaDimensionamento(140, 101, 4, 3) { Id = 324, GrupoId = 40 },
                new LinhaDimensionamento(300, 141, 4, 4) { Id = 325, GrupoId = 40 },
                new LinhaDimensionamento(500, 301, 5, 4) { Id = 326, GrupoId = 40 },
                new LinhaDimensionamento(1000, 501, 7, 6) { Id = 327, GrupoId = 40 },
                new LinhaDimensionamento(2500, 1001, 8, 7) { Id = 328, GrupoId = 40 },
                new LinhaDimensionamento(5000, 2501, 9, 8) { Id = 329, GrupoId = 40 },
                new LinhaDimensionamento(10000, 5001, 10, 9) { Id = 330, GrupoId = 40 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 331, GrupoId = 41 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 332, GrupoId = 41 },
                new LinhaDimensionamento(300, 101, 2, 2) { Id = 333, GrupoId = 41 },
                new LinhaDimensionamento(1000, 301, 3, 3) { Id = 334, GrupoId = 41 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 335, GrupoId = 41 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 336, GrupoId = 41 },
                new LinhaDimensionamento(10000, 5001, 6, 5) { Id = 337, GrupoId = 41 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 338, GrupoId = 42 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 339, GrupoId = 42 },
                new LinhaDimensionamento(300, 101, 2, 2) { Id = 340, GrupoId = 42 },
                new LinhaDimensionamento(1000, 301, 3, 3) { Id = 341, GrupoId = 42 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 342, GrupoId = 42 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 343, GrupoId = 42 },
                new LinhaDimensionamento(10000, 5001, 6, 5) { Id = 344, GrupoId = 42 },
                new LinhaDimensionamento(100, 0, 0, 0) { Id = 345, GrupoId = 43 },
                new LinhaDimensionamento(500, 101, 1, 1) { Id = 346, GrupoId = 43 },
                new LinhaDimensionamento(1000, 501, 2, 2) { Id = 347, GrupoId = 43 },
                new LinhaDimensionamento(2500, 1001, 3, 3) { Id = 348, GrupoId = 43 },
                new LinhaDimensionamento(5000, 2501, 4, 3) { Id = 349, GrupoId = 43 },
                new LinhaDimensionamento(10000, 5001, 5, 4) { Id = 350, GrupoId = 43 },
                new LinhaDimensionamento(19, 0, 0, 0) { Id = 351, GrupoId = 44 },
                new LinhaDimensionamento(50, 20, 1, 1) { Id = 352, GrupoId = 44 },
                new LinhaDimensionamento(100, 51, 2, 2) { Id = 353, GrupoId = 44 },
                new LinhaDimensionamento(300, 101, 4, 3) { Id = 354, GrupoId = 44 },
                new LinhaDimensionamento(500, 301, 4, 4) { Id = 355, GrupoId = 44 },
                new LinhaDimensionamento(1000, 501, 6, 5) { Id = 356, GrupoId = 44 },
                new LinhaDimensionamento(2500, 1001, 8, 7) { Id = 357, GrupoId = 44 },
                new LinhaDimensionamento(5000, 2501, 10, 8) { Id = 358, GrupoId = 44 },
                new LinhaDimensionamento(10000, 5001, 12, 9) { Id = 359, GrupoId = 44 },
                new LinhaDimensionamento(50, 0, 0, 0) { Id = 360, GrupoId = 45 },
                new LinhaDimensionamento(100, 51, 1, 1) { Id = 361, GrupoId = 45 },
                new LinhaDimensionamento(500, 101, 2, 2) { Id = 362, GrupoId = 45 },
                new LinhaDimensionamento(1000, 501, 3, 3) { Id = 363, GrupoId = 45 },
                new LinhaDimensionamento(2500, 1001, 4, 3) { Id = 364, GrupoId = 45 },
                new LinhaDimensionamento(5000, 2501, 5, 4) { Id = 365, GrupoId = 45 },
                new LinhaDimensionamento(10000, 5001, 6, 5) { Id = 366, GrupoId = 45 }
            };

            Usuario usuario = new Usuario("teste@email.com", "Teste", "Cargo Teste")
            {
                Id = 1,
                Senha = "03c32dc379d1b0958f3ef87d94ebb4ec859b9e2fdd297f44d68d8dd5f36800cc", // Teste12
                Perfil = PerfilUsuario.SESMT,
                ContaId = 1
            };

            Empresa empresa = new Empresa("Empresa Teste", "01540533000390")
            {
                Id = 1,
                Cnpj = "01540533000390",
                ContaId = 1,
            };

            Estabelecimento estabelecimento = new Estabelecimento("Uberlândia", "Teste", 1)
            {
                Id = 1,
                Descricao = "Teste",
                GrupoId = 1
            };

            var etapasObrigatorias = new EtapaObrigatoria[6] {
                new EtapaObrigatoria
                {
                    Id = ECodigoEtapaObrigatoria.Convocacao,
                    Nome = "Convocação para a Eleição",
                    Descricao = "No início do processo eleitoral, o empregador deve convocar as eleições para a escolha dos representantes dos empregados na CIPA. Essa convocação precisa ocorrer no prazo mínimo de 60 dias antes do término do mandato em curso (NR-05 - 5.38)",
                    Ordem = (int)ECodigoEtapaObrigatoria.Convocacao,
                    PrazoMandatoAnterior = 60
                },
                new EtapaObrigatoria
                {
                    Id = ECodigoEtapaObrigatoria.FormacaoComissao,
                    Nome = "Formação da Comissão Eleitoral",
                    Descricao = "A Norma Regulamentadora 05 também determina que, no prazo mínimo de 55 dias antes do término do mandato em curso, seja constituída a Comissão Eleitoral (CE). Essa comissão deve ser formada pelos membros atuais da CIPA e será responsável pela organização e acompanhamento do processo eleitoral (NR-05 5.39). O edital de convocação também deve ser enviado para o sindicatos 5 dias após sua publicação.",
                    Ordem = (int)ECodigoEtapaObrigatoria.FormacaoComissao,
                    PrazoMandatoAnterior = 55
                },
                new EtapaObrigatoria
                {
                    Id = ECodigoEtapaObrigatoria.EditalInscricao,
                    Nome = "Edital de Inscrição dos Candidatos",
                    Descricao = "Nessa etapa, é preciso publicar e divulgar o edital de inscrição para a CIPA, em locais de fácil acesso e visualização. Atente-se ao prazo! A NR-5 determina que a publicação ocorra no prazo mínimo de 45 dias antes do término do mandato em curso. (NR-05 - 5.40 a)",
                    Ordem = (int)ECodigoEtapaObrigatoria.EditalInscricao,
                    PrazoMandatoAnterior = 45
                },
                new EtapaObrigatoria
                {
                    Id = ECodigoEtapaObrigatoria.Inscricao,
                    Nome = "Inscrição dos Candidatos",
                    Descricao = "Nessa etapa, o sistema libera acesso aos eleitores cadastrados nessa eleição para eles realizarem sua inscrição. Essa etapa deve ter duração mínima de 15 dias. É importante lembrar que qualquer empregado do estabelecimento, independentemente do setor ou local de trabalho, pode se inscrever. (NR-05 5.40 b. c.)",
                    DuracaoMinima = 15,
                    Ordem = (int)ECodigoEtapaObrigatoria.Inscricao
                },
                new EtapaObrigatoria
                {
                    Id = ECodigoEtapaObrigatoria.Votacao,
                    Nome = "Votação",
                    Descricao = "Nessa etapa, todos os eleitores podem acessar o sistema e escolher um dos candidatos como representante para o próximo mandato da CIPA. A eleição deve ser realizada em um dia normal de trabalho e 30 dias antes do término do mandato em curso, no mínimo. (NR-05 5.40 e. f.)",
                    Ordem = (int)ECodigoEtapaObrigatoria.Votacao,
                    PrazoMandatoAnterior = 30
                },
                new EtapaObrigatoria
                {
                    Id = ECodigoEtapaObrigatoria.Ata,
                    Nome = "Ata de Eleição",
                    Descricao = "Os candidatos votados e não eleitos devem estar relacionados nessa ata, em ordem decrescente de votos para nomeação posterior, em caso de vacância de suplentes. Não se preocupe, a criação desse documento é por nossa conta!",
                    Ordem = (int)ECodigoEtapaObrigatoria.Ata
                }
            };

            var etapasPadroesConta = new EtapaPadraoConta[6];
            for (var i = 0; i < 6; i++)
            {
                etapasPadroesConta[i] = new EtapaPadraoConta(
                    etapasObrigatorias[i].Nome, etapasObrigatorias[i].Descricao, etapasObrigatorias[i].Ordem, 1, 5, etapasObrigatorias[i].Id)
                {
                    Id = (int)etapasObrigatorias[i].Id
                };
            }

            modelBuilder.Entity<Conta>().HasData(conta);
            modelBuilder.Entity<TemplateEmail>().HasData(templatesEmails);
            modelBuilder.Entity<Empresa>().HasData(empresa);
            modelBuilder.Entity<Grupo>().HasData(grupos);
            modelBuilder.Entity<LimiteDimensionamento>().HasData(limitesDimensionamentos);
            modelBuilder.Entity<LinhaDimensionamento>().HasData(linhaDimensionamentos);
            modelBuilder.Entity<Usuario>().HasData(usuario);
            modelBuilder.Entity<Estabelecimento>().HasData(estabelecimento);
            modelBuilder.Entity<EtapaObrigatoria>().HasData(etapasObrigatorias);
            modelBuilder.Entity<EtapaPadraoConta>().HasData(etapasPadroesConta);
        }
    }
}
