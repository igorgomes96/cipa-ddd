using System;
using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Interfaces.Services;

namespace Cipa.Domain.Services
{
    public class EleicaoService : ServiceBase<Eleicao>, IEleicaoService
    {
        private readonly IEleicaoRepository _eleicaoRepository;
        public EleicaoService(IUnitOfWork unitOfWork) : base(unitOfWork.EleicaoRepository, unitOfWork)
        {
            _eleicaoRepository = (IEleicaoRepository)_repository;
        }

        public override Eleicao Adicionar(Eleicao eleicao)
        {
            ValidaQtdaEleicoesPorEstabelecimento(eleicao);

            var estabelecimento = _unitOfWork.EstabelecimentoRepository.BuscarPeloId(eleicao.EstabelecimentoId);
            if (estabelecimento == null)
                throw new NotFoundException($"Estabelecimento {eleicao.EstabelecimentoId} não encontrado.");

            var conta = _unitOfWork.ContaRepository.BuscarPeloId(eleicao.ContaId);
            if (conta == null)
                throw new NotFoundException($"Conta {eleicao.ContaId} não encontrada.");

            var grupo = _unitOfWork.GrupoRepository.BuscarPeloId(eleicao.GrupoId);
            if (grupo == null)
                throw new NotFoundException($"Grupo {eleicao.GrupoId} não encontrado.");

            eleicao.Conta = conta;
            eleicao.Grupo = grupo;
            eleicao.Estabelecimento = estabelecimento;
            eleicao.GerarCronograma();

            return base.Adicionar(eleicao);
        }

        private void ValidaQtdaEleicoesPorEstabelecimento(Eleicao eleicao)
        {
            int qtdaEleicoes = _unitOfWork.EstabelecimentoRepository.QuantidadeEleicoesAno(eleicao.EstabelecimentoId, eleicao.Gestao);
            if (qtdaEleicoes > 0)
                throw new CustomException($"Já há uma eleição cadastrada para este estabelecimento no ano de {eleicao.Gestao}.");
        }

        public override Eleicao Atualizar(Eleicao eleicaoAtualizada)
        {
            Eleicao eleicao = _eleicaoRepository.BuscarPeloId(eleicaoAtualizada.Id);
            if (eleicao == null) throw new NotFoundException("Código de eleição não encontrado.");

            if (eleicao.Gestao != eleicaoAtualizada.Gestao)
                ValidaQtdaEleicoesPorEstabelecimento(eleicaoAtualizada);

            eleicao.DataInicio = eleicaoAtualizada.DataInicio;
            eleicao.TerminoMandatoAnterior = eleicaoAtualizada.TerminoMandatoAnterior;
            eleicao.DuracaoGestao = eleicaoAtualizada.DuracaoGestao;

            if (eleicao.GrupoId != eleicaoAtualizada.GrupoId)
            {
                var novoGrupo = _unitOfWork.GrupoRepository.BuscarPeloId(eleicaoAtualizada.GrupoId);
                if (novoGrupo == null)
                    throw new NotFoundException($"Grupo {eleicao.GrupoId} não encontrado.");

                if (eleicao.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Inscricao))
                {
                    var dimensionamentoAtual = CalcularDimensionamentoEleicao(eleicao); 
                    var novoDimensionamento = novoGrupo.CalcularDimensionamento(dimensionamentoAtual.QtdaEleitores);
                    if (novoDimensionamento.TotalCipeiros > dimensionamentoAtual.QtdaInscricoesAprovadas)
                        throw new CustomException($"Para o grupo {novoGrupo.CodigoGrupo}, o mínimo de inscrições necessária é {novoDimensionamento.TotalCipeiros}, e só houveram {dimensionamentoAtual.QtdaInscricoesAprovadas} inscrições aprovadas nessa eleição.");

                    eleicao.AtualizarDimensionamento(novoDimensionamento);
                }
                eleicao.Grupo = novoGrupo;
            }

            return base.Atualizar(eleicao);
        }

        public Eleicao PassarParaProximaEtapa(Eleicao eleicao)
        {
            var dimensionamento = CalcularDimensionamentoEleicao(eleicao);
            eleicao.PassarParaProximaEtapa(dimensionamento);
            return base.Atualizar(eleicao);
        }

        public IEnumerable<Eleicao> BuscarPelaConta(Conta conta) => _eleicaoRepository.BuscarPelaConta(conta);

        public IEnumerable<Eleicao> BuscarPeloUsuario(Usuario usuario) => _eleicaoRepository.BuscarPeloUsuario(usuario);

        public override void Excluir(Eleicao eleicao)
        {
            if (eleicao.EtapaAtual != null && !eleicao.DataFinalizacao.HasValue)
                throw new CustomException("Um eleição só pode ser excluída antes do início do processo.");
            base.Excluir(eleicao);
        }

        public Dimensionamento CalcularDimensionamentoEleicao(Eleicao eleicao)
        {
            var qtdaEleitores = _eleicaoRepository.QtdaEleitores(eleicao);
            var qtdaVotos = _eleicaoRepository.QtdaVotos(eleicao);
            return CalcularDimensionamentoEleicao(eleicao, qtdaEleitores, qtdaVotos);
        }

