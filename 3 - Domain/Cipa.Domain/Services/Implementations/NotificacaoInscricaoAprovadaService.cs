using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class NotificacaoInscricaoAprovadaService : ComunicadoNotificacaoInscricaoBaseService
    {
        public NotificacaoInscricaoAprovadaService(Inscricao inscricao) : base(inscricao)
        {
        }

        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = Inscricao.Eleicao.Conta.BuscarTemplateEmail(ETipoTemplateEmail.InscricaoAprovada);
            return FormatarEmailPadrao(templateEmail, Inscricao.Eleitor.Email);
        }
    }
}