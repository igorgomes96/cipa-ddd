namespace Cipa.Domain.Entities
{
    public class LinhaDimensionamento : DimensionamentoBase
    {
        public LinhaDimensionamento(int maximo, int minimo, int qtdaEfetivos, int qtdaSuplentes) :
            base(maximo, minimo, qtdaEfetivos, qtdaSuplentes)
        { }
        public int GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }
    }
}