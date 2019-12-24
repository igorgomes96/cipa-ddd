using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class LinkCadastroService : ComunicadoAcessoBaseService
    {
        private string mensagemCadastroSESMT =
            @"Foi criada uma conta para seu acesso ao sistema da CIPA. Para cadastrar sua senha, clique no link abaixo:<br><br>
            <a href=""@LINK"">@LINK</a>
            <br><br><br>Sistema de Votação Online";

        public LinkCadastroService(Usuario usuario) : base(usuario)
        {
            MapeamentoParametros.Add("@LINK", () => LinkCadastro);
        }

        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = new TemplateEmail(ETipoTemplateEmail.CadastroSESMT, "[CIPA] Acesso ao Sistema") {
                Template = mensagemCadastroSESMT
            };
            return FormatarEmailPadrao(templateEmail, Usuario.Email);
        }
    }
}