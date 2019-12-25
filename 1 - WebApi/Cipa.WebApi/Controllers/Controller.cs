using Cipa.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Cipa.WebApi.Controllers
{
    public class Controller : ControllerBase
    {
        protected string IpRequisicao
        {
            get
            {
                return Request.HttpContext.Connection.RemoteIpAddress.ToString();
            }
        }

        protected int ContaId
        {
            get
            {
                return int.Parse(User.Claims.First(c => c.Type == CustomClaimTypes.CodigoConta).Value);
            }
        }

        protected int UsuarioId
        {
            get
            {
                return int.Parse(User.Claims.First(c => c.Type == CustomClaimTypes.UsuarioId).Value);
            }
        }

        protected string NomeUsuario
        {
            get
            {
                return User.Claims.First(c => c.Type == CustomClaimTypes.NomeUsuario).Value;
            }
        }

        protected string EmailUsuario
        {
            get
            {
                return User.Identity.Name;
            }
        }

        protected bool UsuarioEhDoSESMT => User.IsInRole(PerfilUsuario.SESMT);
    }
}