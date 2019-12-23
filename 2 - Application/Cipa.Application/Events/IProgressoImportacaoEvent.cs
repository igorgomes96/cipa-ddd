using System;
using Cipa.Application.Events.EventsArgs;

namespace Cipa.Application.Events
{
    public interface IProgressoImportacaoEvent: IDisposable
    {
        event EventHandler<ProgressoImportacaoEventArgs> NotificacaoProgresso;
        event EventHandler<FinalizacaoImportacaoStatusEventArgs> ImportacaoFinalizada;
        void OnNotificacaoProgresso(object sender, ProgressoImportacaoEventArgs e);
        void OnImportacaoFinalizada(object sender, FinalizacaoImportacaoStatusEventArgs e);
    }
}