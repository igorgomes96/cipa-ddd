using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class EtapaPadraoConta: EtapaBase
    {
        public int Id { get; set; }
        public int ContaId { get; set; }
        public CodigoEtapaObrigatoria? EtapaObrigatoriaId { get; set; }
        public int DuracaoPadrao { get; set; }

        public virtual Conta Conta { get; set; }
        public virtual EtapaObrigatoria EtapaObrigatoria { get; set; }

    }
}