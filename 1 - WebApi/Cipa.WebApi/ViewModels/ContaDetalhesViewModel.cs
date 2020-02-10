using System.Collections.Generic;

namespace Cipa.WebApi.ViewModels
{
    public class ContaDetalhesViewModel: ContaViewModel
    {
        public virtual ICollection<UsuarioViewModel> Usuarios { get; set; }

    }
}
