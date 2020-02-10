using System;

namespace Cipa.WebApi.ViewModels
{
    public class AuthInfoViewModel
    {
        public DateTime Criacao { get; set; }
        public DateTime Expiracao { get; set; }
        public string UsuarioEmail { get; set; }
        public string[] Roles { get; set; }
        public string AccessToken { get; set; }

    }
}