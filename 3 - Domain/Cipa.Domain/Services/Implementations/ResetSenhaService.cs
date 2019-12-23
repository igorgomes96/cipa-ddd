using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Services.Implementations
{
    public class ResetSenhaService : ComunicadoAcessoBaseService
    {
        public ResetSenhaService(Usuario usuario) : base(usuario)
        {
        }

        public override ICollection<Email> FormatarEmails()
        {
            throw new System.NotImplementedException();
        }
    }
}