using System;

namespace Cipa.Domain.Entities
{
    public class LinhaDimensionamento: DimensionamentoBase
    {
        public int GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }
    }
}