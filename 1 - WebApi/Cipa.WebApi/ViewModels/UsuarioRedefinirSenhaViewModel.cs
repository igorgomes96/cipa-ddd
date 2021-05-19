using System;
using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels
{
    public class UsuarioRedefinirSenhaViewModel
    {
        [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
        [StringLength(100, ErrorMessage = "O e-mail pode conter no máximo {1} caracteres.")]
        [Required(ErrorMessage = "O e-mail do usuário deve ser informado.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A nova senha do usuário deve ser informada.")]
        public string Senha { get; set; }
        
    }
}
