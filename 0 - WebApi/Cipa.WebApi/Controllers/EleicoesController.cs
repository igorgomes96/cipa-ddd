using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.WebApi.Filters;
using Cipa.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cipa.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EleicoesController : Controller
    {
        private readonly IEleicaoAppService _eleicaoAppService;
        private readonly IMapper _mapper;
        public EleicoesController(IEleicaoAppService eleicaoAppService, IMapper mapper)
        {
            _eleicaoAppService = eleicaoAppService;
            _mapper = mapper;
        }

        [HttpGet]
        [Pagination]
        public IEnumerable<EleicaoViewModel> GetEleicoes()
        {
            if (User.IsInRole(PerfilUsuario.SESMT))
                return _eleicaoAppService.BuscarPelaConta(ContaId).AsQueryable().ProjectTo<EleicaoViewModel>(_mapper.ConfigurationProvider);
            else
                return _eleicaoAppService.BuscarPeloUsuario(UsuarioId).AsQueryable().ProjectTo<EleicaoDetalheViewModel>(_mapper.ConfigurationProvider);
        }

        [HttpGet("{id}")]
        public ActionResult<EleicaoViewModel> GetEleicao(int id)
        {
            var eleicao = _eleicaoAppService.BuscarPeloId(id);
            if (eleicao == null)
                return NotFound("Eleição não encontrada.");
            else
                return _mapper.Map<EleicaoViewModel>(eleicao);
        }

        [HttpPost]
        public ActionResult<EleicaoViewModel> PostEleicao(EleicaoViewModel eleicao)
        {
            eleicao.ContaId = ContaId;
            eleicao.UsuarioCriacaoId = UsuarioId;
            var eleicaoModel = _mapper.Map<Eleicao>(eleicao);
            return _mapper.Map<EleicaoViewModel>(_eleicaoAppService.Adicionar(eleicaoModel));
        }

        [HttpPut("{id}")]
        public IActionResult PutEleicao(EleicaoViewModel eleicao, int id)
        {
            eleicao.Id = id;
            _eleicaoAppService.Atualizar(_mapper.Map<Eleicao>(eleicao));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public EleicaoViewModel DeleteEleicao(int id)
        {
            return _mapper.Map<EleicaoViewModel>(_eleicaoAppService.Excluir(id));
        }

        [HttpGet("{id}/cronograma")]
        public IEnumerable<EtapaCronogramaViewModel> GetCronograma(int id) =>
            _eleicaoAppService.BuscarCronograma(id).AsQueryable().ProjectTo<EtapaCronogramaViewModel>(_mapper.ConfigurationProvider);

        [HttpGet("{id}/eleitores")]
        [Query("Nome", "Cargo")]
        [Pagination]
        public IQueryable<EleitorViewModel> GetEleitores(int id) =>
             _eleicaoAppService.BuscarEleitores(id).AsQueryable().ProjectTo<EleitorViewModel>(_mapper.ConfigurationProvider);


        [HttpDelete("{eleicaoId}/eleitores/{eleitorId}")]
        public Eleitor DeleteEleitor(int eleicaoId, int eleitorId)
        {
            return _eleicaoAppService.ExcluirEleitor(eleicaoId, eleitorId);
        }

        [HttpGet("{id}/eleitor")]
        public ActionResult<EleitorViewModel> GetEleitorLogado(int id)
        {
            return _mapper.Map<EleitorViewModel>(_eleicaoAppService.BuscarEleitorPeloIdUsuario(id, UsuarioId));
        }

        [Query("Eleitor.Nome")]
        [HttpGet("{id}/inscricoes")]
        public ActionResult<IEnumerable<InscricaoViewModel>> GetInscricoes(int id, string status, int? seedOrder = null)
        {
            StatusInscricao statusInscricao = StatusInscricao.Pendente;
            try
            {
                statusInscricao = (StatusInscricao)Enum.Parse(typeof(StatusInscricao), status);
            }
            catch
            {
                return BadRequest($"Status inválido: {status}");
            }
            var inscricoes = _eleicaoAppService.BuscarInscricoes(id, statusInscricao).AsQueryable();
            if (seedOrder.HasValue)
            {
                Random rnd = new Random(seedOrder.Value);
                return Ok(inscricoes.OrderBy(_ => rnd.Next()).ProjectTo<InscricaoViewModel>(_mapper.ConfigurationProvider));
            }
            return Ok(inscricoes.OrderBy(i => i.Eleitor.Nome).ProjectTo<InscricaoViewModel>(_mapper.ConfigurationProvider));
        }

        [HttpGet("{id}/inscricao")]
        public ActionResult<InscricaoDetalhesViewModel> GetInscricao(int id)
        {
            return _mapper.Map<InscricaoDetalhesViewModel>(_eleicaoAppService.BuscarInscricaoPeloUsuario(id, UsuarioId));
        }

        [HttpPost("{id}/proximaetapa")]
        public IEnumerable<EtapaCronogramaViewModel> PassarParaProximaEtapa(int id)
        {
            return _eleicaoAppService.PassarParaProximaEtapa(id)?.Cronograma.AsQueryable()
                .ProjectTo<EtapaCronogramaViewModel>(_mapper.ConfigurationProvider);
        }

        [HttpPost("{id}/eleitores")]
        public EleitorViewModel PostEleitor(int id, EleitorViewModel eleitorViewModel)
        {
            var eleitor = _mapper.Map<Eleitor>(eleitorViewModel);
            return _mapper.Map<EleitorViewModel>(_eleicaoAppService.AdicionarEleitor(id, eleitor));
        }

        [HttpPost("{id}/inscricoes")]
        public InscricaoViewModel PostInscricao(int id, InscricaoViewModel inscricao)
        {
            return _mapper.Map<InscricaoViewModel>(_eleicaoAppService.FazerInscricao(id, UsuarioId, inscricao.Objetivos));
        }

        [HttpPut("{id}/inscricoes")]
        public InscricaoViewModel PutInscricao(int id, InscricaoViewModel inscricao)
        {
            return _mapper.Map<InscricaoViewModel>(_eleicaoAppService.AtualizarInscricao(id, UsuarioId, inscricao.Objetivos));
        }

        [HttpPut("{id}/inscricoes/{inscricaoId}/aprovar")]
        public InscricaoViewModel PutAprovarInscricao(int id, int inscricaoId)
        {
            return _mapper.Map<InscricaoViewModel>(_eleicaoAppService.AprovarInscricao(id, inscricaoId, UsuarioId));
        }

        [HttpPut("{id}/inscricoes/{inscricaoId}/reprovar")]
        public InscricaoDetalhesViewModel PutReprovarInscricao(int id, int inscricaoId, ReprovacaoViewModel reprovacao)
        {
            return _mapper.Map<InscricaoDetalhesViewModel>(_eleicaoAppService.ReprovarInscricao(id, inscricaoId, UsuarioId, reprovacao.MotivoReprovacao));
        }

        [HttpPost("{id}/inscricoes/{inscricaoId}/votar")]
        public VotoViewModel PostVotar(int id, int inscricaoId)
        {
            return _mapper.Map<VotoViewModel>(_eleicaoAppService.RegistrarVoto(id, inscricaoId, UsuarioId, IpRequisicao));
        }

        [HttpPost("{id}/votarbranco")]
        public VotoViewModel PostVotarEmBranco(int id)
        {
            return _mapper.Map<VotoViewModel>(_eleicaoAppService.VotarEmBranco(id, UsuarioId, IpRequisicao));
        }

        [HttpGet("{id}/votos")]
        public IEnumerable<VotoViewModel> GetVotos(int id)
        {
            return _eleicaoAppService.BuscarVotos(id).AsQueryable().ProjectTo<VotoViewModel>(_mapper.ConfigurationProvider);
        }

        [HttpGet("{id}/votousuario")]
        public VotoViewModel GetVotoUsuario(int id)
        {
            return _mapper.Map<VotoViewModel>(_eleicaoAppService.BuscarVotoUsuario(id, UsuarioId));
        }

        [HttpGet("{id}/apuracao")]
        public IEnumerable<ApuracaoViewModel> GetApuracao(int id)
        {
            return _eleicaoAppService.ApurarVotos(id).AsQueryable().ProjectTo<ApuracaoViewModel>(_mapper.ConfigurationProvider);
        }


    }
}