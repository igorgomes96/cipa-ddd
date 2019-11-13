using System;
using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class Eleicao
    {
        public Eleicao() { }

        public Eleicao(
            int id,
            DateTime dataInicio,
            int duracaoGestao,
            Estabelecimento estabelecimento,
            Grupo grupo,
            Usuario usuarioCriacao,
            DateTime? terminoMandatoAnterior)
        {
            Id = id;
            DataInicio = dataInicio;
            EstabelecimentoId = estabelecimento.Id;
            Estabelecimento = estabelecimento;
            Gestao = terminoMandatoAnterior?.Year ?? dataInicio.Year;
            DuracaoGestao = duracaoGestao;
            Usuario = usuarioCriacao;
            UsuarioCriacaoId = usuarioCriacao.Id;
            ContaId = usuarioCriacao.ContaId ?? throw new CustomException("A conta precisa ser informada para a abertura da eleição.");
            Conta = usuarioCriacao.Conta;
            Grupo = grupo ?? throw new CustomException("O grupo precisa ser informado para a abertura da eleição.");
            GrupoId = grupo.Id == 0 ? throw new CustomException("O grupo precisa ser informado para a abertura da eleição.") : grupo.Id;
            TerminoMandatoAnterior = terminoMandatoAnterior;
        }


        public int Id { get; set; }
        public int Gestao { get; set; }
        public int DuracaoGestao { get; set; }
        public int EstabelecimentoId { get; set; }
        public DateTime DataInicio { get; set; }
        public int UsuarioCriacaoId { get; set; }
        public int ContaId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public int GrupoId { get; set; }
        public DateTime? TerminoMandatoAnterior { get; set; }

        public virtual Estabelecimento Estabelecimento { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Conta Conta { get; set; }
        public virtual Grupo Grupo { get; set; }
        public virtual Dimensionamento Dimensionamento { get; private set; } = new Dimensionamento();
        public virtual ICollection<Inscricao> Inscricoes { get; } = new List<Inscricao>();
        public virtual ICollection<EtapaCronograma> Cronograma { get; } = new List<EtapaCronograma>();
        public virtual ICollection<Eleitor> Eleitores { get; } = new List<Eleitor>();
        public virtual ICollection<Voto> Votos { get; } = new List<Voto>();

        public EtapaCronograma EtapaAtual
        {
            get
            {
                return Cronograma?.FirstOrDefault(c => c.PosicaoEtapa == PosicaoEtapa.Atual);
            }
        }

        public void GerarCronograma()
        {
            var ordem = 1;
            var data = DataInicio;
            foreach (var etapaPadrao in Conta.EtapasPadroes.OrderBy(e => e.Ordem))
            {
                var etapa = new EtapaCronograma
                {
                    DataPrevista = data,
                    DataRealizada = null,
                    Descricao = etapaPadrao.Descricao,
                    EleicaoId = Id,
                    EtapaObrigatoriaId = etapaPadrao.EtapaObrigatoriaId,
                    Nome = etapaPadrao.Nome,
                    PosicaoEtapa = PosicaoEtapa.Futura,
                    Ordem = ordem
                };
                Cronograma.Add(etapa);
                data = data.AddDays(etapaPadrao.DuracaoPadrao);
                ordem++;
            }
            if (Cronograma.Count > 0)
                Cronograma.ElementAt(0).PosicaoEtapa = PosicaoEtapa.Atual;
        }

        public void AtualizarDimensionamento(LinhaDimensionamento linhaDimensionamento) {
            Dimensionamento.QtdaEfetivos = linhaDimensionamento.QtdaEfetivos;
            Dimensionamento.QtdaSuplentes = linhaDimensionamento.QtdaSuplentes;
            Dimensionamento.Minimo = linhaDimensionamento.Minimo;
            Dimensionamento.Maximo = linhaDimensionamento.Maximo;
        }

        public bool JaUltrapassouEtapa(CodigoEtapaObrigatoria etapaObrigatoria)
        {
            if (EtapaAtual == null)
            {
                return DataFinalizacao.HasValue;
            }
            var etapa = Cronograma.FirstOrDefault(e => e.EtapaObrigatoriaId == etapaObrigatoria);
            if (etapa == null) throw new CustomException("Etapa não encontrada.");
            return EtapaAtual.Ordem > etapa.Ordem;
        }

        public bool AindaNaoUltrapassouEtapa(CodigoEtapaObrigatoria etapaObrigatoria)
        {
            if (EtapaAtual == null)
            {
                return !DataFinalizacao.HasValue;
            }
            var etapa = Cronograma.FirstOrDefault(e => e.EtapaObrigatoriaId == etapaObrigatoria);
            if (etapa == null) throw new CustomException("Etapa não encontrada.");
            return EtapaAtual.Ordem < etapa.Ordem;
        }

        public EtapaCronograma BuscaEtapaObrigatoria(CodigoEtapaObrigatoria etapaObrigatoria)
        {
            var etapa = Cronograma.FirstOrDefault(e => e.EtapaObrigatoriaId == etapaObrigatoria);
            if (etapa == null) throw new CustomException("Etapa não encontrada.");
            return etapa;
        }

        public EtapaCronograma EtapaAnterior(EtapaCronograma etapa)
        {
            return Cronograma.Where(e => e.Ordem < etapa.Ordem).OrderByDescending(e => e.Ordem).FirstOrDefault();
        }

        // public EtapaCronograma EtapaAnterior(int etapaId)
        // {
        //     var etapa = Cronograma.FirstOrDefault(e => e.Id == etapaId);
        //     if (etapa == null) throw new CustomException("Etapa não encontrada.");
        //     return EtapaAnterior(etapa);
        // }

        public EtapaCronograma EtapaPosterior(EtapaCronograma etapa)
        {
            return Cronograma.Where(e => e.Ordem > etapa.Ordem).OrderBy(e => e.Ordem).FirstOrDefault();
        }

        // public EtapaCronograma EtapaPosterior(int etapaId)
        // {
        //     var etapa = Cronograma.FirstOrDefault(e => e.Id == etapaId);
        //     if (etapa == null) throw new CustomException("Etapa não encontrada.");
        //     return EtapaPosterior(etapa);
        // }

        public DateTime DataTerminoEtapa(EtapaCronograma etapa)
        {
            var etapaPosterior = EtapaPosterior(etapa);
            if (etapaPosterior == null)
            {
                if (etapa.Id == UltimaEtapa.Id)
                {
                    return DataFinalizacao.Value;
                }
                else
                {
                    throw new CustomException("Etapa não encontrada.");
                }
            }
            var dMenos1 = etapaPosterior.Data.AddDays(-1);
            return etapa.Data >= dMenos1 ? etapa.Data : dMenos1;
        }

        public int QtdaInscricoes(StatusInscricao statusInscricao) =>
            Inscricoes.Count(i => i.StatusInscricao == statusInscricao);

        public EtapaCronograma UltimaEtapa
        {
            get
            {
                return Cronograma.OrderByDescending(e => e.Ordem).FirstOrDefault();
            }
        }
    }

}