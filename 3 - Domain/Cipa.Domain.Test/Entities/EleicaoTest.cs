using System;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Moq;
using Xunit;

namespace Cipa.Domain.Test.Entities
{
    public class EleicaoTest
    {

        private readonly Eleicao eleicao;
        public EleicaoTest()
        {
            eleicao = new Eleicao(1, new DateTime(2019, 1, 1), 2, 1, 1, 1, 1, new DateTime(2019, 2, 28));
        }

        [Fact]
        public void EtapaAtual_CronogramaVazio_RetornaNull()
        {
            // var eleicao = new Eleicao();
            var etapaAtual = eleicao.EtapaAtual;
            Assert.Null(etapaAtual);
        }

        [Fact]
        public void EtapaAtual_ProcessoNaoIniciado_RetornaNull()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            var etapaAtual = eleicao.EtapaAtual;
            Assert.Null(etapaAtual);
        }

        [Fact]
        public void EtapaAtual_CronogramaGerado_RetornaEtapa()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, PosicaoEtapa = PosicaoEtapa.Passada });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, PosicaoEtapa = PosicaoEtapa.Atual });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 3, PosicaoEtapa = PosicaoEtapa.Futura });

            var etapaAtual = eleicao.EtapaAtual;
            Assert.Equal(2, etapaAtual.Id);
        }

        [Fact]
        public void GerarCronograma_EtapasPadroesCarregadas_CronogramaGerado()
        {
            var conta = new Conta();
            conta.EtapasPadroes.Add(
                new EtapaPadraoConta
                {
                    EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao,
                    Id = 1,
                    Ordem = 1,
                    Nome = "Etapa 1",
                    DuracaoPadrao = 10
                }
            );
            conta.EtapasPadroes.Add(
                new EtapaPadraoConta
                {
                    EtapaObrigatoriaId = CodigoEtapaObrigatoria.FormacaoComissao,
                    Id = 2,
                    Ordem = 2,
                    Nome = "Etapa 2",
                    DuracaoPadrao = 1
                }
            );
            // var eleicao = new Eleicao();
            eleicao.Conta = conta;
            eleicao.DataInicio = new DateTime(2019, 1, 1);

            eleicao.GerarCronograma();

            Assert.Collection(eleicao.Cronograma,
                etapa =>
                {
                    Assert.Equal(new DateTime(2019, 1, 1), etapa.DataPrevista);
                    Assert.Equal("Etapa 1", etapa.Nome);
                    Assert.Equal(1, etapa.Ordem);
                    Assert.Equal(PosicaoEtapa.Futura, etapa.PosicaoEtapa);
                },
                etapa =>
                {
                    Assert.Equal(new DateTime(2019, 1, 11), etapa.DataPrevista);
                    Assert.Equal("Etapa 2", etapa.Nome);
                    Assert.Equal(2, etapa.Ordem);
                    Assert.Equal(PosicaoEtapa.Futura, etapa.PosicaoEtapa);
                }
            );
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNullNaoFinalizada_RetornaFalse()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.False(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNullJaFinalizada_RetornaTrue()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.DataFinalizacao = DateTime.Today;
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.True(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNaoEncontrada_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var exception = Assert.Throws<CustomException>(() => eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaUltrapassada_RetornaTrue()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.True(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaAtual_RetornaFalse()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.False(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNaoUltrapassada_RetornaFalse()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.False(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNullNaoFinalizada_RetornaTrue()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.True(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNullJaFinalizada_RetornaFalse()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.DataFinalizacao = DateTime.Today;
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.False(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNaoEncontrada_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var exception = Assert.Throws<CustomException>(() => eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNaoUltrapassada_RetornaTrue()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.True(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaAtual_RetornaFalse()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.False(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaUltrapassada_RetornaTrue()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.True(retorno);
        }

        [Fact]
        public void BuscaEtapaObrigatoria_EtapaNaoEncontrada_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var exception = Assert.Throws<CustomException>(() => eleicao.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void BuscaEtapaObrigatoria_EtapaEncontrada_RetornaEtapa()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var etapa = eleicao.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.Convocacao);
            Assert.Equal(PosicaoEtapa.Atual, etapa.PosicaoEtapa);
            Assert.Equal(CodigoEtapaObrigatoria.Convocacao, etapa.EtapaObrigatoriaId);
        }

        [Fact]
        public void EtapaAnterior_PrimeiraEtapa_RetornaNull()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.EtapaAnterior(new EtapaCronograma { Ordem = 1 });
            Assert.Null(retorno);
        }

        [Fact]
        public void EtapaAnterior_EtapaDiferenteDaPrimeira_RetornaEtapa()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.EtapaAnterior(new EtapaCronograma { Ordem = 2 });
            Assert.Equal(1, retorno.Ordem);
            Assert.Equal(CodigoEtapaObrigatoria.Ata, retorno.EtapaObrigatoriaId);
        }

        [Fact]
        public void EtapaPosterior_UltimaEtapa_RetornaNull()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.EtapaPosterior(new EtapaCronograma { Ordem = 2 });
            Assert.Null(retorno);
        }

        [Fact]
        public void EtapaPosterior_EtapaDiferenteDaUltima_RetornaEtapa()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.EtapaPosterior(new EtapaCronograma { Ordem = 1 });
            Assert.Equal(2, retorno.Ordem);
            Assert.Equal(CodigoEtapaObrigatoria.Convocacao, retorno.EtapaObrigatoriaId);
        }

        [Fact]
        public void DataTerminoEtapa_EtapaNaoEncontrada_ThrowsException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var exception = Assert.Throws<CustomException>(() => eleicao.DataTerminoEtapa(new EtapaCronograma { Id = 3, Ordem = 3 }));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void DataTerminoEtapa_UltimaEtapa_RetornaDataFinalizacao()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            eleicao.DataFinalizacao = new DateTime(2019, 2, 15);
            var retorno = eleicao.DataTerminoEtapa(new EtapaCronograma { Id = 2, Ordem = 2 });
            Assert.Equal(new DateTime(2019, 2, 15), retorno);
        }

        [Fact]
        public void DataTerminoEtapa_PrimeiraEtapa_RetornaDataTerminoEtapa()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao, DataPrevista = new DateTime(2019, 2, 20) });
            var retorno = eleicao.DataTerminoEtapa(new EtapaCronograma { Id = 1, Ordem = 1 });
            Assert.Equal(new DateTime(2019, 2, 19), retorno);
        }

        [Fact]
        public void UltimaEtapa_CronogramaConsistente_RetornaUltimaEtapa()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao, DataPrevista = new DateTime(2019, 2, 20) });
            Assert.Equal(2, eleicao.UltimaEtapa.Id);
        }

        [Theory]
        [InlineData(StatusInscricao.Aprovada, 3)]
        [InlineData(StatusInscricao.Pendente, 1)]
        [InlineData(StatusInscricao.Reprovada, 2)]
        public void QtdaInscricoes_InscricoesCarregadas_RetornaQtda(StatusInscricao statusInscricao, int qtdaExperada)
        {
            // var eleicao = new Eleicao();
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Reprovada });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Pendente });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Reprovada });

            Assert.Equal(qtdaExperada, eleicao.QtdaInscricoes(statusInscricao));
        }

        [Fact]
        public void BuscarEleitor_IdExistente_RetornaEleitor()
        {
            // var eleicao = new Eleicao();
            eleicao.Eleitores.Add(new Eleitor { Id = 1 });
            eleicao.Eleitores.Add(new Eleitor { Id = 2 });
            eleicao.Eleitores.Add(new Eleitor { Id = 3 });

            var eleitor = eleicao.BuscarEleitor(2);

            Assert.Equal(2, eleitor.Id);
        }

        [Fact]
        public void BuscarEleitor_IdInexistente_RetornaNull()
        {
            // var eleicao = new Eleicao();
            eleicao.Eleitores.Add(new Eleitor { Id = 1 });
            eleicao.Eleitores.Add(new Eleitor { Id = 2 });
            eleicao.Eleitores.Add(new Eleitor { Id = 3 });

            var eleitor = eleicao.BuscarEleitor(4);

            Assert.Null(eleitor);
        }

        [Theory]
        [InlineData(26, 0, "Esta eleição ainda não possui a quantidade mínima de inscritos!")]
        [InlineData(26, 1, "Ainda há 1 inscrição pendente de aprovação!")]
        [InlineData(27, 1, "Ainda há 1 inscrição pendente de aprovação!")]
        [InlineData(27, 2, "Ainda há 2 inscrições pendentes de aprovação!")]
        public void PassarParaProximaEtapa_EtapaInscricaoQtdaMinimaInscritos_ThrowsCustomException(
            int qtdaInscricoesAprovadas, int qtdaInscricoesPendentes, string mensagemErro)
        {
            var dimensionamento = new Dimensionamento(10000, 5001, 15, 12)
            {
                QtdaInscricoesAprovadas = qtdaInscricoesAprovadas,
                QtdaInscricoesPendentes = qtdaInscricoesPendentes,
                QtdaInscricoesReprovadas = 2
            };

            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });

            var excecao = Assert.Throws<CustomException>(() => eleicao.PassarParaProximaEtapa(dimensionamento));
            Assert.Equal(mensagemErro, excecao.Message);
        }

        [Theory]
        [InlineData(7000, 3499)]
        [InlineData(7001, 3500)]
        public void PassarParaProximaEtapa_EtapaVotacaoSemQtdaMinimaVotos_ThrowCustomException(
            int qtdaEleitores, int qtdaVotos)
        {
            var dimensionamento = new Dimensionamento(10000, 5001, 15, 12)
            {
                QtdaInscricoesAprovadas = 27,
                QtdaInscricoesPendentes = 0,
                QtdaInscricoesReprovadas = 2,
                QtdaEleitores = qtdaEleitores,
                QtdaVotos = qtdaVotos
            };

            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });

            var excecao = Assert.Throws<CustomException>(() => eleicao.PassarParaProximaEtapa(dimensionamento));
            Assert.Equal("Esta eleição ainda não atingiu os 50% de participação de todos os funcionários, conforme exigido pela NR-5.", excecao.Message);
        }

        public static object[][] InlineDataCronograma = new object[][] {
            new object[] {
                PosicaoEtapa.Futura, PosicaoEtapa.Futura, PosicaoEtapa.Futura,
                PosicaoEtapa.Atual, PosicaoEtapa.Futura, PosicaoEtapa.Futura,
                DateTime.Today, null, null, null
            },
            new object[] {
                PosicaoEtapa.Atual, PosicaoEtapa.Futura, PosicaoEtapa.Futura,
                PosicaoEtapa.Passada, PosicaoEtapa.Atual, PosicaoEtapa.Futura,
                null, DateTime.Today, null, null
            },
            new object[] {
                PosicaoEtapa.Passada, PosicaoEtapa.Atual, PosicaoEtapa.Futura,
                PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Atual,
                null, null, DateTime.Today, null
            },
            new object[] {
                PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Atual,
                PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Passada,
                null, null, null, DateTime.Now
            },
        };
        [Theory, MemberData(nameof(InlineDataCronograma))]
        public void PassarParaProximaEtapa_NenhumaInsconsistencia_CronogramaAtualizado(
            PosicaoEtapa posicaoEtapaCorrente1, PosicaoEtapa posicaoEtapaCorrente2, PosicaoEtapa posicaoEtapaCorrente3,
            PosicaoEtapa posicaoEtapaEsperada1, PosicaoEtapa posicaoEtapaEsperada2, PosicaoEtapa posicaoEtapaEsperada3,
            DateTime? dataRealizada1, DateTime? dataRealizada2, DateTime? dataRealizada3, DateTime? dataFinalizacaoEleicao
        )
        {
            var dimensionamento = new Dimensionamento(10000, 5001, 15, 12)
            {
                QtdaInscricoesAprovadas = 27,
                QtdaInscricoesPendentes = 0,
                QtdaInscricoesReprovadas = 2,
                QtdaEleitores = 7000,
                QtdaVotos = 3500
            };

            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = posicaoEtapaCorrente1, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = posicaoEtapaCorrente2, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = posicaoEtapaCorrente3, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });

            eleicao.PassarParaProximaEtapa(dimensionamento);

            Assert.Collection(eleicao.Cronograma,
                etapa =>
                {
                    Assert.Equal(posicaoEtapaEsperada1, etapa.PosicaoEtapa);
                    Assert.Equal(dataRealizada1, etapa.DataRealizada);
                },
                etapa =>
                {
                    Assert.Equal(posicaoEtapaEsperada2, etapa.PosicaoEtapa);
                    Assert.Equal(dataRealizada2, etapa.DataRealizada);
                },
                etapa =>
                {
                    Assert.Equal(posicaoEtapaEsperada3, etapa.PosicaoEtapa);
                    Assert.Equal(dataRealizada3, etapa.DataRealizada);
                }
            );

            if (!dataFinalizacaoEleicao.HasValue)
            {
                Assert.Null(eleicao.DataFinalizacao);
            }
            else
            {
                Assert.InRange(eleicao.DataFinalizacao.Value, DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1));
            }
        }

        [Fact]
        public void AdicionarEleitor_EleitorEmailDuplicado_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Eleitores.Add(new Eleitor { Email = "email1@teste.com" });
            eleicao.Eleitores.Add(new Eleitor { Email = "email2@teste.com" });

            var novoEleitor = new Eleitor { Email = "email1@teste.com" };
            var excecao = Assert.Throws<CustomException>(() => eleicao.AdicionarEleitor(novoEleitor, It.IsAny<Dimensionamento>(), It.IsAny<Dimensionamento>()));
            Assert.Equal("Já existe um eleitor cadastrado com o mesmo e-mail para essa eleição.", excecao.Message);
        }

        [Fact]
        public void AdicionarEleitor_EleicaoAposEtapaVotacao_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Eleitores.Add(new Eleitor { Email = "email1@teste.com" });
            eleicao.Eleitores.Add(new Eleitor { Email = "email2@teste.com" });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var novoEleitor = new Eleitor { Email = "email3@teste.com" };
            var excecao = Assert.Throws<CustomException>(() => eleicao.AdicionarEleitor(novoEleitor, It.IsAny<Dimensionamento>(), It.IsAny<Dimensionamento>()));
            Assert.Equal("Não é permitido cadastrar eleitores após o período de votação.", excecao.Message);
        }

        [Fact]
        public void AdicionarEleitor_DimensionamentoInvalido_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Eleitores.Add(new Eleitor { Email = "email1@teste.com" });
            eleicao.Eleitores.Add(new Eleitor { Email = "email2@teste.com" });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var novoEleitor = new Eleitor { Email = "email3@teste.com" };
            var dimensionamentoAtual = new Dimensionamento(29, 20, 1, 1)
            {
                QtdaEleitores = 29,
                QtdaInscricoesAprovadas = 2,
                QtdaInscricoesPendentes = 2,
                QtdaInscricoesReprovadas = 2
            };

            var dimensionamentoProposto = new Dimensionamento(50, 30, 2, 1)
            {
                QtdaEleitores = 39
            };
            var excecao = Assert.Throws<CustomException>(() => eleicao.AdicionarEleitor(novoEleitor, dimensionamentoAtual, dimensionamentoProposto));
            Assert.Equal("Não é possível adicionar esse novo eleitor, pois sua inclusão altera o dimensionamento da eleição e com isso a quantidade mínima de inscritos passa a ser superior à quantidade atual de inscritos.", excecao.Message);
        }

        [Fact]
        public void AdicionarEleitor_DimensionamentoValido_AtualizaDimensionamentoEleicaoAdicionaEleitor()
        {
            // var eleicao = new Eleicao();
            eleicao.Eleitores.Add(new Eleitor { Email = "email1@teste.com" });
            eleicao.Eleitores.Add(new Eleitor { Email = "email2@teste.com" });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var novoEleitor = new Eleitor { Email = "email3@teste.com" };
            var dimensionamentoAtual = new Dimensionamento(29, 20, 1, 1)
            {
                QtdaEleitores = 29,
                QtdaInscricoesAprovadas = 3,
                QtdaInscricoesPendentes = 2,
                QtdaInscricoesReprovadas = 2
            };

            var dimensionamentoProposto = new Dimensionamento(50, 30, 2, 1)
            {
                QtdaEleitores = 30
            };

            eleicao.AdicionarEleitor(novoEleitor, dimensionamentoAtual, dimensionamentoProposto);
            Assert.Equal(3, eleicao.Eleitores.Count);
            Assert.Contains(eleicao.Eleitores, e => e.Email == "email3@teste.com");
            Assert.Same(dimensionamentoProposto, eleicao.Dimensionamento);
        }

        [Fact]
        public void FazerInscricao_ForaPeriodoInscricoes_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var exception = Assert.Throws<CustomException>(() => eleicao.FazerInscricao(new Eleitor { Id = 1 }, "Meus Objetivos."));
            Assert.Equal("As inscrições podem ser realizadas somente no período de inscrição. Confira o cronograma da eleição.", exception.Message);
        }

        [Fact]
        public void FazerInscricao_EleitorJaInscrito_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Inscricoes.Add(new Inscricao { EleitorId = 1 });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var exception = Assert.Throws<CustomException>(() => eleicao.FazerInscricao(new Eleitor { Id = 1 }, "Meus Objetivos."));
            Assert.Equal("Esse eleitor já está inscrito na eleição.", exception.Message);
        }

        [Fact]
        public void AtualizarInscricao_ForaPeriodoInscricoes_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var exception = Assert.Throws<CustomException>(() => eleicao.AtualizarInscricao(new Eleitor { Id = 1 }, "Meus Objetivos."));
            Assert.Equal("As inscrições não podem ser alteradas fora do período de inscrição.", exception.Message);
        }

        [Fact]
        public void AtualizarInscricao_InscricaoNaoEncontrada_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var exception = Assert.Throws<NotFoundException>(() => eleicao.AtualizarInscricao(new Eleitor { Id = 1 }, "Meus Objetivos."));
            Assert.Equal("Inscrição não encontrada.", exception.Message);
        }

        [Fact]
        public void AtualizarInscricao_InscricaoEncontrada_InscricaoAtualizada()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Inscricoes.Add(new Inscricao { EleitorId = 1, Objetivos = "Teste", StatusInscricao = StatusInscricao.Reprovada });

            var inscricaoRetornada = eleicao.AtualizarInscricao(new Eleitor { Id = 1 }, "Meus Objetivos.");
            var inscricao = eleicao.BuscarInscricaoPeloEleitorId(1);
            Assert.Same(inscricaoRetornada, inscricao);
            Assert.Equal("Meus Objetivos.", inscricao.Objetivos);
            Assert.Equal(StatusInscricao.Pendente, inscricao.StatusInscricao);
        }

        [Fact]
        public void ReprovarInscricao_ForaPeriodoInscricoes_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var exception = Assert.Throws<CustomException>(() => eleicao.ReprovarInscricao(1, new Usuario(), "Motivo Reprovação"));
            Assert.Equal("As inscrições não podem ser reprovadas fora do período de inscrição.", exception.Message);
        }

        [Fact]
        public void ReprovarInscricao_InscricaoNaoEncontrada_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var exception = Assert.Throws<NotFoundException>(() => eleicao.ReprovarInscricao(1, new Usuario(), "Motivo Reprovação"));
            Assert.Equal("Inscrição não encontrada.", exception.Message);
        }

        [Fact]
        public void ReprovarInscricao_InscricaoEncontrada_AdicionaReprovacao()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Inscricoes.Add(new Inscricao { Id = 1, EleitorId = 1, Objetivos = "Teste", StatusInscricao = StatusInscricao.Pendente });

            var usuarioAprovador = new Usuario
            {
                Email = "aprovador@email.com",
                Nome = "Aprovador"
            };
            var inscricaoRetornada = eleicao.ReprovarInscricao(1, usuarioAprovador, "Foto indevida");
            Assert.Collection(eleicao.Inscricoes,
                inscricao =>
                {
                    Assert.Same(inscricao, inscricaoRetornada);
                    Assert.Equal(StatusInscricao.Reprovada, inscricao.StatusInscricao);
                    Assert.Collection(inscricao.Reprovacoes,
                        reprovacao =>
                        {
                            Assert.Equal("aprovador@email.com", reprovacao.EmailAprovador);
                            Assert.Equal("Aprovador", reprovacao.NomeAprovador);
                            Assert.Equal("Foto indevida", reprovacao.MotivoReprovacao);
                        }
                    );
                }
            );
        }

        [Fact]
        public void AprovarInscricao_ForaPeriodoInscricoes_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var exception = Assert.Throws<CustomException>(() => eleicao.AprovarInscricao(1, new Usuario()));
            Assert.Equal("As inscrições não podem ser aprovadas fora do período de inscrição.", exception.Message);
        }

        [Fact]
        public void AprovarInscricao_InscricaoNaoEncontrada_ThrowsCustomException()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });

            var exception = Assert.Throws<NotFoundException>(() => eleicao.AprovarInscricao(1, new Usuario()));
            Assert.Equal("Inscrição não encontrada.", exception.Message);
        }

        [Fact]
        public void AprovarInscricao_InscricaoEncontrada_AtualizaInscricao()
        {
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Inscricoes.Add(new Inscricao { Id = 1, EleitorId = 1, Objetivos = "Teste", StatusInscricao = StatusInscricao.Pendente });

            var usuarioAprovador = new Usuario
            {
                Email = "aprovador@email.com",
                Nome = "Aprovador"
            };

            var inscricaoRetornada = eleicao.AprovarInscricao(1, usuarioAprovador);
            Assert.Collection(eleicao.Inscricoes,
                inscricao =>
                {
                    Assert.Same(inscricao, inscricaoRetornada);
                    Assert.Equal(StatusInscricao.Aprovada, inscricao.StatusInscricao);
                    Assert.Equal("aprovador@email.com", inscricao.EmailAprovador);
                    Assert.Equal("Aprovador", inscricao.NomeAprovador);
                }
            );
        }

        [Fact]
        public void RegistrarVoto_EleitorJaVotou_ThrowsCustomException()
        {
            var eleitor = new Eleitor { Id = 1 };
            // var eleicao = new Eleicao();
            eleicao.Inscricoes.Add(new Inscricao { EleitorId = 1, Eleitor = eleitor });
            eleicao.Votos.Add(new Voto(eleitor, "::1"));

            var exception = Assert.Throws<CustomException>(() => eleicao.RegistrarVoto(1, eleitor, "::1"));
            Assert.Equal("Eleitor já registrou seu voto nessa eleição. Não é possível votar mais de uma vez.", exception.Message);
        }

        [Theory]
        [InlineData(PosicaoEtapa.Atual, PosicaoEtapa.Futura, PosicaoEtapa.Futura)]
        [InlineData(PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Atual)]
        public void RegistrarVoto_EleicaoForaDoPeriodoDeVotacao_ThrowsCustomException(PosicaoEtapa posicaoEtapa1, PosicaoEtapa posicaoEtapa2, PosicaoEtapa posicaoEtapa3)
        {
            var eleitor = new Eleitor { Id = 1 };
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = posicaoEtapa1, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = posicaoEtapa2, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = posicaoEtapa3, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Inscricoes.Add(new Inscricao { EleitorId = 1, Eleitor = eleitor });
            eleicao.Votos.Add(new Voto(new Eleitor { Id = 2 }, "::1"));

            var exception = Assert.Throws<CustomException>(() => eleicao.RegistrarVoto(1, eleitor, "::1"));
            Assert.Equal("Essa eleição não está no período de votação.", exception.Message);
        }

        [Fact]
        public void RegistrarVoto_InscricaoNaoEncontrada_ThrowsNotFoundException()
        {
            var eleitor = new Eleitor { Id = 1 };
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Inscricoes.Add(new Inscricao { Id = 1, EleitorId = 2, Eleitor = eleitor });
            eleicao.Votos.Add(new Voto(new Eleitor { Id = 2 }, "::1"));

            var exception = Assert.Throws<NotFoundException>(() => eleicao.RegistrarVoto(2, eleitor, "::1"));
            Assert.Equal("Inscrição não encontrada.", exception.Message);
        }

        [Theory]
        [InlineData(StatusInscricao.Pendente)]
        [InlineData(StatusInscricao.Reprovada)]
        public void RegistrarVoto_InscricaoNaoAprovada_ThrowNotFoundException(StatusInscricao statusInscricao)
        {
            var eleitor = new Eleitor { Id = 1 };
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Inscricoes.Add(new Inscricao { Id = 1, EleitorId = 1, Eleitor = eleitor, StatusInscricao = StatusInscricao.Aprovada });
            eleicao.Inscricoes.Add(new Inscricao { Id = 2, EleitorId = 2, Eleitor = new Eleitor { Id = 2 }, StatusInscricao = statusInscricao });

            var exception = Assert.Throws<NotFoundException>(() => eleicao.RegistrarVoto(2, eleitor, "::1"));
            Assert.Equal("Inscrição não encontrada.", exception.Message);

        }

        [Fact]
        public void RegistrarVoto_VotoValido_VotoRegistrado()
        {
            var eleitor = new Eleitor { Id = 1 };
            // var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 3, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Inscricoes.Add(new Inscricao { Id = 1, EleitorId = 1, Eleitor = eleitor, StatusInscricao = StatusInscricao.Aprovada });
            eleicao.Inscricoes.Add(new Inscricao { Id = 2, EleitorId = 2, Eleitor = new Eleitor { Id = 2 }, StatusInscricao = StatusInscricao.Aprovada });

            var votoRetornado = eleicao.RegistrarVoto(2, eleitor, "::1");
            Assert.Collection(eleicao.Inscricoes,
                inscricao =>
                {
                    Assert.Equal(0, inscricao.Votos);
                },
                inscricao =>
                {
                    Assert.Equal(1, inscricao.Votos);
                }
            );
            Assert.Collection(eleicao.Votos,
                voto =>
                {
                    Assert.Same(voto, votoRetornado);
                }
            );

        }

    }
}