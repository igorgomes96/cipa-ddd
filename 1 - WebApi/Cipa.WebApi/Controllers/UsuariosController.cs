using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.WebApi.Authentication;
using Cipa.WebApi.Filters;
using Cipa.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Cipa.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IMapper _mapper;
        public UsuariosController(IUsuarioAppService usuarioAppService, IMapper mapper)
        {
            _usuarioAppService = usuarioAppService;
            _mapper = mapper;
        }

        [HttpGet]
        [Pagination]
        [Authorize(PoliticasAutorizacao.UsuarioSESMT)]
        public IEnumerable<UsuarioViewModel> GetUsuarios() =>
            _usuarioAppService.BuscarUsuariosPelaConta(ContaId).AsQueryable().ProjectTo<UsuarioViewModel>(_mapper.ConfigurationProvider);

        [HttpGet("logado")]
        public ActionResult<UsuarioViewModel> GetUsuarioLogado() =>
            _mapper.Map<UsuarioViewModel>(_usuarioAppService.BuscarPeloId(UsuarioId));

        [HttpGet("{id}")]
        public ActionResult<UsuarioViewModel> GetUsuario(int id)
        {
            var usuario = _usuarioAppService.BuscarPeloId(id);
            if (usuario.ContaId.HasValue && usuario.ContaId.Value != ContaId)
                return Forbid();
            return _mapper.Map<UsuarioViewModel>(_usuarioAppService.BuscarPeloId(id));
        }

        [HttpPost]
        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        public UsuarioViewModel PostUsuario(UsuarioViewModel usuario)
        {
            var novoUsuario = _mapper.Map<Usuario>(usuario);
            novoUsuario.ContaId = ContaId;
            novoUsuario.Perfil = PerfilUsuario.SESMT;  // A criação manual de usuário somente acontece para usuários do SESMT
            return _mapper.Map<UsuarioViewModel>(_usuarioAppService.Adicionar(novoUsuario));
        }

        [HttpPut("{id}")]
        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        public IActionResult PutUsuario(int id, UsuarioViewModel usuarioViewModel)
        {
            var usuario = _mapper.Map<Usuario>(usuarioViewModel);
            usuario.Id = id;
            usuario.ContaId = ContaId;
            usuario.Perfil = PerfilUsuario.SESMT;
            _usuarioAppService.Atualizar(usuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        public ActionResult<UsuarioViewModel> DeleteUsuario(int id)
        {
            if (id == UsuarioId) return BadRequest("Não é permitida a auto-exclusão.");
            return _mapper.Map<UsuarioViewModel>(_usuarioAppService.Excluir(id));
        }

    }
}