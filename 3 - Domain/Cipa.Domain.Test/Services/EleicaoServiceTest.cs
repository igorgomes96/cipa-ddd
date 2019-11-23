// using System;
// using Cipa.Domain.Entities;
// using Cipa.Domain.Exceptions;
// using Cipa.Domain.Helpers;
// using Cipa.Domain.Interfaces.Repositories;
// using Cipa.Domain.Services;
// using Moq;
// using Xunit;

// namespace Cipa.Domain.Test.Services
// {
//     public class EleicaoServiceTest
//     {
//         private readonly Mock<IEleicaoRepository> eleicaoRepositoryMoq;
//         private readonly Mock<IEstabelecimentoRepository> estabelecimentoRepositoryMoq;
//         private readonly Mock<IUsuarioRepository> usuarioRepositoryMoq;
//         private readonly Mock<IContaRepository> contaRepositoryMoq;
//         private readonly Mock<IGrupoRepository> grupoRepositoryMoq;
//         private readonly Mock<IUnitOfWork> unitOfWorkMoq;
//         private readonly EleicaoService eleicaoService;
//         private readonly Eleicao eleicao;
//         public EleicaoServiceTest()
//         {
//             eleicao = new Eleicao(1, new DateTime(2019, 1, 1), 2, 1, 1, 1, 1, new DateTime(2019, 2, 28));
//             eleicaoRepositoryMoq = new Mock<IEleicaoRepository>();
//             estabelecimentoRepositoryMoq = new Mock<IEstabelecimentoRepository>();
//             usuarioRepositoryMoq = new Mock<IUsuarioRepository>();
//             contaRepositoryMoq = new Mock<IContaRepository>();
//             grupoRepositoryMoq = new Mock<IGrupoRepository>();
//             unitOfWorkMoq = new Mock<IUnitOfWork>();
//             unitOfWorkMoq.Setup(u => u.EleicaoRepository).Returns(eleicaoRepositoryMoq.Object);
//             unitOfWorkMoq.Setup(u => u.EstabelecimentoRepository).Returns(estabelecimentoRepositoryMoq.Object);
//             unitOfWorkMoq.Setup(u => u.UsuarioRepository).Returns(usuarioRepositoryMoq.Object);
//             unitOfWorkMoq.Setup(u => u.ContaRepository).Returns(contaRepositoryMoq.Object);
//             unitOfWorkMoq.Setup(u => u.GrupoRepository).Returns(grupoRepositoryMoq.Object);
//             eleicaoService = new EleicaoService(unitOfWorkMoq.Object);
//         }

//         [Fact]
//         public void Adicionar_MaisDe1EleicaoPorEstabelecimentoNoAno_ThrowsException()
//         {
//             estabelecimentoRepositoryMoq
//                 .Setup(e => e.QuantidadeEleicoesAno(It.IsAny<int>(), It.IsAny<int>()))
//                 .Returns(1);
//             estabelecimentoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Estabelecimento());
//             contaRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Conta());
//             grupoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Grupo());

//             var exception = Assert.Throws<CustomException>(() => eleicaoService.Adicionar(eleicao));
//             Assert.Equal("Já há uma eleição cadastrada para este estabelecimento no ano de 2019.", exception.Message);
//         }

//         [Fact]
//         public void Adicionar_NenhumaEleicaoPorEstabelecimentoNoAno_AdicionaEleicao()
//         {
//             estabelecimentoRepositoryMoq
//                 .Setup(e => e.QuantidadeEleicoesAno(It.IsAny<int>(), It.IsAny<int>()))
//                 .Returns(0);
//             eleicaoRepositoryMoq
//                 .Setup(e => e.Adicionar(It.IsAny<Eleicao>()))
//                 .Returns(eleicao);

//             estabelecimentoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Estabelecimento());
//             contaRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Conta());
//             grupoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Grupo());

//             var eleicaoAdicionada = eleicaoService.Adicionar(eleicao);
//             Assert.NotNull(eleicaoAdicionada);
//         }

