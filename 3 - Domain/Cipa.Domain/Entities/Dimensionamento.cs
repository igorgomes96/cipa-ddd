using System;

namespace Cipa.Domain.Entities {
    public class Dimensionamento: DimensionamentoBase {

        public Dimensionamento() { }
        public Dimensionamento(
            DimensionamentoBase dimensionamento) {
            Maximo = dimensionamento.Maximo;
            Minimo = dimensionamento.Minimo;
            QtdaEfetivos = dimensionamento.QtdaEfetivos;
            QtdaSuplentes = dimensionamento.QtdaSuplentes;
        }

        public int QtdaEleitores { get; set; }
        public int QtdaVotos { get; set; }
        public int QtdaInscricoesAprovadas { get; set; }
        public int QtdaInscricoesReprovadas { get; set; }
        public int QtdaInscricoesPendentes { get; set; }

        public int QtdaInscricoes {
            get {
                return QtdaInscricoesAprovadas + QtdaInscricoesPendentes + QtdaInscricoesReprovadas;
            }
        }

        public bool PossuiQtdaMinimaCandidatos
        {
            get
            {
                return (QtdaInscricoesAprovadas + QtdaInscricoesPendentes) >= TotalCipeiros;
            }
        }

        
        public int QtdaMinimaVotos {
            get {
                return (int)Math.Ceiling((decimal)QtdaEleitores / 2);
            }
        }

        public bool PossuiQtdaMinimaVotos {
            get {
                return QtdaVotos >= QtdaMinimaVotos;
            }
        }
    }
}