        private Dimensionamento CalcularDimensionamentoEleicao(Eleicao eleicao, int qtdaEleitores, int qtdaVotos)
        {
            var linhaDimensionamento = eleicao.Grupo.CalcularDimensionamento(qtdaEleitores);
            return new Dimensionamento(linhaDimensionamento)
            {
                QtdaEleitores = qtdaEleitores,
                QtdaVotos = qtdaVotos,
                QtdaInscricoesAprovadas = eleicao.QtdaInscricoes(StatusInscricao.Aprovada),
                QtdaInscricoesPendentes = eleicao.QtdaInscricoes(StatusInscricao.Pendente),
                QtdaInscricoesReprovadas = eleicao.QtdaInscricoes(StatusInscricao.Reprovada),
            };
        }

        public Eleicao BuscarPeloIdCarregarEleitores(int id) => _eleicaoRepository.BuscarPeloIdCarregarEleitores(id);

        public IEnumerable<Eleitor> BuscarEleitores(int id) => _eleicaoRepository.BuscarEleitores(id);

        public IEnumerable<Inscricao> BuscarInscricoes(int eleicaoId, StatusInscricao? status = null) =>
             _eleicaoRepository.BuscarInscricoes(eleicaoId, status);


        public Inscricao BuscarInscricaoPeloUsuario(int eleicaoId, Usuario usuario) =>
            BuscarInscricoes(eleicaoId).FirstOrDefault(i => i.Eleitor.UsuarioId == usuario.Id);

        public bool ExcluirEleitor(Eleicao eleicao, int eleitorId)
        {
            var dimensionamentoAtual = CalcularDimensionamentoEleicao(eleicao);
            var novoDimensionamento = CalcularDimensionamentoEleicao(eleicao, dimensionamentoAtual.QtdaEleitores - 1, dimensionamentoAtual.QtdaVotos);
            var excluido = eleicao.ExcluirEleitor(eleitorId, novoDimensionamento);
            base.Atualizar(eleicao);
            return excluido;
        }

        public Eleicao BuscarPeloIdCarregarTodoAgregado(int id) => _eleicaoRepository.BuscarPeloIdCarregarTodoAgregado(id);

        public Eleitor AdicionarEleitor(Eleicao eleicao, Eleitor eleitor)
        {
            var dimensionamentoAtual = CalcularDimensionamentoEleicao(eleicao);
            var dimensionamentoProposto = CalcularDimensionamentoEleicao(eleicao, dimensionamentoAtual.QtdaEleitores + 1, dimensionamentoAtual.QtdaVotos);
            var usuario = _unitOfWork.UsuarioRepository.BuscarUsuario(eleitor.Email);

            if (usuario == null)
            {
                usuario = _unitOfWork.UsuarioRepository.Adicionar(new Usuario
                {
                    Cargo = eleitor.Cargo,
                    ContaId = null,
                    Email = eleitor.Email,
                    Nome = eleitor.Nome,
                    Perfil = Perfil.Eleitor,
                    CodigoRecuperacao = Guid.NewGuid()
                });
            }
            eleitor.UsuarioId = usuario.Id;
            eleitor.Usuario = usuario;
            eleicao.AdicionarEleitor(eleitor, dimensionamentoAtual, dimensionamentoProposto);
            base.Atualizar(eleicao);
            return eleitor;
        }

        public Inscricao FazerInscricao(Eleicao eleicao, Eleitor eleitor, string objetivos)
        {
            eleicao.FazerInscricao(eleitor, objetivos);
            base.Atualizar(eleicao);
            return eleicao.Inscricoes.First(i => i.EleitorId == eleitor.Id);
        }

        public Inscricao AtualizarInscricao(Eleicao eleicao, Eleitor eleitor, string objetivos)
        {
            var inscricao = eleicao.AtualizarInscricao(eleitor, objetivos);
            base.Atualizar(eleicao);
            return inscricao;
        }

        public Inscricao AprovarInscricao(Eleicao eleicao, int inscricaoId, Usuario usuarioAprovador)
        {
            var inscricaoAprovada = eleicao.AprovarInscricao(inscricaoId, usuarioAprovador);
            base.Atualizar(eleicao);
            return inscricaoAprovada;
        }

        public Inscricao ReprovarInscricao(Eleicao eleicao, int inscricaoId, Usuario usuarioAprovador, string motivoReprovacao)
        {
            eleicao.ReprovarInscricao(inscricaoId, usuarioAprovador, motivoReprovacao);
            base.Atualizar(eleicao);
            return eleicao.BuscarInscricaoPeloId(inscricaoId);
        }

        public Voto RegistrarVoto(Eleicao eleicao, int inscricaoId, int usuarioId, string ip)
        {
            var eleitor = eleicao.BuscarEleitorPeloIdUsuario(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");
            var voto = eleicao.RegistrarVoto(inscricaoId, eleitor, ip);
            base.Atualizar(eleicao);
            return voto;
        }
    }
}