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
        public IFormatadorEmailService ObterFormatadorEmailParaAcesso(ETipoTemplateEmail tipoTemplate, Usuario usuario)
        {
            switch (tipoTemplate) {
                case ETipoTemplateEmail.CadastroSESMT:
                    return new LinkCadastroService(usuario);
                case ETipoTemplateEmail.ResetSenha:
                    return new ResetSenhaService(usuario);
                default:
                    throw new CustomException("Formatador de e-mail não registrado!");
            }
        }

        public IFormatadorEmailService ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(ETipoTemplateEmail comunicado, Eleicao eleicao)
        {
            switch (comunicado) {
                case ETipoTemplateEmail.ConviteParaInscricao:
                    return new ConviteInscricaoFormatadorService(eleicao);
                case ETipoTemplateEmail.ConviteParaVotacao:
                    return new ConviteVotacaoFormatadorService(eleicao);
                case ETipoTemplateEmail.EditalConvocacao:
                    return new EditalConvocacaoFormatadorService(eleicao);
                case ETipoTemplateEmail.ErroMudancaEtapaCronograma:
                    return new ComunicadoErroMudancaEtapaService(eleicao);
                case ETipoTemplateEmail.SucessoMudancaEtapaCronograma:
                    return new ComunicadoSucessoMudancaEtapaService(eleicao);
                default:
                    throw new CustomException("Formatador de e-mail não registrado!");
            }
        }

        public IFormatadorEmailService ObterFormatadorEmailParaNotificacoesInscricoes(ETipoTemplateEmail tipoTemplate, Inscricao inscricao)
        {
            switch (tipoTemplate) {
                case ETipoTemplateEmail.InscricaoRealizada:
                    return new NotificacaoInscricaoRealizadaService(inscricao);
                case ETipoTemplateEmail.InscricaoAprovada:
                   return new NotificacaoInscricaoAprovadaService(inscricao);
                case ETipoTemplateEmail.InscricaoReprovada:
                   return new NotificacaoInscricaoReprovadaService(inscricao);
                case ETipoTemplateEmail.ReanaliseInscricao:
                   return new NotificacaoInscricaoEmReanaliseService(inscricao);
                default:
                    throw new CustomException("Formatador de e-mail não registrado!");
            }
        }
    }
}