//         [Theory]
//         [InlineData(5000, 1000, 12, 9)]
//         [InlineData(5001, 2000, 15, 12)]
//         [InlineData(10000, 3000, 15, 12)]
//         [InlineData(10001, 4000, 17, 15)]
//         [InlineData(12500, 5000, 17, 15)]
//         [InlineData(12501, 6000, 19, 18)]
//         public void CalcularDimensionamentoEleicao_EleicaoConsistente_RetornaDimensionamento(
//             int qtdaEleitores, int qtdaVotos, int qtdaEfetivos, int qtdaSuplentes
//         )
//         {
//             eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(qtdaEleitores);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(qtdaVotos);

//             var grupo = new Grupo();
//             grupo.Dimensionamentos.Add(new LinhaDimensionamento(5000, 2501, 12, 9));
//             grupo.Dimensionamentos.Add(new LinhaDimensionamento(10000, 5001, 15,12));
//             grupo.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };

//             eleicao.Grupo = grupo;

//             var dimensionamento = eleicaoService.CalcularDimensionamentoEleicao(eleicao);

//             Assert.Equal(qtdaEleitores, dimensionamento.QtdaEleitores);
//             Assert.Equal(qtdaVotos, dimensionamento.QtdaVotos);
//             Assert.Equal(qtdaEfetivos, dimensionamento.QtdaEfetivos);
//             Assert.Equal(qtdaSuplentes, dimensionamento.QtdaSuplentes);
//         }

//         [Fact]
//         public void Atualizar_MudancaGrupoQtdaInscricoesInsuficientes_ThrowsCustomException()
//         {
//             var grupoOld = new Grupo { Id = 1, CodigoGrupo = "C-1" };
//             grupoOld.Dimensionamentos.Add(new LinhaDimensionamento(5000, 2501, 2, 2));
//             grupoOld.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };
//             var grupoNew = new Grupo { Id = 2, CodigoGrupo = "C-2" };
//             grupoNew.Dimensionamentos.Add(new LinhaDimensionamento(5000, 2501, 3, 3));
//             grupoNew.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };

//             var eleicaoOld = eleicao;
//             eleicaoOld.Grupo = grupoOld;
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Reprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Pendente });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Reprovada });
//             eleicaoOld.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
//             eleicaoOld.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });

//             var eleicaoNew = new Eleicao(1, new DateTime(2019, 1, 1), 2, 1, grupoNew.Id, 1, 1, new DateTime(2019, 2, 28));
//             eleicaoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(eleicaoOld);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(3000);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);

//             estabelecimentoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Estabelecimento());
//             contaRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Conta());
//             grupoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(grupoNew);

//             var exception = Assert.Throws<CustomException>(() => eleicaoService.Atualizar(eleicaoNew));
//             Assert.Equal("Para o grupo C-2, o mínimo de inscrições necessária é 6, e só houveram 3 inscrições aprovadas nessa eleição.", exception.Message);

//         }

//         [Fact]
//         public void Atualizar_MudancaGrupoQtdaInscricoesSuficientes_AlteraDimensionamento()
//         {
//             var grupoOld = new Grupo { Id = 1, CodigoGrupo = "C-1" };
//             grupoOld.Dimensionamentos.Add(new LinhaDimensionamento(5000, 2501, 1, 2));
//             grupoOld.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };
//             var grupoNew = new Grupo { Id = 2, CodigoGrupo = "C-2" };
//             grupoNew.Dimensionamentos.Add(new LinhaDimensionamento(5001, 2502, 3, 4));
//             grupoNew.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10000, IntervaloAcrescimo = 2500, AcrescimoEfetivos = 2, AcrescimoSuplentes = 3 };

