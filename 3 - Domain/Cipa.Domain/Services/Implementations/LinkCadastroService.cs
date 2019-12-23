using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Services.Implementations
{
    public class LinkCadastroService : ComunicadoAcessoBaseService
    {
        public LinkCadastroService(Usuario usuario) : base(usuario)
        {
        }

        public override ICollection<Email> FormatarEmails()
        {
            throw new System.NotImplementedException();
        }
    }
}