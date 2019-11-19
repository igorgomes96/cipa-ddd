using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels {
    public class EmpresaViewModel {
        public int Id { get; set; }
        [Required(ErrorMessage = "A razão social da empresa deve ser informada.")]
        public string RazaoSocial { get; set; }
        [StringLength(14, ErrorMessage = "O CNPJ deve possuir {1} caracteres.", MinimumLength = 14)]
        public string Cnpj { get; set; }
        public string CnpjFormatado { get; set; }
        [StringLength(255, ErrorMessage = "As informações gerais podem conter no máximo {1} caracteres.")]
        public string InformacoesGerais { get; set; }
    }
}