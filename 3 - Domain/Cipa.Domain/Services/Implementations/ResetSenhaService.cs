using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class ResetSenhaService : ComunicadoAcessoBaseService
    {

        private string mensagemResetSenha =
            @"Foi solicitado reset de senha para seu usuário. Para cadastrar uma nova senha, clique no link abaixo:<br><br>
            <a href=""@LINK"">@LINK</a>
            <br><br><br>Sistema de Votação Online";
        public ResetSenhaService(Usuario usuario) : base(usuario)
        {
            MapeamentoParametros.Add("@LINK", () => LinkResetSenha);
        }

        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = new TemplateEmail(ETipoTemplateEmail.CadastroSESMT, "[CIPA] Acesso ao Sistema") {
                Template = mensagemResetSenha
            };
            return FormatarEmailPadrao(templateEmail, Usuario.Email);
        }
    }
}