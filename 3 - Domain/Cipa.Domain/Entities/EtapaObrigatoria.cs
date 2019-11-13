

using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class EtapaObrigatoria: EtapaBase
    {
        public CodigoEtapaObrigatoria Id { get; set; }
        public int? DuracaoMinima { get; set; }
        // Qtda mínima de dias antes do término do mandato anterior
        public int PrazoMandatoAnterior { get; set; }

    }
}