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

        public void AtualizarInscricao(string objetivos)
        {
            Objetivos = objetivos;
            StatusInscricao = StatusInscricao.Pendente;
        }
        
        public void AprovarInscricao(Usuario usuarioAprovador)
        {
            StatusInscricao = StatusInscricao.Aprovada;
            EmailAprovador = usuarioAprovador.Email;
            NomeAprovador = usuarioAprovador.Nome;
        }

        public void ReprovarInscricao(Usuario usuarioAprovador, string motivoReprovacao)
        {
            StatusInscricao = StatusInscricao.Reprovada;
            Reprovacoes.Add(new Reprovacao
            {
                EmailAprovador = usuarioAprovador.Email,
                MotivoReprovacao = motivoReprovacao,
                NomeAprovador = usuarioAprovador.Nome
            });
        }
    }
}