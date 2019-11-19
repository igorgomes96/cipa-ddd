using System;
using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;

namespace Cipa.Domain.Entities
{
    public class Eleicao : Entity<int>
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

        public EtapaCronograma UltimaEtapa
        {
            get
            {
                return Cronograma.OrderByDescending(e => e.Ordem).FirstOrDefault();
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
        }

        public void AtualizarDimensionamento(LinhaDimensionamento linhaDimensionamento)
        {
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
            if (etapa == null) return null;
            return Cronograma.Where(e => e.Ordem < etapa.Ordem).OrderByDescending(e => e.Ordem).FirstOrDefault();
        }

        public EtapaCronograma EtapaAnterior()
        {
            return EtapaAnterior(EtapaAtual);
        }

        public EtapaCronograma EtapaPosterior(EtapaCronograma etapa) =>
            Cronograma.Where(e => e.Ordem > etapa.Ordem).OrderBy(e => e.Ordem).FirstOrDefault();

        public EtapaCronograma EtapaPosterior()
        {
            // Se ainda não iniciou o processo, a próxima etapa é a primeira etapa do cronograma
            if (EtapaAtual == null && Cronograma.All(e => e.PosicaoEtapa == PosicaoEtapa.Futura))
                return Cronograma.FirstOrDefault();
            return EtapaPosterior(EtapaAtual);
        }

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

        public Eleitor BuscarEleitor(int id) => Eleitores.FirstOrDefault(e => e.Id == id);

        public Eleitor BuscarEleitorPeloIdUsuario(int usuarioId) => Eleitores.FirstOrDefault(e => e.UsuarioId == usuarioId);

        private void ValidarMudancaEtapa(Dimensionamento dimensionamento)
        {
            if (EtapaAtual == null) return;
            if (EtapaAtual.EtapaObrigatoriaId == CodigoEtapaObrigatoria.Inscricao)
            {
                if (!dimensionamento.PossuiQtdaMinimaInscritos)
                    throw new CustomException("Esta eleição ainda não possui a quantidade mínima de inscritos!");

                var inscricoesPendentes = dimensionamento.QtdaInscricoesPendentes;
                if (inscricoesPendentes > 0)
                    throw new CustomException($"Ainda há {inscricoesPendentes} {(inscricoesPendentes > 1 ? "inscrições pendentes" : "inscrição pendente")} de aprovação!");
            }

            if (EtapaAtual.EtapaObrigatoriaId == CodigoEtapaObrigatoria.Votacao && !dimensionamento.PossuiQtdaMinimaVotos)
            {
                throw new CustomException("Esta eleição ainda não atingiu os 50% de participação de todos os funcionários, conforme exigido pela NR-5.");
            }
        }

        public void PassarParaProximaEtapa(Dimensionamento dimensionamento)
        {
            ValidarMudancaEtapa(dimensionamento);
            var data = DateTime.Today;
            var proximaEtapa = EtapaPosterior();
            if (EtapaAtual != null)
            {
                EtapaAtual.ErroMudancaEtapa = null;
                EtapaAtual.PosicaoEtapa = PosicaoEtapa.Passada;
            }

            if (proximaEtapa != null)
            {
                proximaEtapa.DataRealizada = data;
                proximaEtapa.PosicaoEtapa = PosicaoEtapa.Atual;
            }
            else // Eleição finalizada
            {
                DataFinalizacao = DateTime.Now;
            }
        }

        public Eleitor AdicionarEleitor(
            Eleitor eleitor, Dimensionamento dimensionamentoAtual, Dimensionamento dimensionamentoProposto)
        {
            eleitor.Email = eleitor.Email.Trim().ToLower();

            if (JaUltrapassouEtapa(CodigoEtapaObrigatoria.Votacao))
            {
                throw new CustomException("Não é permitido cadastrar eleitores após o período de votação.");
            }

            if (Eleitores.Any(e => e.Email == eleitor.Email))
            {
                throw new CustomException("Já existe um eleitor cadastrado com o mesmo e-mail para essa eleição.");
            }

            if (JaUltrapassouEtapa(CodigoEtapaObrigatoria.Inscricao) && (dimensionamentoAtual.QtdaEleitores + 1) >= dimensionamentoAtual.Maximo)
            {
                if (dimensionamentoAtual.QtdaInscricoesAprovadas < dimensionamentoProposto.TotalCipeiros)
                {
                    throw new CustomException($"Não é possível adicionar esse novo eleitor, pois sua inclusão altera o dimensionamento da eleição e com isso a quantidade mínima de inscritos passa a ser superior à quantidade atual de inscritos.");
                }
                else
                {
                    Dimensionamento = dimensionamentoProposto;
                }
            }
            Eleitores.Add(eleitor);
            return eleitor;
        }
    }

}