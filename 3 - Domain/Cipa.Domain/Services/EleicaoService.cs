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
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;
        private readonly IUsuarioService _usuarioService;

        public EleicaoService(
            IEleicaoRepository eleicaoRepository,
            IEstabelecimentoRepository estabelecimentoRepository,
            IUsuarioService usuarioService
        ) : base(eleicaoRepository)
        {
            _eleicaoRepository = eleicaoRepository;
            _estabelecimentoRepository = estabelecimentoRepository;
            _usuarioService = usuarioService;
        }

        public override Eleicao Adicionar(Eleicao eleicao)
        {
            int qtdaEleicoes = _estabelecimentoRepository.QuantidadeEleicoesAno(eleicao.Estabelecimento, eleicao.Gestao);
            if (qtdaEleicoes > 0)
                throw new CustomException($"Já há uma eleição cadastrada para este estabelecimento no ano de {eleicao.Gestao}.");

            eleicao.GerarCronograma();
            return _repository.Adicionar(eleicao);
        }

        public override Eleicao Atualizar(Eleicao eleicao)
        {
            Eleicao eleicaoOld = _eleicaoRepository.BuscarPeloId(eleicao.Id);
            if (eleicaoOld == null) throw new NotFoundException("Código de eleição não encontrado.");
            bool jaPassouPeriodoInscricao = eleicaoOld.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Inscricao);
            if (jaPassouPeriodoInscricao && eleicaoOld.GrupoId != eleicao.GrupoId)
            {
                var dimensionamentoOld = CalcularDimensionamentoEleicao(eleicaoOld);
                var novoDimensionamento = eleicao.Grupo.CalcularDimensionamento(dimensionamentoOld.QtdaEleitores);
                if (novoDimensionamento.TotalCipeiros > dimensionamentoOld.QtdaInscricoesAprovadas)
                    throw new CustomException($"Para o grupo {eleicao.Grupo.CodigoGrupo}, o mínimo de inscrições necessária é {novoDimensionamento.TotalCipeiros}, e só houveram {dimensionamentoOld.QtdaInscricoesAprovadas} inscrições aprovadas nessa eleição.");

                eleicao.AtualizarDimensionamento(novoDimensionamento);
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
            var eleitor = _eleicaoRepository.BuscarEleitor(eleicao, eleitorId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");
            if (eleitor.Inscricao != null && eleitor.Inscricao.StatusInscricao != StatusInscricao.Reprovada)
                throw new CustomException("Não é possível excluir esse eleitor pois ele é um dos inscritos nessa eleição!");

            if (eleitor.Voto != null && eleitor.Voto.Id != 0)
                throw new CustomException("Não é possível excluir esse eleitor pois ele já votou nessa eleição!");

            var excluido = eleicao.Eleitores.Remove(eleitor);
            base.Atualizar(eleicao);
            return excluido;
        }

        public Eleicao BuscarPeloIdCarregarTodoAgregado(int id) => _eleicaoRepository.BuscarPeloIdCarregarTodoAgregado(id);

        public Eleitor AdicionarEleitor(Eleicao eleicao, Eleitor eleitor)
        {
            var dimensionamentoAtual = CalcularDimensionamentoEleicao(eleicao);
            var dimensionamentoProposto = CalcularDimensionamentoEleicao(eleicao, dimensionamentoAtual.QtdaEleitores + 1, dimensionamentoAtual.QtdaVotos);
            var usuario = _usuarioService.BuscarUsuario(eleitor.Email);

            // Encapsular em transação
            if (usuario == null)
            {
                usuario = _usuarioService.Adicionar(new Usuario
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
            eleicao.AdicionarEleitor(eleitor, dimensionamentoAtual, dimensionamentoAtual);
            base.Atualizar(eleicao);
            return eleitor;
        }
    }
}