//             var eleicaoOld = eleicao;
//             eleicaoOld.Grupo = grupoOld;
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Inscricoes.Add(new Inscricao { StatusInscricao = StatusInscricao.Aprovada });
//             eleicaoOld.Cronograma.Add(new EtapaCronograma { Ordem = 1, PosicaoEtapa = PosicaoEtapa.Passada, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Inscricao });
//             eleicaoOld.Cronograma.Add(new EtapaCronograma { Ordem = 2, PosicaoEtapa = PosicaoEtapa.Atual, EtapaObrigatoriaId = CodigoEtapaObrigatoria.Votacao });

//             var eleicaoNew = new Eleicao(1, new DateTime(2019, 1, 1), 2, 1, grupoNew.Id, 1, 1, new DateTime(2019, 2, 28));
//             eleicaoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(eleicaoOld);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(3000);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);
//             eleicaoRepositoryMoq.Setup(e => e.Atualizar(It.IsAny<Eleicao>())).Verifiable();

//             estabelecimentoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Estabelecimento());
//             contaRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(new Conta());
//             grupoRepositoryMoq.Setup(e => e.BuscarPeloId(It.IsAny<int>())).Returns(grupoNew);

//             var eleicaoAtualizada = eleicaoService.Atualizar(eleicaoNew);

//             Assert.Equal(3, eleicaoAtualizada.Dimensionamento.QtdaEfetivos);
//             Assert.Equal(4, eleicaoAtualizada.Dimensionamento.QtdaSuplentes);
//             Assert.Equal(2502, eleicaoAtualizada.Dimensionamento.Minimo);
//             Assert.Equal(5001, eleicaoAtualizada.Dimensionamento.Maximo);

//         }

//         [Fact]
//         public void ExcluirEleitor_EleitorNaoEncontrado_ThrowsNotFoundException()
//         {
//             var grupo = new Grupo { Id = 1, CodigoGrupo = "C-1" };
//             grupo.Dimensionamentos.Add(new LinhaDimensionamento(10, 0, 1, 1));
//             grupo.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10, IntervaloAcrescimo = 2, AcrescimoEfetivos = 1, AcrescimoSuplentes = 1 };
//             eleicao.Grupo = grupo;

//             eleicaoRepositoryMoq.Setup(e => e.BuscarPeloIdCarregarTodoAgregado(It.IsAny<int>())).Returns(eleicao);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(1);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);

//             var exception = Assert.Throws<NotFoundException>(() => eleicaoService.ExcluirEleitor(1, 1));
//             Assert.Equal("Eleitor não encontrado.", exception.Message);
//         }

//         [Theory]
//         [InlineData(StatusInscricao.Aprovada)]
//         [InlineData(StatusInscricao.Pendente)]
//         public void ExcluirEleitor_EleitorInscritoAprovadoOuPendente_ThrowsCustomException(StatusInscricao statusInscricao)
//         {
//             var eleitor = new Eleitor { Id = 1 };
//             eleicao.Eleitores.Add(eleitor);
//             eleicao.Inscricoes.Add(new Inscricao { EleitorId = eleitor.Id, Eleitor = eleitor, StatusInscricao = statusInscricao });

//             var grupo = new Grupo { Id = 1, CodigoGrupo = "C-1" };
//             grupo.Dimensionamentos.Add(new LinhaDimensionamento(10, 0, 1, 1));
//             grupo.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10, IntervaloAcrescimo = 2, AcrescimoEfetivos = 1, AcrescimoSuplentes = 1 };
//             eleicao.Grupo = grupo;

//             eleicaoRepositoryMoq.Setup(e => e.BuscarPeloIdCarregarTodoAgregado(It.IsAny<int>())).Returns(eleicao);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(1);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);

//             var exception = Assert.Throws<CustomException>(() => eleicaoService.ExcluirEleitor(1, 1));
//             Assert.Equal("Não é possível excluir esse eleitor pois ele é um dos inscritos nessa eleição!", exception.Message);
//         }

//         [Fact]
//         public void ExcluirEleitor_InscricaoReprovadaEleitorJaVotou_ThrowsCustomException()
//         {
//             var eleitor = new Eleitor { Id = 1 };
//             eleicao.Eleitores.Add(eleitor);
//             eleicao.Inscricoes.Add(new Inscricao { EleitorId = eleitor.Id, Eleitor = eleitor, StatusInscricao = StatusInscricao.Reprovada });
//             eleicao.Votos.Add(new Voto(eleitor, "::1"));

