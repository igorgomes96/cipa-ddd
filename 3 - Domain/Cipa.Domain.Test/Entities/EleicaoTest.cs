using System;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Xunit;

namespace Cipa.Domain.Test.Entities
{
    public class EleicaoTest
    {

        [Fact]
        public void EtapaAtual_CronogramaVazio_RetornaNull()
        {
            var eleicao = new Eleicao();
            var etapaAtual = eleicao.EtapaAtual;
            Assert.Null(etapaAtual);
        }

        [Fact]
        public void EtapaAtual_ProcessoNaoIniciado_RetornaNull()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            var etapaAtual = eleicao.EtapaAtual;
            Assert.Null(etapaAtual);
        }

        [Fact]
        public void EtapaAtual_CronogramaGerado_RetornaEtapa()
        {
            var eleicao = new Eleicao();
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
            var eleicao = new Eleicao();
            eleicao.Conta = conta;
            eleicao.DataInicio = new DateTime(2019, 1, 1);

            eleicao.GerarCronograma();

            Assert.Collection(eleicao.Cronograma,
                etapa =>
                {
                    Assert.Equal(new DateTime(2019, 1, 1), etapa.DataPrevista);
                    Assert.Equal("Etapa 1", etapa.Nome);
                    Assert.Equal(1, etapa.Ordem);
                    Assert.Equal(PosicaoEtapa.Atual, etapa.PosicaoEtapa);
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
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.False(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNullJaFinalizada_RetornaTrue()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.DataFinalizacao = DateTime.Today;
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.True(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var exception = Assert.Throws<CustomException>(() => eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaUltrapassada_RetornaTrue()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.True(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaAtual_RetornaFalse()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.False(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNaoUltrapassada_RetornaFalse()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.False(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNullNaoFinalizada_RetornaTrue()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.True(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNullJaFinalizada_RetornaFalse()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura });
            eleicao.DataFinalizacao = DateTime.Today;
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.False(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var exception = Assert.Throws<CustomException>(() => eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNaoUltrapassada_RetornaTrue()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.True(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaAtual_RetornaFalse()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.False(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaUltrapassada_RetornaTrue()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.True(retorno);
        }

        [Fact]
        public void BuscaEtapaObrigatoria_EtapaNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var exception = Assert.Throws<CustomException>(() => eleicao.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void BuscaEtapaObrigatoria_EtapaEncontrada_RetornaEtapa()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var etapa = eleicao.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.Convocacao);
            Assert.Equal(PosicaoEtapa.Atual, etapa.PosicaoEtapa);
            Assert.Equal(CodigoEtapaObrigatoria.Convocacao, etapa.EtapaObrigatoriaId);
        }

        [Fact]
        public void EtapaAnterior_PrimeiraEtapa_RetornaNull()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.EtapaAnterior(new EtapaCronograma { Ordem = 1 });
            Assert.Null(retorno);
        }

        [Fact]
        public void EtapaAnterior_EtapaDiferenteDaPrimeira_RetornaEtapa()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.EtapaAnterior(new EtapaCronograma { Ordem = 2 });
            Assert.Equal(1, retorno.Ordem);
            Assert.Equal(CodigoEtapaObrigatoria.Ata, retorno.EtapaObrigatoriaId);
        }

        [Fact]
        public void EtapaPosterior_UltimaEtapa_RetornaNull()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.EtapaPosterior(new EtapaCronograma { Ordem = 2 });
            Assert.Null(retorno);
        }

        [Fact]
        public void EtapaPosterior_EtapaDiferenteDaUltima_RetornaEtapa()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var retorno = eleicao.EtapaPosterior(new EtapaCronograma { Ordem = 1 });
            Assert.Equal(2, retorno.Ordem);
            Assert.Equal(CodigoEtapaObrigatoria.Convocacao, retorno.EtapaObrigatoriaId);
        }

        [Fact]
        public void DataTerminoEtapa_EtapaNaoEncontrada_ThrowsException()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            var exception = Assert.Throws<CustomException>(() => eleicao.DataTerminoEtapa(new EtapaCronograma { Id = 3, Ordem = 3 }));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void DataTerminoEtapa_UltimaEtapa_RetornaDataFinalizacao()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao });
            eleicao.DataFinalizacao = new DateTime(2019, 2, 15);
            var retorno = eleicao.DataTerminoEtapa(new EtapaCronograma { Id = 2, Ordem = 2 });
            Assert.Equal(new DateTime(2019, 2, 15), retorno);
        }

        [Fact]
        public void DataTerminoEtapa_PrimeiraEtapa_RetornaDataTerminoEtapa()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao, DataPrevista = new DateTime(2019, 2, 20) });
            var retorno = eleicao.DataTerminoEtapa(new EtapaCronograma { Id = 1, Ordem = 1 });
            Assert.Equal(new DateTime(2019, 2, 19), retorno);
        }

        [Fact]
        public void UltimaEtapa_CronogramaConsistente_RetornaUltimaEtapa()
        {
            var eleicao = new Eleicao();
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 1, Ordem = 1, PosicaoEtapa = PosicaoEtapa.Futura, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Ata });
            eleicao.Cronograma.Add(new EtapaCronograma { Id = 2, Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Convocacao, DataPrevista = new DateTime(2019, 2, 20) });
            Assert.Equal(2, eleicao.UltimaEtapa.Id);
        }

        [Theory]
        [InlineData(StatusInscricao.Aprovada, 3)]
        [InlineData(StatusInscricao.Pendente, 1)]
        [InlineData(StatusInscricao.Reprovada, 2)]
        public void QtdaInscricoes_InscricoesCarregadas_RetornaQtda(StatusInscricao statusInscricao, int qtdaExperada) {
            Eleicao eleicao = new Eleicao();
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Reprovada });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Pendente });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicao.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Reprovada });

            Assert.Equal(qtdaExperada, eleicao.QtdaInscricoes(statusInscricao));
        }

        

    }
}