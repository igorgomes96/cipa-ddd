using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
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
        public IEnumerable<EleicaoViewModel> GetEleicoes()
        {
            if (User.IsInRole(Perfil.SESMT))
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
        public EleicaoViewModel PostEleicao(EleicaoViewModel eleicao)
        {
            var eleicaoModel = _mapper.Map<Eleicao>(eleicao);
            eleicaoModel.ContaId = ContaId;
            eleicaoModel.UsuarioCriacaoId = UsuarioId;
            return _mapper.Map<EleicaoViewModel>(_eleicaoAppService.Adicionar(eleicaoModel));
        }

        [HttpPut("{id}")]
        public IActionResult PutEleicao(EleicaoViewModel eleicao, int id) {
            eleicao.Id = id;
            _eleicaoAppService.Atualizar(_mapper.Map<Eleicao>(eleicao));
            return NoContent();
        }
    }
}