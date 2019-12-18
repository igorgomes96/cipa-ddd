using System;
using System.Threading;
using Cipa.Application.Events.EventsArgs;

namespace Cipa.Application.Events
{
    public static class ProgressoImportacaoEvent
    {
        public static event EventHandler<ProgressoImportacaoEventArgs> NotificacaoProgresso;
        public static event EventHandler<FinalizacaoImportacaoStatusEventArgs> ImportacaoFinalizada;

        public static void OnNotificacaoProgresso(object sender, ProgressoImportacaoEventArgs e)
        {
            var proporcaoEtapa = (decimal)1 / e.TotalEtapas;
            var progressoReal = (proporcaoEtapa * ((decimal)e.LinhasProcessadas / e.TotalLinhas)) + ((e.EtapaAtual - 1) * proporcaoEtapa);
            e.Progresso = progressoReal;
            EventHandler<ProgressoImportacaoEventArgs> handler = ProgressoImportacaoEvent.NotificacaoProgresso;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public static void OnImportacaoFinalizada(object sender, FinalizacaoImportacaoStatusEventArgs e) {
            EventHandler<FinalizacaoImportacaoStatusEventArgs> handler = ProgressoImportacaoEvent.ImportacaoFinalizada;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}