using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces {
    public interface IUsuarioAppService: IAppServiceBase<Usuario>
    {
        Usuario BuscarUsuario(string email, string senha);
        IEnumerable<Usuario> BuscarUsuariosPelaConta(int contaId);
    }
}