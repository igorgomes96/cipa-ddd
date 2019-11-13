namespace Cipa.WebApi.ViewModels
{
    public class EstabelecimentoViewModel
    {
        public int Id { get; set; }
        public string Cidade { get; set; }
        public string Endereco { get; set; }
        public string Descricao { get; set; }
        public int EmpresaId { get; set; }
        public int GrupoId { get; set; }
        public string Grupo { get; set; }
        
        public EmpresaViewModel Empresa { get; set; }
    }
}