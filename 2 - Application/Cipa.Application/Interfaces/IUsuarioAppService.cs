using Cipa.Domain.Entities;
using System.Collections.Generic;

namespace Cipa.Application.Interfaces
{
    public interface IUsuarioAppService : IAppServiceBase<Usuario>
    {
        Usuario BuscarUsuario(string email, string senha);
        IEnumerable<Usuario> BuscarUsuariosPelaConta(int contaId);
    }
}