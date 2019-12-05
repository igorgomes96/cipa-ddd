using System.Collections.Generic;
using System.Linq;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application
{
    public class EleicaoAppService : AppServiceBase<Eleicao>, IEleicaoAppService
    {

        public EleicaoAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.EleicaoRepository){ }

        public override Eleicao Adicionar(Eleicao eleicao)
        {
            var estabelecimento = _unitOfWork.EstabelecimentoRepository.BuscarPeloId(eleicao.EstabelecimentoId); // .BuscarPeloIdCarregarEleicoes(eleicao.EstabelecimentoId);
            if (estabelecimento == null)
                throw new NotFoundException($"Estabelecimento {eleicao.EstabelecimentoId} não encontrado.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarPeloId(eleicao.UsuarioCriacaoId); // BuscarPeloIdCarregarAgregadoConta(eleicao.UsuarioCriacaoId);
            if (usuario == null)
                throw new NotFoundException($"Usuário {eleicao.UsuarioCriacaoId} não encontrado.");

            var grupo = _unitOfWork.GrupoRepository.BuscarPeloId(eleicao.GrupoId);
            if (grupo == null)
                throw new NotFoundException($"Grupo {eleicao.GrupoId} não encontrado.");

            var novaEleicao = new Eleicao(
                eleicao.DataInicio,
                eleicao.DuracaoGestao,
                eleicao.TerminoMandatoAnterior,
                usuario, estabelecimento, grupo);
            novaEleicao.GerarCronograma();

            return base.Adicionar(novaEleicao);
        }

        public override void Atualizar(Eleicao eleicao)
        {
            Eleicao eleicaoExistente = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicao.Id);
            if (eleicaoExistente == null) throw new NotFoundException("Código de eleição não encontrado.");

            eleicaoExistente.DataInicio = eleicao.DataInicio;
            eleicaoExistente.TerminoMandatoAnterior = eleicao.TerminoMandatoAnterior;
            eleicaoExistente.DuracaoGestao = eleicao.DuracaoGestao;

            var grupo = _unitOfWork.GrupoRepository.BuscarPeloId(eleicao.GrupoId);
            if (grupo == null)
                throw new NotFoundException($"Grupo {eleicao.GrupoId} não encontrado.");

            eleicaoExistente.Grupo = grupo;

            base.Atualizar(eleicaoExistente);
        }

        public override Eleicao Excluir(int id)
        {
            Eleicao eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(id);
            if (eleicao == null) throw new NotFoundException("Código de eleição não encontrado.");

            if (eleicao.EtapaAtual != null || eleicao.DataFinalizacao.HasValue)
            {
                throw new CustomException("Um eleição só pode ser excluída antes do início do processo.");
            }
            _unitOfWork.EleicaoRepository.Excluir(eleicao);
            _unitOfWork.Commit();
            return eleicao;
        }

        public IEnumerable<EtapaCronograma> BuscarCronograma(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");
            return eleicao.Cronograma.OrderBy(e => e.Ordem);
        }

        public IEnumerable<Eleitor> BuscarEleitores(int eleicaoId) =>
            _unitOfWork.EleicaoRepository.BuscarEleitores(eleicaoId).OrderBy(e => e.Nome);

        public Eleitor BuscarEleitorPeloIdUsuario(int eleicaoId, int usuarioId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            return eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
        }

        public IEnumerable<Inscricao> BuscarInscricoes(int eleicaoId, StatusInscricao? status = null) =>
            _unitOfWork.EleicaoRepository.BuscarInscricoes(eleicaoId, status);

        public Inscricao BuscarInscricaoPeloUsuario(int eleicaoId, int usuarioId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            return eleicao.BuscarInscricaoPeloEleitorId(eleitor.Id);
        }

        public IEnumerable<Eleicao> BuscarPelaConta(int contaId)
        {
            return _unitOfWork.EleicaoRepository.BuscarPelaConta(contaId);
        }

        public IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId)
        {
            return _unitOfWork.EleicaoRepository.BuscarPeloUsuario(usuarioId);
        }

        public Eleitor ExcluirEleitor(int eleicaoId, int eleitorId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitor(eleitorId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var eleitorExcluido = eleicao.ExcluirEleitor(eleitor);
            if (!eleitorExcluido)
                throw new CustomException("Erro ao excluir eleitor.");

            base.Atualizar(eleicao);
            return eleitor;
        }

        public Eleicao PassarParaProximaEtapa(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            eleicao.PassarParaProximaEtapa();
            base.Atualizar(eleicao);
            return eleicao;
        }

        public Eleitor AdicionarEleitor(int eleicaoId, Eleitor eleitor)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarUsuario(eleitor.Email);
            if (usuario == null)
                usuario = new Usuario(eleitor.Email, eleitor.Nome, eleitor.Cargo);
            eleitor.Usuario = usuario;

            var eleitorAdicionado = eleicao.AdicionarEleitor(eleitor);
            base.Atualizar(eleicao);
            return eleitorAdicionado;
        }

        public Inscricao FazerInscricao(int eleicaoId, int usuarioId, string objetivos)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var inscricao = eleicao.FazerInscricao(eleitor, objetivos);
            base.Atualizar(eleicao);
            return inscricao;
        }

        public Inscricao AtualizarInscricao(int eleicaoId, int usuarioId, string objetivos)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var inscricao = eleicao.AtualizarInscricao(eleitor.Id, objetivos);
            base.Atualizar(eleicao);
            return inscricao;
        }

        public Inscricao AprovarInscricao(int eleicaoId, int inscricaoId, int usuarioAprovadorId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarPeloId(usuarioAprovadorId);
            if (usuario == null) throw new CustomException("Usuário inválido!");

            var inscricaoAprovada = eleicao.AprovarInscricao(inscricaoId, usuario);
            base.Atualizar(eleicao);
            return inscricaoAprovada;
        }

        public Inscricao ReprovarInscricao(int eleicaoId, int inscricaoId, int usuarioAprovadorId, string motivoReprovacao)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarPeloId(usuarioAprovadorId);
            if (usuario == null) throw new CustomException("Usuário inválido!");

            var inscricaoReprovacao = eleicao.ReprovarInscricao(inscricaoId, usuario, motivoReprovacao);
            base.Atualizar(eleicao);
            return inscricaoReprovacao;
        }

        public Voto RegistrarVoto(int eleicaoId, int inscricaoId, int usuarioId, string ip)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var voto = eleicao.RegistrarVoto(inscricaoId, eleitor, ip);
            base.Atualizar(eleicao);
            return voto;
        }

        public Voto VotarEmBranco(int eleicaoId, int usuarioId, string ip)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var voto = eleicao.VotarEmBranco(eleitor, ip);
            base.Atualizar(eleicao);
            return voto;
        }

        public IEnumerable<Voto> BuscarVotos(int eleicaoId) => _unitOfWork.EleicaoRepository.BuscarVotos(eleicaoId);

        public Voto BuscarVotoUsuario(int eleicaoId, int usuarioId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            return eleicao.BuscarVotoEleitor(eleitor);
        }

        public IEnumerable<Inscricao> ApurarVotos(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");
            return eleicao.ApurarVotos();
        }

        public Eleitor AtualizarEleitor(int eleicaoId, Eleitor eleitor)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarUsuario(eleitor.Email);
            if (usuario == null)
                usuario = new Usuario(eleitor.Email, eleitor.Nome, eleitor.Cargo);
            eleitor.Usuario = usuario;

            var eleitorAtualizado = eleicao.AtualizarEleitor(eleitor);
            base.Atualizar(eleicao);
            return eleitorAtualizado;
        }

        public Eleitor BuscarEleitor(int eleicaoId, int eleitorId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitor(eleitorId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");
            return eleitor;
        }

        public IEnumerable<EtapaCronograma> AtualizarCronograma(int eleicaoId, EtapaCronograma etapa)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            eleicao.AtualizarCronograma(etapa);
            base.Atualizar(eleicao);
            return eleicao.Cronograma;
        }
    }
}