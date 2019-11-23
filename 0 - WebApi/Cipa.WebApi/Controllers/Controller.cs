using System.Linq;
using Cipa.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;

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
    }
}