using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels
{

    public class EtapaBaseViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome da etapa deve ser informado.")]
        [StringLength(100, ErrorMessage = "O nome da etapa deve possuir no máximo {1} caracteres.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "A descrição da etapa deve ser informada.")]
        [StringLength(4000, ErrorMessage = "A descrição da etapa deve possuir no máximo {1} caracteres.")]
        public string Descricao { get; set; }
        [Required(ErrorMessage = "A posição/ordem da etapa no cronograma deve ser informada.")]
        public int Ordem { get; set; }

    }
}
