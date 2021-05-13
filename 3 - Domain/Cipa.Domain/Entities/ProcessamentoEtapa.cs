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
            HorarioMudancaEtapa = DateTime.Now.HorarioBrasilia();
        }

        public ProcessamentoEtapa(Eleicao eleicao, EtapaCronograma etapaCronograma, EtapaCronograma etapaCronogramaAnterior)
        {
            EtapaCronograma = etapaCronograma;
            EtapaCronogramaId = etapaCronograma.Id;
            EtapaCronogramaAnterior = etapaCronogramaAnterior;
            EtapaCronogramaAnteriorId = etapaCronogramaAnterior?.Id;
            Eleicao = eleicao;
            StatusProcessamentoEtapa = EStatusProcessamentoEtapa.Pendente;
            HorarioMudancaEtapa = DateTime.Now.HorarioBrasilia();
        }

        public DateTime HorarioMudancaEtapa { get; private set; }
        public EStatusProcessamentoEtapa StatusProcessamentoEtapa { get; private set; }
        public int EtapaCronogramaId { get; private set; }
        public int? EtapaCronogramaAnteriorId { get; private set; }
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
            TerminoProcessamento = DateTime.Now.HorarioBrasilia();
            StatusProcessamentoEtapa = statusProcessamentoEtapa;
            MensagemErro = mensagemErro;
        }

        public ICollection<Email> RealizarProcessamentoGerarEmails(IFormatadorEmailServiceFactory formatadorFactory)
        {
            if (StatusProcessamentoEtapa != EStatusProcessamentoEtapa.Processando)
                throw new CustomException("O processamento de uma etapa só pode ocorrer quando o status for igual a 'Processando'.");

            try
            {
                IFormatadorEmailService formatador = ObterFomatadorEmail(formatadorFactory);
                ICollection<Email> emails = new List<Email>();
                if (formatador != null)
                    emails = formatador.FormatarEmails();

                FinalizarProcessamento(EStatusProcessamentoEtapa.ProcessadoComSucesso);
                return emails;
            }
            catch (Exception e)
            {
                var mensagem = $"Erro ao processar mudança de etapa/envio de e-mail. Contate o suporte.\r\n{e.Message}";
                FinalizarProcessamento(EStatusProcessamentoEtapa.ErroProcessamento, mensagem);
                return new List<Email>();
            }
        }

        private IFormatadorEmailService ObterFomatadorEmail(IFormatadorEmailServiceFactory formatadorFactory)
        {
            switch (EtapaCronograma.EtapaObrigatoriaId)
            {
                case (ECodigoEtapaObrigatoria.Convocacao):
                    if (Eleicao.Configuracao.EnvioEditalConvocao)
                        return formatadorFactory.ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(ETipoTemplateEmail.EditalConvocacao, Eleicao);
                    break;
                case (ECodigoEtapaObrigatoria.Inscricao):
                    if (Eleicao.Configuracao.EnvioConviteInscricao)
                        return formatadorFactory.ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(ETipoTemplateEmail.ConviteParaInscricao, Eleicao);
                    break;
                case (ECodigoEtapaObrigatoria.Votacao):
                    if (Eleicao.Configuracao.EnvioConviteVotacao)
                        return formatadorFactory.ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(ETipoTemplateEmail.ConviteParaVotacao, Eleicao);
                    break;
                default:
                    return null;
            }
            return null;
        }
    }
}
