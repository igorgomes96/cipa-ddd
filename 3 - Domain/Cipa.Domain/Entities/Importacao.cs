using System;
using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public enum StatusImportacao
    {
        Aguardando,
        Processando,
        FinalizadoComSucesso,
        FinalizadoComFalha
    }

    public class Importacao : Entity<int>
    {
        public int ArquivoId { get; set; }
        public StatusImportacao Status { get; set; } = StatusImportacao.Aguardando;
        public int EleicaoId { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Arquivo Arquivo { get; set; }
        public virtual Eleicao Eleicao { get; set; }
        public virtual ICollection<Inconsistencia> Inconsistencias { get; set; }
    }
}
