using System;

namespace Cipa.WebApi.ViewModels
{
    
    public class ArquivoViewModel
    {
        public int Id { get; set; }
        public DateTime DataUpload { get; set; }
        public string Nome { get; set; }
        public long Tamanho { get; set; }
        public string ContentType { get; set; }

    }
}
