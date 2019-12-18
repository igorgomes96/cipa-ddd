using System;

namespace Cipa.Application.Events.EventsArgs
{
    public class ProgressoImportacaoEventArgs : EventArgs
    {
        public int TotalLinhas { get; set; }
        public int LinhasProcessadas { get; set; }
        public int TotalEtapas { get; set; }
        public int EtapaAtual { get; set; }
        public decimal Progresso { get; set; }
        public string EmailUsuario { get; set; }
    }
}