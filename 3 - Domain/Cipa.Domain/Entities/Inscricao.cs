using System;
using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public enum StatusInscricao
    {
        Pendente,
        Aprovada,
        Reprovada
    }

    public class Inscricao : Entity<int>
    {
        public int Votos { get; set; }
        public StatusInscricao StatusInscricao { get; set; }
        public int EleitorId { get; set; }
        public int EleicaoId { get; set; }
        public string Foto { get; set; }
        public string Objetivos { get; set; }
        public string EmailAprovador { get; set; }
        public string NomeAprovador { get; set; }
        public DateTime? HorarioAprovacao { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Eleitor Eleitor { get; set; }
        public virtual Eleicao Eleicao { get; set; }
        public virtual ICollection<Reprovacao> Reprovacoes { get; } = new List<Reprovacao>();
    }
}