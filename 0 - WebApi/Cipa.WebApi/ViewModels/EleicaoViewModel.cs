using System;

namespace Cipa.WebApi.ViewModels
{
    public class EleicaoViewModel
    {
        public int Id { get; set; }
        public int Gestao { get; set; }
        public int DuracaoGestao { get; set; }
        public DateTime DataInicio { get; set; }
        public int EstabelecimentoId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? HorarioFinalizacao { get; set; }
        public int GrupoId { get; set; }
        public string Grupo { get; set; }
        public int QtdaEfetivos { get; set; }
        public int QtdaSuplentes { get; set; }
        public DateTime? TerminoMandatoAnterior { get; set; }
        public bool UsuarioEleitor { get; set; }

        public EstabelecimentoViewModel Estabelecimento { get; set; }
        public EtapaCronogramaViewModel EtapaAtual { get; set; }
    }

}