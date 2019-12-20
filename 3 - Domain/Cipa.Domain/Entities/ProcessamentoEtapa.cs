using System;
using System.Collections.Generic;
using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Factories.Interfaces;
using Cipa.Domain.Helpers;
using Cipa.Domain.Services.Interfaces;

namespace Cipa.Domain.Entities
{
    public class ProcessamentoEtapa : Entity<int>
    {
        public ProcessamentoEtapa()
        {  // Entity Framework
            StatusProcessamentoEtapa = EStatusProcessamentoEtapa.Pendente;
            HorarioMudancaEtapa = DateTime.Now;
        }

        public ProcessamentoEtapa(Eleicao eleicao, EtapaCronograma etapaCronograma, EtapaCronograma etapaCronogramaAnterior)
        {
            EtapaCronograma = etapaCronograma;
            EtapaCronogramaAnterior = etapaCronogramaAnterior;
            Eleicao = eleicao;
            StatusProcessamentoEtapa = EStatusProcessamentoEtapa.Pendente;
            HorarioMudancaEtapa = DateTime.Now;
        }

        public DateTime HorarioMudancaEtapa { get; private set; }
        public EStatusProcessamentoEtapa StatusProcessamentoEtapa { get; private set; }
        public DateTime? TerminoProcessamento { get; private set; }
        public string MensagemErro { get; private set; }
        public DateTime DataCadastro { get; private set; }

        public virtual EtapaCronograma EtapaCronograma { get; private set; }
        public virtual EtapaCronograma EtapaCronogramaAnterior { get; private set; }
        public virtual Eleicao Eleicao { get; private set; }

        public void IniciarProcessamento()
        {
            StatusProcessamentoEtapa = EStatusProcessamentoEtapa.Processando;
            MensagemErro = null;
        }

        private void FinalizarProcessamento(EStatusProcessamentoEtapa statusProcessamentoEtapa, string mensagemErro = null)
        {
            TerminoProcessamento = DateTime.Now;
            StatusProcessamentoEtapa = statusProcessamentoEtapa;
            MensagemErro = mensagemErro;
        }

        public ICollection<Email> RealizarProcessamentoGerarEmails(EmailConfiguration emailConfiguration, IFormatadorEmailServiceFactory formatadorFactory)
        {
            if (StatusProcessamentoEtapa != EStatusProcessamentoEtapa.Processando)
                throw new CustomException("O processamento de uma etapa só pode ocorrer quando o status for igual a 'Processando'.");

            try
            {
                IFormatadorEmailService formatador = ObterFomatadorEmail(EtapaCronograma.EtapaObrigatoriaId, formatadorFactory);
                ICollection<Email> emails = new List<Email>();
                if (formatador != null)
                    emails = formatador.FormatarEmails();

                FinalizarProcessamento(EStatusProcessamentoEtapa.ProcessadoComSucesso);
                return emails;
            }
            catch (Exception e)
            {
                var mensagem = $"Erro ao processar mudança de etapa/envio de e-mail. Contate o suporte.\n{e.Message}\n{e.StackTrace}";
                FinalizarProcessamento(EStatusProcessamentoEtapa.ErroProcessamento, mensagem);
                return new List<Email>();
            }
        }

        private IFormatadorEmailService ObterFomatadorEmail(
            CodigoEtapaObrigatoria? codigoEtapaObrigatoria,
            IFormatadorEmailServiceFactory formatadorFactory)
        {
            switch (EtapaCronograma.EtapaObrigatoriaId)
            {
                case (CodigoEtapaObrigatoria.Convocacao):
                    return formatadorFactory.ObterFormatadorEmail(EComunicado.EditalConvocacao, Eleicao);
                case (CodigoEtapaObrigatoria.Inscricao):
                    return formatadorFactory.ObterFormatadorEmail(EComunicado.ConviteInscricao, Eleicao);
                case (CodigoEtapaObrigatoria.Votacao):
                    return formatadorFactory.ObterFormatadorEmail(EComunicado.ConviteVotacao, Eleicao);
                default:
                    return null;
            }
        }
    }
}
