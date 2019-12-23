using System;
using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Factories.Interfaces;
using Cipa.Domain.Helpers;
using Cipa.Domain.Services.Interfaces;
using Moq;
using Xunit;

namespace Cipa.Domain.Test.Entities
{
    public class ProcessamentoEtapaTest
    {
        private readonly ProcessamentoEtapa processamentoEtapa;
        private readonly Mock<IFormatadorEmailServiceFactory> formatadorFactory;

        public ProcessamentoEtapaTest()
        {
            formatadorFactory = new Mock<IFormatadorEmailServiceFactory>();
            var empresa = new Empresa("Soluções TI", "30271795000133") { Id = 1 };
            var estabelecimento = new Estabelecimento("Uberlândia", "Av. Teste, 777, Bairro Santa Mônica", empresa.Id)
            {
                Empresa = empresa
            };
            var conta = new Conta { Id = 1 };
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Convocação", "Convocação", 1, conta.Id, 1, ECodigoEtapaObrigatoria.Convocacao));
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Inscrição", "Inscrição", 2, conta.Id, 2, ECodigoEtapaObrigatoria.Inscricao));
            conta.AdicionarEtapaPadrao(new EtapaPadraoConta("Votação", "Votação", 3, conta.Id, 1, ECodigoEtapaObrigatoria.Votacao));
            var usuarioCriacao = new Usuario("gestor@email.com", "Gestor", "Técnico do SESMT")
            {
                Conta = conta
            };
            var grupo = new Grupo("C-TESTE");
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(5, 0, 1, 1));
            grupo.LimiteDimensionamento = new LimiteDimensionamento(5, 1, 1, 1);
            var eleicao = new Eleicao(new DateTime(2020, 1, 1), 2, new DateTime(2020, 2, 28), usuarioCriacao, estabelecimento, grupo);
            eleicao.GerarCronograma();
            var etapaInscricao = eleicao.BuscarEtapaObrigatoria(ECodigoEtapaObrigatoria.Inscricao);
            processamentoEtapa = new ProcessamentoEtapa(
                eleicao,
                etapaInscricao,
                eleicao.RetonarEtapaAnterior(etapaInscricao)
            );
        }

        [Fact]
        public void IniciarProcessamento_AlteraStatausProcessamento()
        {
            processamentoEtapa.IniciarProcessamento();
            Assert.Equal(EStatusProcessamentoEtapa.Processando, processamentoEtapa.StatusProcessamentoEtapa);
        }

        [Fact]
        public void RealizarProcessamento_ErroAoProcessar_AtualizaStatusMensagemErro()
        {
            formatadorFactory.Setup(f => f.ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(It.IsAny<ETipoTemplateEmail>(), It.IsAny<Eleicao>()))
                .Throws(new Exception("Erro"));

            processamentoEtapa.IniciarProcessamento();
            var retorno = processamentoEtapa.RealizarProcessamentoGerarEmails(new EmailConfiguration(), formatadorFactory.Object);
            Assert.Empty(retorno);
            Assert.Equal(EStatusProcessamentoEtapa.ErroProcessamento, processamentoEtapa.StatusProcessamentoEtapa);
            Assert.StartsWith("Erro ao processar mudança de etapa/envio de e-mail. Contate o suporte.\r\nErro", processamentoEtapa.MensagemErro);
        }

        [Fact]
        public void RealizarProcessamento_ProcessadoComSucesso_AtualizaStatusRetornaEmails()
        {
            var formatador = new Mock<IFormatadorEmailService>();
            formatadorFactory.Setup(f => f.ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(It.IsAny<ETipoTemplateEmail>(), It.IsAny<Eleicao>()))
                .Returns(formatador.Object);
            
            formatador.Setup(f => f.FormatarEmails()).Returns(new List<Email> { new Email("", "", "", "") } );

            processamentoEtapa.IniciarProcessamento();
            var retorno = processamentoEtapa.RealizarProcessamentoGerarEmails(new EmailConfiguration(), formatadorFactory.Object);
            Assert.Equal(1, retorno.Count);
            Assert.Equal(EStatusProcessamentoEtapa.ProcessadoComSucesso, processamentoEtapa.StatusProcessamentoEtapa);
            Assert.Null(processamentoEtapa.MensagemErro);
        }


    }
}