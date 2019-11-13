using System.Collections.Generic;
using System.Linq;

namespace Cipa.Domain.Entities
{
    public class Grupo
    {
        public int Id { get; set; }
        public string CodigoGrupo { get; set; }

        public virtual LimiteDimensionamento LimiteDimensionamento { get; set; }
        public virtual ICollection<LinhaDimensionamento> Dimensionamentos { get; } = new List<LinhaDimensionamento>();

        public LinhaDimensionamento CalcularDimensionamento(int qtdaEleitores)
        {
            var dimensionamento = Dimensionamentos.Where(d => qtdaEleitores >= d.Minimo)
                    .OrderByDescending(d => d.Minimo).FirstOrDefault();
            if (qtdaEleitores > LimiteDimensionamento.Limite)
            {
                var limite = LimiteDimensionamento.Limite + 1;
                var intervalo = LimiteDimensionamento.IntervaloAcrescimo;
                var excedente = qtdaEleitores - limite;
                var indiceIntervalo = excedente / intervalo + 1;
                var maximo = limite + intervalo * indiceIntervalo - 1;
                var minimo = dimensionamento.Maximo - intervalo + 1;
                var qtdaEfetivos = dimensionamento.QtdaEfetivos + LimiteDimensionamento.AcrescimoEfetivos * indiceIntervalo;
                var qtdaSuplentes = dimensionamento.QtdaSuplentes + LimiteDimensionamento.AcrescimoSuplentes * indiceIntervalo;
                return new LinhaDimensionamento {
                    Grupo = this,
                    GrupoId = Id,
                    Maximo = maximo,
                    Minimo = minimo,
                    QtdaEfetivos = qtdaEfetivos,
                    QtdaSuplentes = qtdaSuplentes
                };
            }
            return dimensionamento;
        }
    }
}