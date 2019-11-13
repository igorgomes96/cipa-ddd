using Cipa.Domain.Entities;
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
        public AutenticacaoController(ILoginService loginService) {
            _loginService = loginService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<AuthInfoViewModel> Login(Usuario usuario) => 
            _loginService.Login(usuario.Email, usuario.Senha);
    }
}