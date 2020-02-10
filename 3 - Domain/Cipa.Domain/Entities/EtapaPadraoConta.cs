using Cipa.Domain.Enums;
using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public class EtapaPadraoConta : EtapaBase
    {
        public EtapaPadraoConta(string nome, string descricao, int ordem, int contaId, int duracaoPadrao, ECodigoEtapaObrigatoria? etapaObrigatoriaId = null)
            : base(nome, descricao, ordem)
        {
            EtapaObrigatoriaId = etapaObrigatoriaId;
            ContaId = contaId;
            DuracaoPadrao = duracaoPadrao;
        }
        public int ContaId { get; private set; }
        public ECodigoEtapaObrigatoria? EtapaObrigatoriaId { get; set; }
        public int DuracaoPadrao { get; set; }

        public virtual Conta Conta { get; set; }
        public virtual EtapaObrigatoria EtapaObrigatoria { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ContaId;
            yield return Ordem;
        }

    }
}