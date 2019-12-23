using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Cipa.Domain.Services.Implementations;
using Moq;
using Xunit;

namespace Cipa.Domain.Test.Services
{
    public class ComunicadoErroMudancaEtapaServiceTest
    {
        private readonly Grupo grupo;
        private readonly Empresa empresa;
        private readonly Estabelecimento estabelecimento;
        private readonly Usuario usuarioCriacao;
        private readonly Eleicao eleicao;

        public ComunicadoErroMudancaEtapaServiceTest()
        {
            empresa = new Empresa("Soluções TI", "30271795000133") { Id = 1 };
            estabelecimento = new Estabelecimento("Uberlândia", "Av. Teste, 777, Bairro Santa Mônica", empresa.Id)
            {
                Empresa = empresa
            };
            var conta = new Conta { Id = 1 };
            var template = new StringBuilder();
            template.Append("Ocorreu um erro ao finalizar a etapa atual da eleição da ");
            template.Append("CIPA, que está sendo realizada na empresa @EMPRESA_CNPJ. Por favor, verifique.\n");
            template.Append("<br><br>\nMensagem de erro: <strong>@ERRO</strong>\n<br>\n");
            template.Append("Etapa Atual: <strong>@ETAPA_ATUAL</strong>\n<br>\n");
            template.Append("Etapa Posterior: <strong>@ETAPA_POSTERIOR</strong>\n<br><br>\n");
            template.Append("Obs.: A etapa atual deverá ser finalizada manualmente. Para isso, clique no botão \"Próxima Etapa\" do cronograma.");

            conta.AdicionarTempateEmail(
                new TemplateEmail(ETipoTemplateEmail.ErroMudancaEtapaCronograma, "[CIPA] Erro ao Realizar Mudança de Etapa")
                {
                    Template = template.ToString()
                }
            );
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Convocação", "Convocação", 1, conta.Id, 1, ECodigoEtapaObrigatoria.Convocacao));
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Inscrição", "Inscrição", 2, conta.Id, 2, ECodigoEtapaObrigatoria.Inscricao));
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Votação", "Votação", 3, conta.Id, 2, ECodigoEtapaObrigatoria.Votacao));
            usuarioCriacao = new Usuario("gestor@email.com", "Gestor", "Técnico do SESMT")
            {
                Conta = conta
            };
            grupo = new Grupo("C-TESTE");
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(5, 0, 1, 1));
            grupo.LimiteDimensionamento = new LimiteDimensionamento(5, 1, 1, 1);
            eleicao = new Eleicao(new DateTime(2020, 1, 1), 2, new DateTime(2020, 2, 28), usuarioCriacao, estabelecimento, grupo);
            eleicao.GerarCronograma();
        }

        [Fact]
        public void FormatarEmailPadrao_TodosOsParametrosValidos_RetornaListaEmails()
        {
            var usuario1 = new Usuario("email1@email.com", "Usuário 1", "Cargo 1") { Id = 1 };
            var usuario2 = new Usuario("email2@email.com", "Usuário 2", "Cargo 2") { Id = 2, Senha = "A#SESR3" };
            var usuario3 = new Usuario("email3@email.com", "Usuário 3", "Cargo 3") { Id = 3 };
            var usuario4 = new Usuario("email4@email.com", "Usuário 4", "Cargo 4") { Id = 4, Senha = "A#SEsd98f" };
            var eleitor1 = new Eleitor("Eleitor 1", "email1@email.com") { Id = 1, Usuario = usuario1, Cargo = "Cargo 1" };
            var eleitor2 = new Eleitor("Eleitor 2", "email2@email.com") { Id = 2, Usuario = usuario2, Cargo = "Cargo 2" };
            var eleitor3 = new Eleitor("Eleitor 3", "email3@email.com") { Id = 3, Usuario = usuario3, Cargo = "Cargo 3" };
            var eleitor4 = new Eleitor("Eleitor 4", "email4@email.com") { Id = 4, Usuario = usuario4, Cargo = "Cargo 4" };

            eleicao.AdicionarEleitor(eleitor1);
            eleicao.AdicionarEleitor(eleitor2);
            eleicao.AdicionarEleitor(eleitor3);
            eleicao.AdicionarEleitor(eleitor4);

            eleicao.PassarParaProximaEtapa();
            eleicao.PassarParaProximaEtapa();

            var inscricao1 = eleicao.FazerInscricao(eleitor1, "Objetivos 1");
            inscricao1.Id = 1;
            var inscricao2 = eleicao.FazerInscricao(eleitor3, "Objetivos 3");
            inscricao2.Id = 2;
            eleicao.AprovarInscricao(1, usuario1);
            eleicao.AprovarInscricao(2, usuario1);

            eleicao.PassarParaProximaEtapa();

            Assert.Throws<CustomException>(() => eleicao.PassarParaProximaEtapa(true));

            ComunicadoEleicaoBaseService comunicadoEleicao = new ComunicadoErroMudancaEtapaService(eleicao);
            var emails = comunicadoEleicao.FormatarEmails();

            var mensagemEsperadaBuilder = new StringBuilder();
            mensagemEsperadaBuilder.Append("Ocorreu um erro ao finalizar a etapa atual da eleição da CIPA, que está sendo realizada na ");
            mensagemEsperadaBuilder.Append("empresa Soluções TI, inscrita no CNPJ 30.271.795/0001-33. Por favor, verifique.");
            mensagemEsperadaBuilder.Append("\n<br><br>\n");
            mensagemEsperadaBuilder.Append("Mensagem de erro: <strong>Esta eleição ainda não atingiu os 50% de participação de todos os funcionários, conforme exigido pela NR-5.");
            mensagemEsperadaBuilder.Append("</strong>\n<br>\n");
            mensagemEsperadaBuilder.Append("Etapa Atual: <strong>Votação</strong>\n<br>\n");
            mensagemEsperadaBuilder.Append("Etapa Posterior: <strong>Finalização da Eleição</strong>\n<br><br>\n");
            mensagemEsperadaBuilder.Append("Obs.: A etapa atual deverá ser finalizada manualmente. Para isso, clique no botão \"Próxima Etapa\" do cronograma.");

            Assert.Collection(emails,
                (Email email) =>
                {
                    Assert.Equal(mensagemEsperadaBuilder.ToString(), email.Mensagem);
                    Assert.Collection(email.DestinatariosLista,
                        destinatario =>
                        {
                            Assert.Equal("gestor@email.com", destinatario);
                        });
                }
            );


        }


    }
}