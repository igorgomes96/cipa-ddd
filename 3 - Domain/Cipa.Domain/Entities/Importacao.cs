using System;
using System.Collections.Generic;
using System.Linq;

namespace Cipa.Domain.Entities
{
    public enum StatusImportacao
    {
        Aguardando,
        Processando,
        FinalizadoComSucesso,
        FinalizadoComFalha
    }

    public class Importacao : Entity<int>
    {
        public Importacao()
        {
            _inconsistencias = new List<Inconsistencia>();
            Status = StatusImportacao.Aguardando;
        }

        public Importacao(Arquivo arquivo, Eleicao eleicao)
        {
            Arquivo = arquivo;
            Eleicao = eleicao;
            _inconsistencias = new List<Inconsistencia>();
            Status = StatusImportacao.Aguardando;
        }

        // public int ArquivoId { get; set; }
        public StatusImportacao Status { get; private set; }
        public int EleicaoId { get; set; }
        public DateTime DataCadastro { get; private set; }

        public virtual Arquivo Arquivo { get; private set; }
        public virtual Eleicao Eleicao { get; private set; }
        private List<Inconsistencia> _inconsistencias;
        public virtual IReadOnlyCollection<Inconsistencia> Inconsistencias { get => _inconsistencias; }

        public void IniciarProcessamento()
        {
            _inconsistencias.Clear();
            Status = StatusImportacao.Processando;
        }

        public void FinalizarProcessamentoComSucesso()
        {
            _inconsistencias.Clear();
            Status = StatusImportacao.FinalizadoComSucesso;
        }

        public void FinalizarImportacaoComFalha(IEnumerable<Inconsistencia> inconsistencias)
        {
            Status = StatusImportacao.FinalizadoComFalha;
            if (inconsistencias != null && inconsistencias.Any())
                _inconsistencias.AddRange(inconsistencias);
        }

    }
}
