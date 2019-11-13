namespace Cipa.WebApi.ViewModels {
    public class EmpresaViewModel {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string Cnpj { get; set; }
        public string CnpjFormatado { get; set; }
        public string InformacoesGerais { get; set; }
    }
}