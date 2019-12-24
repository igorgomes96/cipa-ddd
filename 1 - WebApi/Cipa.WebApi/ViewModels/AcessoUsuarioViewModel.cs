using System;
using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels
{
    public class AcessoUsuarioViewModel
    {
        public string Email { get; set; }
        public Guid? CodigoRecuperacao { get; set; }
        public string Senha { get; set; }
    }
}
