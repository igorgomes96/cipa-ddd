using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Services;
using Moq;
using Xunit;

namespace Cipa.Domain.Test.Services
{
    public class EleicaoServiceTest
    {
        private readonly Mock<IEleicaoRepository> eleicaoRepositoryMoq;
        private readonly Mock<IEstabelecimentoRepository> estabelecimentoRepositoryMoq;
        private readonly EleicaoService eleicaoService;
        public EleicaoServiceTest()
        {
            eleicaoRepositoryMoq = new Mock<IEleicaoRepository>();
            estabelecimentoRepositoryMoq = new Mock<IEstabelecimentoRepository>();
            eleicaoService = new EleicaoService(eleicaoRepositoryMoq.Object, estabelecimentoRepositoryMoq.Object);
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
        ) {
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
        public void Atualizar_MudancaGrupoQtdaInscricoesInsuficientes_ThrowsCustomException() {
            
        }
    }
}