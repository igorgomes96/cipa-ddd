using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Services.Interfaces;

namespace Cipa.Domain.Factories.Interfaces
{
    public interface IFormatadorEmailServiceFactory
    {
        IFormatadorEmailService ObterFormatadorEmail(EComunicado comunicado, Eleicao eleicao);
    }
}