using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Factories.Interfaces;
using Cipa.Domain.Services.Implementations;
using Cipa.Domain.Services.Interfaces;

namespace Cipa.Domain.Factories
{
    public class FormatadorEmailServiceFactory : IFormatadorEmailServiceFactory
    {
        public IFormatadorEmailService ObterFormatadorEmail(EComunicado comunicado, Eleicao eleicao)
        {
            switch (comunicado) {
                case EComunicado.ConviteInscricao:
                    return new ConviteInscricaoFormatadorService(eleicao);
                case EComunicado.ConviteVotacao:
                    return new ConviteVotacaoFormatadorService(eleicao);
                case EComunicado.EditalConvocacao:
                    return new EditalConvocacaoFormatadorService(eleicao);
                default:
                    throw new CustomException("Formatador de e-mail não registrado!");
            }
        }
    }
}