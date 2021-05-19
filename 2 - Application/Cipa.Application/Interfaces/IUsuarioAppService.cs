using Cipa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cipa.Application.Interfaces
{
    public interface IUsuarioAppService : IAppServiceBase<Usuario>
    {
        Usuario BuscarUsuario(string email, string senha);
        Usuario BuscarUsuarioPeloCodigoRecuperacao(Guid codigoRecuperacao);
        IEnumerable<Usuario> BuscarUsuariosPelaConta(int contaId);
        Usuario CadastrarNovaSenha(Guid codigoRecuperacao, string senha);
        IEnumerable<Usuario> BuscarUsuariosAdministradores();
        Usuario AdicionarAdministrador(Usuario usuario);
        void ResetarSenha(string email);
        Task RedefinirSenha(string email, string novaSenha);
    }
}