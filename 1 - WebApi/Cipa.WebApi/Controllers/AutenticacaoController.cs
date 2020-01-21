using System;
using AutoMapper;
using Cipa.Domain.Helpers;
using Cipa.WebApi.Authentication;
using Cipa.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cipa.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IMapper _mapper;
        public AutenticacaoController(ILoginService loginService, IMapper mapper)
        {
            _loginService = loginService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<AuthInfoViewModel> Login(AcessoUsuarioViewModel usuario) =>
            _loginService.Login(usuario.Email, usuario.Senha);

        [HttpPost("alterarconta/{contaId}")]
        [Authorize(Roles = PerfilUsuario.Administrador)]
        public ActionResult<AuthInfoViewModel> AlterarConta(int contaId) =>
            _loginService.AlterarContaTokenAdministrador(UsuarioId, contaId);


        [HttpGet("codigorecuperacao/{codigo}")]
        [AllowAnonymous]
        public ActionResult<UsuarioViewModel> BuscarUsuarioPeloCodigoRecuperacao(string codigo)
        {
            Guid codigoGuid;
            try
            {
                codigoGuid = new Guid(codigo);
            }
            catch
            {
                return BadRequest("Código de recuperação inválido.");
            }

            return _mapper.Map<UsuarioViewModel>(_loginService.BuscarUsuarioPeloCodigoRecuperacao(codigoGuid));
        }

        [HttpPut("cadastrarsenha")]
        [AllowAnonymous]
        public ActionResult<AuthInfoViewModel> CadastraSenha(AcessoUsuarioViewModel usuario)
        {
            if (!usuario.CodigoRecuperacao.HasValue)
                return BadRequest("Código de recuperação inválido.");
            return _loginService.CadastrarNovaSenha(usuario.CodigoRecuperacao.Value, usuario.Senha);
        }

        [HttpPut("resetarsenha")]
        [AllowAnonymous]
        public IActionResult ResetarSenha(AcessoUsuarioViewModel usuario)
        {
            _loginService.ResetarSenha(usuario.Email);
            return NoContent();
        }
    }
}