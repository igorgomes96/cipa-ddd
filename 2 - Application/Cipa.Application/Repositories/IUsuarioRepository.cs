using Cipa.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Cipa.Application.Repositories
{
    public interface IUsuarioRepository : IRepositoryBase<Usuario>
    {
        Usuario BuscarUsuario(string email);
        Usuario BuscarUsuario(string email, string senha);
        IEnumerable<Usuario> BuscarUsuariosPelaConta(int contaId);
        Usuario BuscarUsuarioPeloCodigoRecuperacao(Guid codigo);

    }
}