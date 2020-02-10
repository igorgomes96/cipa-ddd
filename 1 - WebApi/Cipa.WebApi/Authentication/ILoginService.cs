using System;
using Cipa.Domain.Entities;
using Cipa.WebApi.ViewModels;

namespace Cipa.WebApi.Authentication
{
    public interface ILoginService
    {
        AuthInfoViewModel Login(string email, string senha);
        AuthInfoViewModel CadastrarNovaSenha(Guid codigoRecuperacao, string senha);
        void ResetarSenha(string email);
        Usuario BuscarUsuarioPeloCodigoRecuperacao(Guid codigoRecuperacao);
        AuthInfoViewModel AlterarContaTokenAdministrador(int usuarioId, int contaId);
    }
}