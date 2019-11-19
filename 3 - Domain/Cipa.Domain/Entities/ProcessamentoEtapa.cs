using System;

namespace Cipa.Domain.Entities
{
    public class ProcessamentoEtapa: Entity<int>
    {
        public int? EtapaAnteriorId { get; set; } // Será null quando iniciar a eleição.
        public int? EtapaAtualId { get; set; }  // Será null quando terminar a eleição.
        public int EleicaoId { get; set; }
        public DateTime HorarioMudancaEtapa { get; set; }
        public bool Processado { get; set; }
        public DateTime? TerminoProcessamento { get; set; }
        public string ErroProcessamento { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual EtapaCronograma EtapaAnterior { get; set; }
        public virtual EtapaCronograma EtapaAtual { get; set; }
        public virtual Eleicao Eleicao { get; set; }
    }
}
