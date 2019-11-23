using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class EtapaObrigatoria: Entity<CodigoEtapaObrigatoria>
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Ordem { get; set; }
        public int? DuracaoMinima { get; set; }
        // Qtda mínima de dias antes do término do mandato anterior
        public int PrazoMandatoAnterior { get; set; }

    }
}