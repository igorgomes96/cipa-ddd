using System;
using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Application.Events.EventsArgs
{
    public class FinalizacaoImportacaoStatusEventArgs : EventArgs
    {
        public StatusImportacao Status { get; set; }
        public int QtdaErros { get; set; }
        public string EmailUsuario { get; set; }
    }
}