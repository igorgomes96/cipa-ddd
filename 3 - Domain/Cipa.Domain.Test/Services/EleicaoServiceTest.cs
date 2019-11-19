using System;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Interfaces.Services;
using Cipa.Domain.Services;
using Moq;
using Xunit;

namespace Cipa.Domain.Test.Services
{
    public class EleicaoServiceTest
    {
        private readonly Mock<IEleicaoRepository> eleicaoRepositoryMoq;
        private readonly Mock<IEstabelecimentoRepository> estabelecimentoRepositoryMoq;
        private readonly Mock<IUsuarioService> usuarioServiceMoq;
        private readonly EleicaoService eleicaoService;
        public EleicaoServiceTest()
        {
            eleicaoRepositoryMoq = new Mock<IEleicaoRepository>();
            estabelecimentoRepositoryMoq = new Mock<IEstabelecimentoRepository>();
            usuarioServiceMoq = new Mock<IUsuarioService>();
            eleicaoService = new EleicaoService(eleicaoRepositoryMoq.Object, estabelecimentoRepositoryMoq.Object, usuarioServiceMoq.Object);
        }

        [Fact]
        public void Adicionar_MaisDe1EleicaoPorEstabelecimentoNoAno_ThrowsException()
        {
            estabelecimentoRepositoryMoq
                .Setup(e => e.QuantidadeEleicoesAno(It.IsAny<Estabelecimento>(), It.IsAny<int>()))
                .Returns(1);
            var exception = Assert.Throws<CustomException>(() => eleicaoService.Adicionar(new Eleicao { Gestao = 2019 }));
            Assert.Equal("Já há uma eleição cadastrada para este estabelecimento no ano de 2019.", exception.Message);
        }

        [Fact]
        public void Adicionar_NenhumaEleicaoPorEstabelecimentoNoAno_AdicionaEleicao()
        {
            estabelecimentoRepositoryMoq
                .Setup(e => e.QuantidadeEleicoesAno(It.IsAny<Estabelecimento>(), It.IsAny<int>()))
                .Returns(0);
            eleicaoRepositoryMoq
                .Setup(e => e.Adicionar(It.IsAny<Eleicao>()))
                .Returns(new Eleicao());

            var eleicaoAdicionada = eleicaoService.Adicionar(new Eleicao { Conta = new Conta() });
            Assert.NotNull(eleicaoAdicionada);
        }

        [Theory]
        [InlineData(5000, 1000, 12, 9)]
        [InlineData(5001, 2000, 15, 12)]
        [InlineData(10000, 3000, 15, 12)]
        [InlineData(10001, 4000, 17, 15)]
        [InlineData(12500, 5000, 17, 15)]
        [InlineData(12501, 6000, 19, 18)]
        public void CalcularDimensionamentoEleicao_EleicaoConsistente_RetornaDimensionamento(
            int qtdaEleitores, int qtdaVotos, int qtdaEfetivos, int qtdaSuplentes
        )
        {
            eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(qtdaEleitores);
            eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(qtdaVotos);

            var grupo = new Grupo();
            grupo.Dimensionamentos.Add(new LinhaDimensionamento { Minimo = 2501, Maximo = 5000, QtdaEfetivos = 12, QtdaSuplentes = 9 });
            grupo.Dimensionamentos.Add(new LinhaDimensionamento { Minimo = 5001, Maximo = 10000, QtdaEfetivos = 15, QtdaSuplentes = 12 });
            grupo.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };

            var eleicao = new Eleicao();
            eleicao.Grupo = grupo;

            var dimensionamento = eleicaoService.CalcularDimensionamentoEleicao(eleicao);

            Assert.Equal(qtdaEleitores, dimensionamento.QtdaEleitores);
            Assert.Equal(qtdaVotos, dimensionamento.QtdaVotos);
            Assert.Equal(qtdaEfetivos, dimensionamento.QtdaEfetivos);
            Assert.Equal(qtdaSuplentes, dimensionamento.QtdaSuplentes);
        }

