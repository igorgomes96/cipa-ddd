using System;

namespace Cipa.WebApi.ViewModels
{

    public class ImportacaoViewModel
    {
        public int Id { get; set; }
        public DateTime Horario { get; set; }
        public string Status { get; set; }
        public ArquivoViewModel Arquivo { get; set; }

    }
}
