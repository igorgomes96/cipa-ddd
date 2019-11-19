using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class EtapaObrigatoria: EtapaBase<CodigoEtapaObrigatoria>
    {
        public int? DuracaoMinima { get; set; }
        // Qtda mínima de dias antes do término do mandato anterior
        public int PrazoMandatoAnterior { get; set; }

    }
}