//             var grupo = new Grupo { Id = 1, CodigoGrupo = "C-1" };
//             grupo.Dimensionamentos.Add(new LinhaDimensionamento(10, 0, 1, 1));
//             grupo.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10, IntervaloAcrescimo = 2, AcrescimoEfetivos = 1, AcrescimoSuplentes = 1 };
//             eleicao.Grupo = grupo;

//             eleicaoRepositoryMoq.Setup(e => e.BuscarPeloIdCarregarTodoAgregado(It.IsAny<int>())).Returns(eleicao);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(1);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);

//             var exception = Assert.Throws<CustomException>(() => eleicaoService.ExcluirEleitor(1, 1));
//             Assert.Equal("Não é possível excluir esse eleitor pois ele já votou nessa eleição!", exception.Message);
//         }

//         [Fact]
//         public void ExcluirEleitor_EleitorNaoInscritoJaVotou_ThrowsCustomException()
//         {
//             var eleitor = new Eleitor { Id = 1 };
//             eleicao.Eleitores.Add(eleitor);
//             eleicao.Votos.Add(new Voto(eleitor, "::1"));

//             var grupo = new Grupo { Id = 1, CodigoGrupo = "C-1" };
//             grupo.Dimensionamentos.Add(new LinhaDimensionamento(10, 0, 1, 1));
//             grupo.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10, IntervaloAcrescimo = 2, AcrescimoEfetivos = 1, AcrescimoSuplentes = 1 };
//             eleicao.Grupo = grupo;

//             eleicaoRepositoryMoq.Setup(e => e.BuscarPeloIdCarregarTodoAgregado(It.IsAny<int>())).Returns(eleicao);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(1);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);

//             var exception = Assert.Throws<CustomException>(() => eleicaoService.ExcluirEleitor(1, 1));
//             Assert.Equal("Não é possível excluir esse eleitor pois ele já votou nessa eleição!", exception.Message);
//         }

//         [Fact]
//         public void ExcluirEleitor_EleitorPodeSerExcluido_RetornaTrueExcluiEleitor()
//         {
//             eleicao.Eleitores.Add(new Eleitor { Id = 1 });
//             eleicao.Eleitores.Add(new Eleitor { Id = 2 });
//             eleicao.Eleitores.Add(new Eleitor { Id = 3 });

//             var grupo = new Grupo { Id = 1, CodigoGrupo = "C-1" };
//             grupo.Dimensionamentos.Add(new LinhaDimensionamento(10, 0, 1, 1));
//             grupo.LimiteDimensionamento = new LimiteDimensionamento { Limite = 10, IntervaloAcrescimo = 2, AcrescimoEfetivos = 1, AcrescimoSuplentes = 1 };
//             eleicao.Grupo = grupo;

//             var serviceBaseMoq = new Mock<ServiceBase<Eleicao>>();
//             serviceBaseMoq.Setup(s => s.Atualizar(It.IsAny<Eleicao>())).Returns(eleicao);

//             eleicaoRepositoryMoq.Setup(e => e.BuscarPeloIdCarregarTodoAgregado(It.IsAny<int>())).Returns(eleicao);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaEleitores(It.IsAny<Eleicao>())).Returns(3);
//             eleicaoRepositoryMoq.Setup(e => e.QtdaVotos(It.IsAny<Eleicao>())).Returns(0);

//             var retorno = eleicaoService.ExcluirEleitor(1, 2);
//             Assert.True(retorno);
//             Assert.Collection(eleicao.Eleitores,
//                 item =>
//                 {
//                     Assert.Equal(1, item.Id);
//                 },
//                 item =>
//                 {
//                     Assert.Equal(3, item.Id);
//                 }
//             );
//         }
//     }
// }