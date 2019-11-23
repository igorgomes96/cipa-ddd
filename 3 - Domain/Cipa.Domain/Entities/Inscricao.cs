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

    public enum ResultadoApuracao
    {
        NaoEleito,
        Efetivo,
        Suplente
    }

    public class Inscricao : Entity<int>
    {
        public Inscricao(int eleicaoId, int eleitorId, string objetivos)
        {
            EleitorId = eleitorId;
            EleicaoId = eleicaoId;
            Objetivos = objetivos;
            StatusInscricao = StatusInscricao.Pendente;
            Votos = 0;
        }
        public Inscricao(Eleicao eleicao, Eleitor eleitor, string objetivos)
        {
            Eleicao = eleicao;
            Eleitor = eleitor;
            Objetivos = objetivos;
            StatusInscricao = StatusInscricao.Pendente;
            Votos = 0;
        }
        public int Votos { get; internal set; }
        public StatusInscricao StatusInscricao { get; internal set; }
        public int EleitorId { get; private set; }
        public int EleicaoId { get; private set; }
        public string Foto { get; set; }
        public string Objetivos { get; set; }
        public string EmailAprovador { get; set; }
        public string NomeAprovador { get; set; }
        public DateTime? HorarioAprovacao { get; set; }
        public DateTime DataCadastro { get; set; }
        public ResultadoApuracao ResultadoApuracao { get; internal set; } = ResultadoApuracao.NaoEleito;

        public virtual Eleitor Eleitor { get; private set; }
        public virtual Eleicao Eleicao { get; private set; }
        public virtual ICollection<Reprovacao> Reprovacoes { get; } = new List<Reprovacao>();

        public void AtualizarInscricao(string objetivos)
        {
            Objetivos = objetivos;
            StatusInscricao = StatusInscricao.Pendente;
        }

        internal void AprovarInscricao(Usuario usuarioAprovador)
        {
            StatusInscricao = StatusInscricao.Aprovada;
            EmailAprovador = usuarioAprovador.Email;
            NomeAprovador = usuarioAprovador.Nome;
            HorarioAprovacao = DateTime.Now;
        }

        internal void ReprovarInscricao(Usuario usuarioAprovador, string motivoReprovacao)
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