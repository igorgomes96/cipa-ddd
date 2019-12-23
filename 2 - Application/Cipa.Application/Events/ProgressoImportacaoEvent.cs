using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Cipa.Application.Events.EventsArgs;

namespace Cipa.Application.Events
{
    public class ProgressoImportacaoEvent : IProgressoImportacaoEvent
    {
        public event EventHandler<ProgressoImportacaoEventArgs> NotificacaoProgresso;
        public event EventHandler<FinalizacaoImportacaoStatusEventArgs> ImportacaoFinalizada;
        private ISubject<ProgressoImportacaoEventArgs, ProgressoImportacaoEventArgs> _notificationEventSubject;
        private IDisposable _notificationEventSubscription;

        public ProgressoImportacaoEvent()
        {
            _notificationEventSubject = Subject.Synchronize(new Subject<ProgressoImportacaoEventArgs>());

            _notificationEventSubscription =
            _notificationEventSubject.ObserveOn(NewThreadScheduler.Default)
                .Throttle(TimeSpan.FromMilliseconds(5))
                .Subscribe(progress =>
                {
                    EventHandler<ProgressoImportacaoEventArgs> handler = NotificacaoProgresso;
                    if (handler != null)
                    {
                        handler(this, progress);
                    }
                });
        }

        public void OnNotificacaoProgresso(object sender, ProgressoImportacaoEventArgs e)
        {
            var proporcaoEtapa = (decimal)1 / e.TotalEtapas;
            var progressoReal = (proporcaoEtapa * ((decimal)e.LinhasProcessadas / e.TotalLinhas)) + ((e.EtapaAtual - 1) * proporcaoEtapa);
            e.Progresso = progressoReal;
            _notificationEventSubject.OnNext(e);
        }

        public void OnImportacaoFinalizada(object sender, FinalizacaoImportacaoStatusEventArgs e)
        {
            EventHandler<FinalizacaoImportacaoStatusEventArgs> handler = ImportacaoFinalizada;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public void Dispose()
        {
            if (_notificationEventSubscription != null)
            {
                _notificationEventSubscription.Dispose();
                _notificationEventSubscription = null;
            }
        }
    }
}