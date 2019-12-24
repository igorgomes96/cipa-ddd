using Cipa.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Cipa.Application.Interfaces
{
    public interface IUsuarioAppService : IAppServiceBase<Usuario>
    {
        Usuario BuscarUsuario(string email, string senha);
        Usuario BuscarUsuarioPeloCodigoRecuperacao(Guid codigoRecuperacao);
        IEnumerable<Usuario> BuscarUsuariosPelaConta(int contaId);
        Usuario CadastrarNovaSenha(Guid codigoRecuperacao, string senha);
        void ResetarSenha(string email);
    }
}