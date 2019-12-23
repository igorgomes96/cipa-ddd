using System.Collections.Generic;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;

namespace Cipa.Domain.Services.Implementations
{
    public class NotificacaoInscricaoRealizadaService : ComunicadoNotificacaoInscricaoBaseService
    {
        public NotificacaoInscricaoRealizadaService(Inscricao inscricao) : base(inscricao)
        {
        }

        public override ICollection<Email> FormatarEmails()
        {
            var templateEmail = Inscricao.Eleicao.Conta.BuscarTemplateEmail(ETipoTemplateEmail.InscricaoRealizada);
            return FormatarEmailPadrao(templateEmail, Inscricao.Eleitor.Email);
        }
    }
}