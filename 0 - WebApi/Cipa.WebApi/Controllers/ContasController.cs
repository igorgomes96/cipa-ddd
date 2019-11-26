using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.WebApi.Filters;
using Cipa.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cipa.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContasController : Controller
    {
        private readonly IContaAppService _contaAppService;
        private readonly IMapper _mapper;
        public ContasController(IContaAppService contaAppService, IMapper mapper)
        {
            _contaAppService = contaAppService;
            _mapper = mapper;
        }

        [HttpGet("cronograma")]
        public IEnumerable<EtapaPadraoContaViewModel> GetCronogramaPadrao() =>
            _contaAppService.BuscarCronogramaPadrao(ContaId).AsQueryable().ProjectTo<EtapaPadraoContaViewModel>(_mapper.ConfigurationProvider);

        [HttpPost("cronograma")]
        public EtapaPadraoContaViewModel PostEtapaPadrao(EtapaPadraoContaViewModel etapaPadrao) =>
            _mapper.Map<EtapaPadraoContaViewModel>(_contaAppService.AdicionarEtapaPadrao(ContaId, _mapper.Map<EtapaPadraoConta>(etapaPadrao)));

        [HttpPut("cronograma/{etapaPadraoId}")]
        public EtapaPadraoContaViewModel PutEtapaPadrao(int etapaPadraoId, EtapaPadraoContaViewModel etapaPadrao)
        {
            etapaPadrao.Id = etapaPadraoId;
            return _mapper.Map<EtapaPadraoContaViewModel>(_contaAppService.AtualizarEtapaPadrao(ContaId, _mapper.Map<EtapaPadraoConta>(etapaPadrao)));
        }

        [HttpDelete("cronograma/{etapaPadraoId}")]
        public EtapaPadraoContaViewModel DeleteEtapaPadrao(int etapaPadraoId) =>
            _mapper.Map<EtapaPadraoContaViewModel>(_contaAppService.RemoverEtapaPadrao(ContaId, etapaPadraoId));
    }
}