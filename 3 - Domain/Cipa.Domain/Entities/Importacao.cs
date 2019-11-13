using System;

namespace Cipa.Domain.Entities
{
    public enum StatusImportacao {
        Aguardando,
        Processando,
        FinalizadoComSucesso,
        FinalizadoComFalha
    }

    public class Importacao
    {
        public int Id { get; set; }
        public DateTime Horario { get; set; }
        public int ArquivoId { get; set; }
        public StatusImportacao Status { get; set; } = StatusImportacao.Aguardando;
        public int EleicaoId { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Arquivo Arquivo { get; set; }
        public virtual Eleicao Eleicao { get; set; }
    }
}
