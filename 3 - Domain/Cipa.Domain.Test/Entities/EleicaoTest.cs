using System;
using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Moq;
using Xunit;

namespace Cipa.Domain.Test.Entities
{
    public class eleicao
    {
        #region Funções Auxiliares
        private Conta ContaPadrao()
        {
            var conta = new Conta();
            conta.EtapasPadroes.Add(new EtapaPadraoConta("Etapa 1", null, 1, 1, 10, CodigoEtapaObrigatoria.Convocacao));
            conta.EtapasPadroes.Add(new EtapaPadraoConta("Etapa 2", null, 2, 1, 1, CodigoEtapaObrigatoria.Inscricao));
            conta.EtapasPadroes.Add(new EtapaPadraoConta("Etapa 3", null, 3, 1, 1, CodigoEtapaObrigatoria.Votacao));
            conta.EtapasPadroes.Add(new EtapaPadraoConta("Etapa 4", null, 4, 1, 1, CodigoEtapaObrigatoria.Ata));
            return conta;
        }

        private Usuario UsuarioPadrao => new Usuario("teste@email.com", "Teste", "Teste Cargo") { Conta = ContaPadrao() };
        private Grupo GrupoPadrao()
        {
            var grupo = new Grupo("C-Teste")
            {
                LimiteDimensionamento = new LimiteDimensionamento(10000, 2500, 2, 2)
            };
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(19, 0, 0, 0));
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(50, 20, 1, 1));
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(100, 51, 3, 3));
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(500, 101, 4, 3));
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(1000, 501, 6, 4));
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(2500, 1001, 9, 7));
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(5000, 2501, 12, 9));
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(10000, 5001, 15, 12));
            return grupo;
        }
        private Estabelecimento EstabelecimentoPadrao => new Estabelecimento { Grupo = GrupoPadrao() };

        private Eleicao CriarEleicao(Conta conta, Grupo grupo, int qtdaEleicoesNoEstabelecimento = 0)
        {
            var eleicao = new Eleicao(
                new DateTime(2019, 1, 1),
                2,
                new DateTime(2019, 2, 28),
                UsuarioPadrao,
                EstabelecimentoPadrao,
                qtdaEleicoesNoEstabelecimento,
                grupo);
            eleicao.Id = 1;
            eleicao.GerarCronograma();
            return eleicao;
        }

        private Eleicao CriarEleicao() => CriarEleicao(ContaPadrao(), GrupoPadrao());

        private void FinalizarEleicao(Eleicao eleicao)
        {
            for (int i = 0; i <= eleicao.Cronograma.Count; i++)
                eleicao.PassarParaProximaEtapa();
        }

        private void PassarParaEtapaInscricao(Eleicao eleicao)
        {
            while (eleicao.EtapaAtual?.EtapaObrigatoriaId != CodigoEtapaObrigatoria.Inscricao)
            {
                eleicao.PassarParaProximaEtapa();
            }
        }

        private void PassarParaEtapaVotacao(Eleicao eleicao)
        {
            while (eleicao.EtapaAtual?.EtapaObrigatoriaId != CodigoEtapaObrigatoria.Votacao)
            {
                eleicao.PassarParaProximaEtapa();
            }
        }

        private void PassarEtapaAte(Eleicao eleicao, CodigoEtapaObrigatoria codigoEtapaObrigatoria)
        {
            while (eleicao.EtapaAtual?.EtapaObrigatoriaId != codigoEtapaObrigatoria)
            {
                eleicao.PassarParaProximaEtapa();
            }
        }
        #endregion

        [Fact]
        public void EtapaAtual_ProcessoNaoIniciado_RetornaNull()
        {
            var eleicao = CriarEleicao();
            Assert.Null(eleicao.EtapaAtual);
        }

        [Fact]
        public void EtapaAtual_CronogramaGerado_RetornaEtapa()
        {
            var eleicao = CriarEleicao();
            eleicao.PassarParaProximaEtapa();
            Assert.Equal(1, eleicao.EtapaAtual.Ordem);
        }

        [Fact]
        public void GerarCronograma_EtapasPadroesCarregadas_CronogramaGerado()
        {

            var eleicao = CriarEleicao();

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
                },
                etapa =>
                {
                    Assert.Equal(new DateTime(2019, 1, 12), etapa.DataPrevista);
                    Assert.Equal("Etapa 3", etapa.Nome);
                    Assert.Equal(3, etapa.Ordem);
                    Assert.Equal(PosicaoEtapa.Futura, etapa.PosicaoEtapa);
                },
                etapa =>
                {
                    Assert.Equal(new DateTime(2019, 1, 13), etapa.DataPrevista);
                    Assert.Equal("Etapa 4", etapa.Nome);
                    Assert.Equal(4, etapa.Ordem);
                    Assert.Equal(PosicaoEtapa.Futura, etapa.PosicaoEtapa);
                }
            );
        }

        [Fact]
        public void JaUltrapassouEtapa_ProcessoNaoIniciado_RetornaFalse()
        {
            var eleicao = CriarEleicao();
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.False(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_ProcessoFinalizado_RetornaTrue()
        {
            var eleicao = CriarEleicao();
            FinalizarEleicao(eleicao);
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.True(retorno);
            Assert.NotNull(eleicao.DataFinalizacao);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var exception = Assert.Throws<CustomException>(() => eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaUltrapassada_RetornaTrue()
        {
            var eleicao = CriarEleicao();
            eleicao.PassarParaProximaEtapa();
            eleicao.PassarParaProximaEtapa();
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.True(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaAtual_RetornaFalse()
        {
            var eleicao = CriarEleicao();
            eleicao.PassarParaProximaEtapa();
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.Equal(PosicaoEtapa.Atual, eleicao.EtapaAtual.PosicaoEtapa);
            Assert.False(retorno);
        }

        [Fact]
        public void JaUltrapassouEtapa_EtapaNaoUltrapassada_RetornaFalse()
        {
            var eleicao = CriarEleicao();
            eleicao.PassarParaProximaEtapa();
            var retorno = eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Inscricao);
            Assert.False(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_ProcessoNaoIniciado_RetornaTrue()
        {
            var eleicao = CriarEleicao();
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.True(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_ProcessoFinalizado_RetornaFalse()
        {
            var eleicao = CriarEleicao();
            FinalizarEleicao(eleicao);
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Ata);
            Assert.False(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var exception = Assert.Throws<CustomException>(() => eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaNaoUltrapassada_RetornaTrue()
        {
            var eleicao = CriarEleicao();
            eleicao.PassarParaProximaEtapa();
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Inscricao);
            Assert.True(retorno);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaAtual_RetornaFalse()
        {
            var eleicao = CriarEleicao();
            eleicao.PassarParaProximaEtapa();
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.False(retorno);
            Assert.Equal(CodigoEtapaObrigatoria.Convocacao, eleicao.EtapaAtual.EtapaObrigatoriaId);
        }

        [Fact]
        public void AindaNaoUltrapassouEtapa_EtapaUltrapassada_RetornaFalse()
        {
            var eleicao = CriarEleicao();
            eleicao.PassarParaProximaEtapa();
            eleicao.PassarParaProximaEtapa();
            var retorno = eleicao.AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria.Convocacao);
            Assert.False(retorno);
        }

        [Fact]
        public void BuscaEtapaObrigatoria_EtapaNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var exception = Assert.Throws<CustomException>(() => eleicao.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.EditalInscricao));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void BuscaEtapaObrigatoria_EtapaEncontrada_RetornaEtapa()
        {
            var eleicao = CriarEleicao();
            var etapa = eleicao.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.Inscricao);
            Assert.Equal(CodigoEtapaObrigatoria.Inscricao, etapa.EtapaObrigatoriaId);
        }

        [Fact]
        public void EtapaAnterior_PrimeiraEtapa_RetornaNull()
        {
            var eleicao = CriarEleicao();
            var retorno = eleicao.RetonarEtapaAnterior(new EtapaCronograma("Etapa", null, 1, eleicao.Id, DateTime.Today, null));
            Assert.Null(retorno);
        }

        [Fact]
        public void EtapaAnterior_EtapaDiferenteDaPrimeira_RetornaEtapa()
        {
            var eleicao = CriarEleicao();
            var retorno = eleicao.RetonarEtapaAnterior(new EtapaCronograma("Etapa", null, 2, eleicao.Id, DateTime.Today, null));
            Assert.Equal(1, retorno.Ordem);
            Assert.Equal(CodigoEtapaObrigatoria.Convocacao, retorno.EtapaObrigatoriaId);
        }

        [Fact]
        public void EtapaPosterior_UltimaEtapa_RetornaNull()
        {
            var eleicao = CriarEleicao();
            for (int i = 0; i < eleicao.Cronograma.Count; i++)
                eleicao.PassarParaProximaEtapa();
            var retorno = eleicao.RetornarEtapaPosterior(new EtapaCronograma("Etapa", null, 4, eleicao.Id, DateTime.Today, null));
            Assert.Null(retorno);
        }

        [Fact]
        public void EtapaPosterior_EtapaDiferenteDaUltima_RetornaEtapa()
        {
            var eleicao = CriarEleicao();
            var retorno = eleicao.RetornarEtapaPosterior(new EtapaCronograma("Etapa", null, 1, eleicao.Id, DateTime.Today, null));
            Assert.Equal(2, retorno.Ordem);
            Assert.Equal(CodigoEtapaObrigatoria.Inscricao, retorno.EtapaObrigatoriaId);
        }

        [Fact]
        public void DataTerminoEtapa_EtapaNaoEncontrada_ThrowsException()
        {
            var eleicao = CriarEleicao();
            var exception = Assert.Throws<CustomException>(() => eleicao.DataTerminoEtapa(new EtapaCronograma("Etapa", null, 5, eleicao.Id, DateTime.Today, null)));
            Assert.Equal("Etapa não encontrada.", exception.Message);
        }

        [Fact]
        public void DataTerminoEtapa_UltimaEtapa_RetornaDataFinalizacao()
        {
            var eleicao = CriarEleicao();
            FinalizarEleicao(eleicao);
            var retorno = eleicao.DataTerminoEtapa(new EtapaCronograma("Etapa", null, 4, eleicao.Id, DateTime.Today, null));
            Assert.InRange(retorno, DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1));
        }

        [Fact]
        public void DataTerminoEtapa_PrimeiraEtapa_RetornaDataTerminoEtapa()
        {
            var eleicao = CriarEleicao();
            var retorno = eleicao.DataTerminoEtapa(eleicao.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.Convocacao));
            Assert.Equal(new DateTime(2019, 1, 10), retorno);
        }

        [Fact]
        public void UltimaEtapa_CronogramaConsistente_RetornaUltimaEtapa()
        {
            var eleicao = CriarEleicao();
            Assert.Equal(4, eleicao.UltimaEtapa.Ordem);
        }

        [Theory]
        [InlineData(StatusInscricao.Aprovada, 3)]
        [InlineData(StatusInscricao.Pendente, 1)]
        [InlineData(StatusInscricao.Reprovada, 2)]
        public void QtdaInscricoes_InscricoesCarregadas_RetornaQtda(StatusInscricao statusInscricao, int qtdaExperada)
        {
            var eleicao = CriarEleicao();
            PassarParaEtapaInscricao(eleicao);
            var usuario1 = new Usuario("eleitor1@email.com", "Eleitor 1", "Cargo");
            var usuario2 = new Usuario("eleitor2@email.com", "Eleitor 2", "Cargo");
            var usuario3 = new Usuario("eleitor3@email.com", "Eleitor 3", "Cargo");
            var usuario4 = new Usuario("eleitor4@email.com", "Eleitor 4", "Cargo");
            var usuario5 = new Usuario("eleitor5@email.com", "Eleitor 5", "Cargo");
            var usuario6 = new Usuario("eleitor6@email.com", "Eleitor 6", "Cargo");

            var inscricao1 = eleicao.FazerInscricao(new Eleitor(usuario1) { Id = 1 }, "Objetivos");
            var inscricao2 = eleicao.FazerInscricao(new Eleitor(usuario2) { Id = 2 }, "Objetivos");
            var inscricao3 = eleicao.FazerInscricao(new Eleitor(usuario3) { Id = 3 }, "Objetivos");
            var inscricao4 = eleicao.FazerInscricao(new Eleitor(usuario4) { Id = 4 }, "Objetivos");
            var inscricao5 = eleicao.FazerInscricao(new Eleitor(usuario5) { Id = 5 }, "Objetivos");
            var inscricao6 = eleicao.FazerInscricao(new Eleitor(usuario6) { Id = 6 }, "Objetivos");

            inscricao1.Id = 1;
            inscricao2.Id = 2;
            inscricao3.Id = 3;
            inscricao4.Id = 4;
            inscricao5.Id = 5;
            inscricao6.Id = 6;

            eleicao.AprovarInscricao(inscricao2.Id, usuario1);
            eleicao.AprovarInscricao(inscricao3.Id, usuario1);
            eleicao.AprovarInscricao(inscricao6.Id, usuario1);
            eleicao.ReprovarInscricao(inscricao1.Id, usuario1, "Motivo");
            eleicao.ReprovarInscricao(inscricao4.Id, usuario1, "Motivo");

            Assert.Equal(qtdaExperada, eleicao.QtdaInscricoes(statusInscricao));
        }

        [Fact]
        public void BuscarEleitor_IdExistente_RetornaEleitor()
        {
            var eleicao = CriarEleicao();
            var usuario1 = new Usuario("eleitor1@email.com", "Eleitor 1", "Cargo");
            var usuario2 = new Usuario("eleitor2@email.com", "Eleitor 2", "Cargo");
            var usuario3 = new Usuario("eleitor3@email.com", "Eleitor 3", "Cargo");

            var eleitor1 = new Eleitor(usuario1) { Id = 1 };
            var eleitor2 = new Eleitor(usuario2) { Id = 2 };
            var eleitor3 = new Eleitor(usuario3) { Id = 3 };

            eleicao.AdicionarEleitor(eleitor1);
            eleicao.AdicionarEleitor(eleitor2);
            eleicao.AdicionarEleitor(eleitor3);

            var eleitor = eleicao.BuscarEleitor(2);

            Assert.Equal(eleitor2, eleitor);
        }

        [Fact]
        public void BuscarEleitor_IdInexistente_RetornaNull()
        {
            var eleicao = CriarEleicao();
            var usuario1 = new Usuario("eleitor1@email.com", "Eleitor 1", "Cargo");
            var usuario2 = new Usuario("eleitor2@email.com", "Eleitor 2", "Cargo");
            var usuario3 = new Usuario("eleitor3@email.com", "Eleitor 3", "Cargo");

            var eleitor1 = new Eleitor(usuario1) { Id = 1 };
            var eleitor2 = new Eleitor(usuario2) { Id = 2 };
            var eleitor3 = new Eleitor(usuario3) { Id = 3 };

            eleicao.AdicionarEleitor(eleitor1);
            eleicao.AdicionarEleitor(eleitor2);
            eleicao.AdicionarEleitor(eleitor3);

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
            var eleicao = CriarEleicao();
            PassarParaEtapaInscricao(eleicao);

            // Adiciona os eleitores
            List<Eleitor> eleitores = new List<Eleitor>();
            for (int i = 0; i < 6000; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            // Faz as inscrições
            var qtdaInscricoesReprovadas = 2;
            for (int i = 0; i < qtdaInscricoesAprovadas + qtdaInscricoesPendentes + qtdaInscricoesReprovadas; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
            }

            // Aprova algumas inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 1; i <= qtdaInscricoesAprovadas; i++)
                eleicao.AprovarInscricao(i, usuarioAprovador);

            // Reprova algumas inscrições
            for (int i = 1; i <= qtdaInscricoesReprovadas; i++)
                eleicao.ReprovarInscricao(i + qtdaInscricoesAprovadas, usuarioAprovador, "Motivo");

            var excecao = Assert.Throws<CustomException>(() => eleicao.PassarParaProximaEtapa());
            Assert.Equal(mensagemErro, excecao.Message);
        }

        [Theory]
        [InlineData(7000, 3499)]
        [InlineData(7001, 3500)]
        public void PassarParaProximaEtapa_EtapaVotacaoSemQtdaMinimaVotos_ThrowCustomException(
            int qtdaEleitores, int qtdaVotos)
        {
            var eleicao = CriarEleicao();

            // Adiciona os eleitores
            List<Eleitor> eleitores = new List<Eleitor>();
            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarParaEtapaInscricao(eleicao);

            // Faz as inscrições
            var qtdaInscricoes = 27;
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            PassarParaEtapaVotacao(eleicao);

            for (int i = 0; i < qtdaVotos; i++)
                eleicao.RegistrarVoto((i % qtdaInscricoes) + 1, eleitores.ElementAt(i), "::1");

            var excecao = Assert.Throws<CustomException>(() => eleicao.PassarParaProximaEtapa());
            Assert.Equal("Esta eleição ainda não atingiu os 50% de participação de todos os funcionários, conforme exigido pela NR-5.", excecao.Message);
        }

        public static object[][] InlineDataCronograma = new object[][] {
            new object[] {
                1, PosicaoEtapa.Atual, PosicaoEtapa.Futura, PosicaoEtapa.Futura, PosicaoEtapa.Futura,
                DateTime.Today, null, null, null, null
            },
            new object[] {
                2, PosicaoEtapa.Passada, PosicaoEtapa.Atual, PosicaoEtapa.Futura, PosicaoEtapa.Futura,
                DateTime.Today, DateTime.Today, null, null, null
            },
            new object[] {
                3, PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Atual, PosicaoEtapa.Futura,
                DateTime.Today, DateTime.Today, DateTime.Today, null, null
            },
            new object[] {
                4, PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Atual,
                DateTime.Today, DateTime.Today, DateTime.Today, DateTime.Today, null
            },
             new object[] {
                5, PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Passada, PosicaoEtapa.Passada,
                DateTime.Today, DateTime.Today, DateTime.Today, DateTime.Today, DateTime.Now
            }
        };
        [Theory, MemberData(nameof(InlineDataCronograma))]
        public void PassarParaProximaEtapa_NenhumaInsconsistencia_CronogramaAtualizado(
            int passarEtapaNVezes,
            PosicaoEtapa posicaoEtapaEsperada1, PosicaoEtapa posicaoEtapaEsperada2, PosicaoEtapa posicaoEtapaEsperada3, PosicaoEtapa posicaoEtapaEsperada4,
            DateTime? dataRealizada1, DateTime? dataRealizada2, DateTime? dataRealizada3, DateTime? dataRealizada4, DateTime? dataFinalizacaoEleicao
        )
        {
            var eleicao = CriarEleicao();

            // Adiciona os eleitores
            List<Eleitor> eleitores = new List<Eleitor>();
            var qtdaEleitores = 21;
            var qtdaInscricoes = 2;
            var qtdaVotos = 15;

            for (int n = 0; n < passarEtapaNVezes; n++)
            {
                if (eleicao.EtapaAtual?.EtapaObrigatoriaId == CodigoEtapaObrigatoria.Convocacao)
                {
                    for (int i = 0; i < qtdaEleitores; i++)
                    {
                        var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                        var eleitor = new Eleitor(usuario) { Id = i };
                        eleicao.AdicionarEleitor(eleitor);
                        eleitores.Add(eleitor);
                    }
                }
                else if (eleicao.EtapaAtual?.EtapaObrigatoriaId == CodigoEtapaObrigatoria.Inscricao)
                {
                    // Faz as inscrições
                    var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
                    for (int i = 0; i < qtdaInscricoes; i++)
                    {
                        var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                        inscricao.Id = i + 1;
                        eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
                    }
                }
                else if (eleicao.EtapaAtual?.EtapaObrigatoriaId == CodigoEtapaObrigatoria.Votacao)
                {
                    // Registra os votos
                    for (int i = 0; i < qtdaVotos; i++)
                        eleicao.RegistrarVoto((i % qtdaInscricoes) + 1, eleitores.ElementAt(i), "::1");
                }
                eleicao.PassarParaProximaEtapa();
            }

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
                },
                etapa =>
                {
                    Assert.Equal(posicaoEtapaEsperada4, etapa.PosicaoEtapa);
                    Assert.Equal(dataRealizada4, etapa.DataRealizada);
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
            var eleicao = CriarEleicao();
            var usuario1 = new Usuario("eleitor1@email.com", "Eleitor 1", "Cargo");
            var usuario2 = new Usuario("eleitor2@email.com", "Eleitor 2", "Cargo");

            var eleitor1 = new Eleitor(usuario1) { Id = 1 };
            var eleitor2 = new Eleitor(usuario2) { Id = 2 };

            var dimensionamento = eleicao.Grupo.CalcularDimensionamento(3);
            eleicao.AdicionarEleitor(eleitor1);
            eleicao.AdicionarEleitor(eleitor2);

            var novoEleitor = new Eleitor(usuario1);
            var excecao = Assert.Throws<CustomException>(() => eleicao.AdicionarEleitor(novoEleitor));
            Assert.Equal("Já existe um eleitor cadastrado com o mesmo e-mail para essa eleição.", excecao.Message);
        }

        [Fact]
        public void AdicionarEleitor_EleicaoAposEtapaVotacao_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 21;
            var qtdaInscricoes = 2;
            var qtdaVotos = 15;
            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);
            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Votacao);
            // Registra os votos
            for (int i = 0; i < qtdaVotos; i++)
                eleicao.RegistrarVoto((i % qtdaInscricoes) + 1, eleitores.ElementAt(i), "::1");

            eleicao.PassarParaProximaEtapa();

            var novoEleitor = new Eleitor(new Usuario("email@email.com", "Teste", "Teste"));
            var excecao = Assert.Throws<CustomException>(() => eleicao.AdicionarEleitor(novoEleitor));
            Assert.Equal("Não é permitido cadastrar eleitores após o período de votação.", excecao.Message);
        }

        [Fact]
        public void AdicionarEleitor_DimensionamentoInvalido_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 2;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            eleicao.PassarParaProximaEtapa();

            var novoUsuario = new Usuario("teste@email.com", "Eleitor 1", "Cargo");
            var novoEleitor = new Eleitor(novoUsuario);

            var excecao = Assert.Throws<CustomException>(() => eleicao.AdicionarEleitor(novoEleitor));
            Assert.Equal("Não é possível adicionar esse novo eleitor, pois sua inclusão altera o dimensionamento da eleição e com isso a quantidade mínima de inscritos passa a ser superior à quantidade atual de inscritos.", excecao.Message);
        }

        [Fact]
        public void AdicionarEleitor_DimensionamentoValido_AtualizaDimensionamentoEleicaoAdicionaEleitor()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            eleicao.PassarParaProximaEtapa();

            var novoUsuario = new Usuario("teste@email.com", "Eleitor 1", "Cargo");
            var novoEleitor = new Eleitor(novoUsuario);

            eleicao.AdicionarEleitor(novoEleitor);
            Assert.Equal(qtdaEleitores + 1, eleicao.Eleitores.Count);
            Assert.Contains(eleicao.Eleitores, e => e.Email == "teste@email.com");
            Assert.Equal(new Dimensionamento(100, 51, 3, 3)
            {
                QtdaEleitores = qtdaEleitores + 1,
                QtdaInscricoesAprovadas = qtdaInscricoes
            }, eleicao.Dimensionamento);

        }

        [Fact]
        public void FazerInscricao_ForaPeriodoInscricoes_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();

            var novoUsuario = new Usuario("teste@email.com", "Eleitor 1", "Cargo");
            var novoEleitor = new Eleitor(novoUsuario);
            eleicao.AdicionarEleitor(novoEleitor);

            var exception = Assert.Throws<CustomException>(() => eleicao.FazerInscricao(novoEleitor, "Meus Objetivos."));
            Assert.Equal("As inscrições podem ser realizadas somente no período de inscrição. Confira o cronograma da eleição.", exception.Message);
        }

        [Fact]
        public void FazerInscricao_EleitorJaInscrito_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var novoUsuario = new Usuario("eleitor1@email.com", "Eleitor 1", "Cargo");
            var novoEleitor = new Eleitor(novoUsuario);

            var exception = Assert.Throws<CustomException>(() => eleicao.FazerInscricao(novoEleitor, "Meus Objetivos."));
            Assert.Equal("Esse eleitor já está inscrito na eleição.", exception.Message);
        }

        [Fact]
        public void AtualizarInscricao_ForaPeriodoInscricoes_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            eleicao.PassarParaProximaEtapa();

            var exception = Assert.Throws<CustomException>(() => eleicao.AtualizarInscricao(eleitores.ElementAt(0).Id, "Meus Objetivos."));
            Assert.Equal("As inscrições não podem ser alteradas fora do período de inscrição.", exception.Message);
        }

        [Fact]
        public void AtualizarInscricao_InscricaoNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var novoUsuario = new Usuario("eleitorteste@email.com", "Eleitor 1", "Cargo");
            var novoEleitor = new Eleitor(novoUsuario) { Id = 1000 };

            var exception = Assert.Throws<NotFoundException>(() => eleicao.AtualizarInscricao(novoEleitor.Id, "Meus Objetivos."));
            Assert.Equal("Inscrição não encontrada.", exception.Message);
        }

        [Fact]
        public void AtualizarInscricao_InscricaoEncontrada_InscricaoAtualizada()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var inscricaoRetornada = eleicao.AtualizarInscricao(eleitores.ElementAt(0).Id, "Meus Objetivos.");
            Assert.Equal("Meus Objetivos.", inscricaoRetornada.Objetivos);
            Assert.Equal(StatusInscricao.Pendente, inscricaoRetornada.StatusInscricao);
        }

        [Fact]
        public void ReprovarInscricao_ForaPeriodoInscricoes_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            eleicao.PassarParaProximaEtapa();

            var exception = Assert.Throws<CustomException>(() => eleicao.ReprovarInscricao(1, usuarioAprovador, "Motivo Reprovação"));
            Assert.Equal("As inscrições não podem ser reprovadas fora do período de inscrição.", exception.Message);
        }

        [Fact]
        public void ReprovarInscricao_InscricaoNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var exception = Assert.Throws<NotFoundException>(() => eleicao.ReprovarInscricao(qtdaInscricoes + 1, usuarioAprovador, "Motivo Reprovação"));
            Assert.Equal("Inscrição não encontrada.", exception.Message);
        }

        [Fact]
        public void ReprovarInscricao_InscricaoEncontrada_AdicionaReprovacao()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var inscricaoRetornada = eleicao.ReprovarInscricao(1, usuarioAprovador, "Foto indevida");

            Assert.Equal(StatusInscricao.Reprovada, inscricaoRetornada.StatusInscricao);
            Assert.Collection(inscricaoRetornada.Reprovacoes,
                reprovacao =>
                {
                    Assert.Equal("aprovador@email.com", reprovacao.EmailAprovador);
                    Assert.Equal("Aprovador", reprovacao.NomeAprovador);
                    Assert.Equal("Foto indevida", reprovacao.MotivoReprovacao);
                }
            );
        }

        [Fact]
        public void AprovarInscricao_ForaPeriodoInscricoes_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            eleicao.PassarParaProximaEtapa();
            var exception = Assert.Throws<CustomException>(() => eleicao.AprovarInscricao(1, usuarioAprovador));
            Assert.Equal("As inscrições não podem ser aprovadas fora do período de inscrição.", exception.Message);
        }

        [Fact]
        public void AprovarInscricao_InscricaoNaoEncontrada_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var exception = Assert.Throws<NotFoundException>(() => eleicao.AprovarInscricao(qtdaInscricoes + 1, usuarioAprovador));
            Assert.Equal("Inscrição não encontrada.", exception.Message);
        }

        [Fact]
        public void AprovarInscricao_InscricaoEncontrada_AtualizaInscricao()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 50;
            var qtdaInscricoes = 6;

            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);

            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var inscricaoRetornada = eleicao.AprovarInscricao(1, usuarioAprovador);

            Assert.Equal(StatusInscricao.Aprovada, inscricaoRetornada.StatusInscricao);
            Assert.Equal("aprovador@email.com", inscricaoRetornada.EmailAprovador);
            Assert.Equal("Aprovador", inscricaoRetornada.NomeAprovador);

        }

        [Fact]
        public void RegistrarVoto_EleitorJaVotou_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 21;
            var qtdaInscricoes = 2;
            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);
            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Votacao);

            eleicao.RegistrarVoto(1, eleitores.ElementAt(0), "::1");
            var exception = Assert.Throws<CustomException>(() => eleicao.RegistrarVoto(1, eleitores.ElementAt(0), "::1"));
            Assert.Equal("Eleitor já registrou seu voto nessa eleição. Não é possível votar mais de uma vez.", exception.Message);
        }

        [Fact]
        public void RegistrarVoto_EleicaoForaDoPeriodoDeVotacao_ThrowsCustomException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 21;
            var qtdaInscricoes = 2;
            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);
            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var exception = Assert.Throws<CustomException>(() => eleicao.RegistrarVoto(1, eleitores.ElementAt(0), "::1"));
            Assert.Equal("Essa eleição não está no período de votação.", exception.Message);
        }

        [Fact]
        public void RegistrarVoto_InscricaoNaoEncontrada_ThrowsNotFoundException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 21;
            var qtdaInscricoes = 2;
            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);
            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Votacao);

            var exception = Assert.Throws<NotFoundException>(() => eleicao.RegistrarVoto(qtdaInscricoes + 1, eleitores.ElementAt(0), "::1"));
            Assert.Equal("Inscrição não encontrada.", exception.Message);
        }

        [Fact]
        public void RegistrarVoto_InscricaoNaoAprovada_ThrowNotFoundException()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 21;
            var qtdaInscricoes = 2;
            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);
            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            var inscricaoReprovada = eleicao.FazerInscricao(eleitores.ElementAt(qtdaInscricoes), "Objetivos");
            inscricaoReprovada.Id = qtdaInscricoes + 1;
            eleicao.ReprovarInscricao(inscricaoReprovada.Id, usuarioAprovador, "Motivo Reprovação");

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Votacao);

            var exception = Assert.Throws<NotFoundException>(() => eleicao.RegistrarVoto(inscricaoReprovada.Id, eleitores.ElementAt(0), "::1"));
            Assert.Equal("Inscrição não encontrada.", exception.Message);

        }

        [Fact]
        public void RegistrarVoto_VotoValido_VotoRegistrado()
        {
            var eleicao = CriarEleicao();
            var qtdaEleitores = 21;
            var qtdaInscricoes = 2;
            List<Eleitor> eleitores = new List<Eleitor>();

            for (int i = 0; i < qtdaEleitores; i++)
            {
                var usuario = new Usuario($"eleitor{i}@email.com", $"Eleitor {i}", "Cargo");
                var eleitor = new Eleitor(usuario) { Id = i };
                eleicao.AdicionarEleitor(eleitor);
                eleitores.Add(eleitor);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Inscricao);
            // Faz as inscrições
            var usuarioAprovador = new Usuario("aprovador@email.com", "Aprovador", "Aprovador");
            for (int i = 0; i < qtdaInscricoes; i++)
            {
                var inscricao = eleicao.FazerInscricao(eleitores.ElementAt(i), "Objetivos");
                inscricao.Id = i + 1;
                eleicao.AprovarInscricao(inscricao.Id, usuarioAprovador);
            }

            PassarEtapaAte(eleicao, CodigoEtapaObrigatoria.Votacao);

            var votoRetornado = eleicao.RegistrarVoto(2, eleitores.ElementAt(0), "::1");
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