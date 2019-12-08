using System.Collections.Generic;

namespace Cipa.WebApi.ViewModels
{
    public class GrupoDetalhesViewModel
    {
        public int Id { get; set; }
        public string CodigoGrupo { get; set; }

        public LimiteDimensionamentoViewModel LimiteDimensionamento { get; set; }
        public IEnumerable<LinhaDimensionamentoViewModel> Dimensionamentos { get; set; }
    }
}