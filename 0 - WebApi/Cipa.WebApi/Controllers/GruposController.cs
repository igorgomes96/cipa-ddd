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

namespace Cipa.WebApi.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GruposController : Controller
    {
        private readonly IGrupoAppService _grupoAppService;
        private readonly IMapper _mapper;
        public GruposController(IGrupoAppService grupoAppService, IMapper mapper)
        {
            _grupoAppService = grupoAppService;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<GrupoViewModel> GetGrupos() =>
            _grupoAppService.BuscarTodos().AsQueryable().ProjectTo<GrupoViewModel>(_mapper.ConfigurationProvider);

        [HttpGet("{id}")]
        public GrupoDetalhesViewModel GetGrupo(int id) =>
            _mapper.Map<GrupoDetalhesViewModel>(_grupoAppService.BuscarPeloId(id));

    }
}