        [Fact]
        public void Atualizar_MudancaGrupoQtdaInscricoesInsuficientes_ThrowsCustomException()
        {
            var grupoOld = new Grupo { Id = 1, CodigoGrupo = "C-1" };
            grupoOld.Dimensionamentos.Add(new LinhaDimensionamento { Minimo = 2501, Maximo = 5000, QtdaEfetivos = 2, QtdaSuplentes = 2 });
            grupoOld.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };
            var grupoNew = new Grupo { Id = 2, CodigoGrupo = "C-2" };
            grupoNew.Dimensionamentos.Add(new LinhaDimensionamento { Minimo = 2501, Maximo = 5000, QtdaEfetivos = 3, QtdaSuplentes = 3 });
            grupoNew.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };
            
            var eleicaoOld = new Eleicao();
            eleicaoOld.Grupo = grupoOld;
            eleicaoOld.GrupoId = grupoOld.Id;
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Reprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Pendente });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Reprovada });
            eleicaoOld.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicaoOld.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            
            var eleicaoNew = new Eleicao();
            eleicaoNew.Grupo = grupoNew;
            eleicaoNew.GrupoId = grupoNew.Id;

            eleicaoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(eleicaoOld);
            eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(3000);
            eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);

            var exception = Assert.Throws<CustomException>(() => eleicaoService.Atualizar(eleicaoNew));
            Assert.Equal("Para o grupo C-2, o mínimo de inscrições necessária é 6, e só houveram 3 inscrições aprovadas nessa eleição.", exception.Message);

        }

        [Fact]
        public void Atualizar_MudancaGrupoQtdaInscricoesSuficientes_AlteraDimensionamento()
        {
            var grupoOld = new Grupo { Id = 1, CodigoGrupo = "C-1" };
            grupoOld.Dimensionamentos.Add(new LinhaDimensionamento { Minimo = 2501, Maximo = 5000, QtdaEfetivos = 1, QtdaSuplentes = 2 });
            grupoOld.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };
            var grupoNew = new Grupo { Id = 2, CodigoGrupo = "C-2" };
            grupoNew.Dimensionamentos.Add(new LinhaDimensionamento { Minimo = 2502, Maximo = 5001, QtdaEfetivos = 3, QtdaSuplentes = 4 });
            grupoNew.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };
            
            var eleicaoOld = new Eleicao();
            eleicaoOld.Grupo = grupoOld;
            eleicaoOld.GrupoId = grupoOld.Id;
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
            eleicaoOld.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
            eleicaoOld.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });
            
            var eleicaoNew = new Eleicao();
            eleicaoNew.Grupo = grupoNew;
            eleicaoNew.GrupoId = grupoNew.Id;

            eleicaoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(eleicaoOld);
            eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(3000);
            eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);
            eleicaoRepositoryMoq.Setup(e => e.Atualizar(It.IsAny<Eleicao>())).Verifiable();

            var eleicaoAtualizada = eleicaoService.Atualizar(eleicaoNew);

            Assert.Equal(3, eleicaoAtualizada.Dimensionamento.QtdaEfetivos);
            Assert.Equal(4, eleicaoAtualizada.Dimensionamento.QtdaSuplentes);
            Assert.Equal(2502, eleicaoAtualizada.Dimensionamento.Minimo);
            Assert.Equal(5001, eleicaoAtualizada.Dimensionamento.Maximo);

        }

        [Fact]
        public void ExcluirEleitor_EleitorNaoEncontrado_ThrowsNotFoundException() {
            eleicaoRepositoryMoq.Setup(e => e.BuscarEleitor(It.IsAny<Eleicao>(), It.IsAny<int>()))
                .Returns((Eleitor)null);

            var exception = Assert.Throws<NotFoundException>(() => eleicaoService.ExcluirEleitor(new Eleicao(), 1));
            Assert.Equal("Eleitor não encontrado.", exception.Message);
        }
        
        [Theory]
        [InlineData(StatusInscricao.Aprovada)]
        [InlineData(StatusInscricao.Pendente)]
        public void ExcluirEleitor_EleitorInscritoAprovadoOuPendente_ThrowsCustomException(StatusInscricao statusInscricao) {
            eleicaoRepositoryMoq.Setup(e => e.BuscarEleitor(It.IsAny<Eleicao>(), It.IsAny<int>()))
                .Returns(new Eleitor {
                    Inscricao = new Inscricao { StatusInscricao = statusInscricao }
                });
            
            var exception = Assert.Throws<CustomException>(() => eleicaoService.ExcluirEleitor(new Eleicao(), 1));
            Assert.Equal("Não é possível excluir esse eleitor pois ele é um dos inscritos nessa eleição!", exception.Message);
        }

        [Fact]
        public void ExcluirEleitor_InscricaoReprovadaEleitorJaVotou_ThrowsCustomException() {
            eleicaoRepositoryMoq.Setup(e => e.BuscarEleitor(It.IsAny<Eleicao>(), It.IsAny<int>()))
                .Returns(new Eleitor {
                    Inscricao = new Inscricao { StatusInscricao = StatusInscricao.Reprovada },
                    Voto = new Voto { Id = 1 }
                });
            
            var exception = Assert.Throws<CustomException>(() => eleicaoService.ExcluirEleitor(new Eleicao(), 1));
            Assert.Equal("Não é possível excluir esse eleitor pois ele já votou nessa eleição!", exception.Message);
        }

        [Fact]
        public void ExcluirEleitor_EleitorNaoInscritoJaVotou_ThrowsCustomException() {
            eleicaoRepositoryMoq.Setup(e => e.BuscarEleitor(It.IsAny<Eleicao>(), It.IsAny<int>()))
                .Returns(new Eleitor {
                    Inscricao = null,
                    Voto = new Voto { Id = 1 }
                });
            
            var exception = Assert.Throws<CustomException>(() => eleicaoService.ExcluirEleitor(new Eleicao(), 1));
            Assert.Equal("Não é possível excluir esse eleitor pois ele já votou nessa eleição!", exception.Message);
        }

        [Fact]
        public void ExcluirEleitor_EleitorPodeSerExcluido_RetornaTrueExcluiEleitor() {
            eleicaoRepositoryMoq.Setup(e => e.BuscarEleitor(It.IsAny<Eleicao>(), It.IsAny<int>()))
                .Returns(new Eleitor {
                    Id = 2,
                    Inscricao = null,
                    Voto = null
                });
            
            var eleicao = new Eleicao();
            eleicao.Eleitores.Add(new Eleitor { Id = 1 });
            eleicao.Eleitores.Add(new Eleitor { Id = 2 });
            eleicao.Eleitores.Add(new Eleitor { Id = 3 });

            var serviceBaseMoq = new Mock<ServiceBase<Eleicao>>();
            serviceBaseMoq.Setup(s => s.Atualizar(It.IsAny<Eleicao>())).Returns(eleicao);

            var retorno = eleicaoService.ExcluirEleitor(eleicao, 2);
            Assert.True(retorno);
            Assert.Collection(eleicao.Eleitores,
                item => {
                    Assert.Equal(1, item.Id);
                },
                item => {
                    Assert.Equal(3, item.Id);
                }
            );
        }
    }
}