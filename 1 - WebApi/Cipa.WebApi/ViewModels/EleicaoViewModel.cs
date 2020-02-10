using System;
using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels
{
    public class EleicaoViewModel
    {
        public int Id { get; set; }
        public int Gestao { get; set; }
        [Required(ErrorMessage = "A duração da gestão deve ser informada.")]
        public int? DuracaoGestao { get; set; }
        [Required(ErrorMessage = "A data de início do processo eleitoral deve ser informado.")]
        public DateTime? DataInicio { get; set; }
        [Required(ErrorMessage = "O código do estabelecimento deve ser informado.")]
        public int? EstabelecimentoId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        [Required(ErrorMessage = "O grupo ao qual pertence o estabelecimento deve ser informado (Consultar o Quadro II da NR-5).")]
        public int? GrupoId { get; set; }
        public string Grupo { get; set; }
        public DateTime? TerminoMandatoAnterior { get; set; }
        public int ContaId { get; set; }
        public int UsuarioCriacaoId { get; set; }
        public ConfiguracaoEleicaoViewModel Configuracao { get; set; }

        public EstabelecimentoViewModel Estabelecimento { get; set; }
        public EtapaCronogramaViewModel EtapaAtual { get; set; }
        public DimensionamentoViewModel Dimensionamento { get; set; }
    }

}