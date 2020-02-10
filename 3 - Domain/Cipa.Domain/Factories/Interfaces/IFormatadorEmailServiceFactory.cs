using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Services.Interfaces;

namespace Cipa.Domain.Factories.Interfaces
{
    public interface IFormatadorEmailServiceFactory
    {
        IFormatadorEmailService ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(ETipoTemplateEmail tipoTemplate, Eleicao eleicao);
        IFormatadorEmailService ObterFormatadorEmailParaNotificacoesInscricoes(ETipoTemplateEmail tipoTemplate, Inscricao inscricao);
        IFormatadorEmailService ObterFormatadorEmailParaAcesso(ETipoTemplateEmail tipoTemplate, Usuario usuario);
    }
}