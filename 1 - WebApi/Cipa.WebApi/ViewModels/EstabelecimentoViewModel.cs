using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels
{
    public class EstabelecimentoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A cidade do estabelecimento deve ser informada.")]
        [StringLength(100, ErrorMessage = "A cidade pode conter no máximo {1} caracteres.")]
        public string Cidade { get; set; }
        [Required(ErrorMessage = "O endereço do estabelecimento deve ser informado.")]
        [StringLength(255, ErrorMessage = "O endereço pode conter no máximo {1} caracteres.")]
        public string Endereco { get; set; }
        [StringLength(255, ErrorMessage = "A descrição pode conter no máximo {1} caracteres.")]
        public string Descricao { get; set; }
        public int EmpresaId { get; set; }
        public int GrupoId { get; set; }
        public string Grupo { get; set; }

        public EmpresaViewModel Empresa { get; set; }
    }
}