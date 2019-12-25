using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cipa.Domain.Entities
{
    public class Eleicao : Entity<int>
    {
        public Eleicao(
            DateTime dataInicio,
            int duracaoGestao,
            DateTime? terminoMandatoAnterior,
            Usuario usuarioCriacao,
            Estabelecimento estabelecimento,
            Grupo grupo)
        {
            DataInicio = dataInicio;
            DuracaoGestao = duracaoGestao;
            TerminoMandatoAnterior = terminoMandatoAnterior;
            Estabelecimento = estabelecimento ?? throw new CustomException("O estabelecimento deve ser informado.");
            if (Estabelecimento.EleicoesDoAnoCorrente.Any())
                throw new CustomException($"Já há uma eleição cadastrada para este estabelecimento no ano de {Gestao}.");
            if (Estabelecimento.EleicoesEmAndamento.Any())
                throw new CustomException($"Existe uma eleição aberta para este estabelecimento.");
            Usuario = usuarioCriacao ?? throw new CustomException("O usuário de criação deve ser informado.");
            _grupo = grupo ?? estabelecimento.Grupo ?? throw new CustomException("O grupo deve ser informado.");
            Conta = usuarioCriacao.Conta ?? throw new CustomException("O usuário de criação da eleição deve estar vinculado à uma conta.");
            _dimensionamento = new Dimensionamento(0, 0, 0, 0);
        }

        public Eleicao(
            int id,
            DateTime dataInicio,
            int duracaoGestao,
            int estabelecimentoId,
            int grupoId,
            int usuarioCriacaoId,
            int contaId,
            DateTime? terminoMandatoAnterior)
        {
            Id = id;
            DataInicio = dataInicio;
            EstabelecimentoId = estabelecimentoId;
            _gestao = terminoMandatoAnterior?.Year ?? dataInicio.Year;
            DuracaoGestao = duracaoGestao;
            UsuarioCriacaoId = usuarioCriacaoId;
            ContaId = contaId;
            GrupoId = grupoId == 0 ? throw new CustomException("O grupo precisa ser informado para a abertura da eleição.") : grupoId;
            TerminoMandatoAnterior = terminoMandatoAnterior;
            DataFinalizacaoPrevista = dataInicio.AddDays(60);
        }

        private int _gestao;
        public int Gestao
        {
            get => _gestao;
        }
        public int DuracaoGestao { get; set; }
        public int EstabelecimentoId { get; private set; }
        private DateTime _dataInicio;
        public DateTime DataInicio
        {
            get => _dataInicio;
            set
            {
                _dataInicio = value;
                if (!TerminoMandatoAnterior.HasValue)
                    _gestao = _dataInicio.Year;
            }
        }
        public int UsuarioCriacaoId { get; private set; }
        public int ContaId { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime? DataFinalizacao { get; private set; }
        public DateTime DataFinalizacaoPrevista { get; private set; }
        public int GrupoId { get; private set; }
        private DateTime? terminoMandatoAnterior;
        public DateTime? TerminoMandatoAnterior
        {
            get => terminoMandatoAnterior;
            set
            {
                terminoMandatoAnterior = value;
                if (terminoMandatoAnterior.HasValue)
                    _gestao = terminoMandatoAnterior.Value.Year;
            }
        }
        // public bool UsuarioEleitor { get; set; }

        public virtual Estabelecimento Estabelecimento { get; private set; }
        public virtual Usuario Usuario { get; private set; }
        public virtual Conta Conta { get; private set; }
        private Grupo _grupo;
        public virtual Grupo Grupo
        {
            get => _grupo;
            set
            {
                if (_grupo != value)
                {
                    var novoDimensionamento = value.CalcularDimensionamento(Dimensionamento.QtdaEleitores);
                    if (novoDimensionamento == null)
                        throw new CustomException("Não foi encontrado o dimensionamento adequado para a eleição. Por favor, contate o suporte.");
                    if (JaUltrapassouEtapa(ECodigoEtapaObrigatoria.Inscricao) && novoDimensionamento.TotalCipeiros > Dimensionamento.QtdaInscricoesAprovadas)
                        throw new CustomException($"Para o grupo {value.CodigoGrupo}, o mínimo de inscrições necessária é {novoDimensionamento.TotalCipeiros}, e só houveram {Dimensionamento.QtdaInscricoesAprovadas} inscrições aprovadas nessa eleição.");
                    AtualizarDimensionamento(novoDimensionamento);
                }
                _grupo = value;
            }
        }
        private Dimensionamento _dimensionamento;
        public virtual Dimensionamento Dimensionamento { get => _dimensionamento ?? new Dimensionamento(0, 0, 0, 0); }
        public virtual ICollection<Inscricao> Inscricoes { get; } = new List<Inscricao>();
        private List<EtapaCronograma> _cronograma = new List<EtapaCronograma>();
        public virtual IReadOnlyCollection<EtapaCronograma> Cronograma
        {
            get => new ReadOnlyCollection<EtapaCronograma>(_cronograma.OrderBy(e => e.Ordem).ToList());
        }
        public virtual ICollection<Eleitor> Eleitores { get; } = new List<Eleitor>();
        public virtual ICollection<Voto> Votos { get; } = new List<Voto>();
        public virtual ICollection<Importacao> Importacoes { get; } = new List<Importacao>();
        public virtual ICollection<ProcessamentoEtapa> ProcessamentosEtapas { get; } = new List<ProcessamentoEtapa>();

        public EtapaCronograma EtapaAtual
        {
            get => Cronograma.FirstOrDefault(c => c.PosicaoEtapa == EPosicaoEtapa.Atual);
        }

        public EtapaCronograma UltimaEtapa
        {
            get => Cronograma.OrderByDescending(e => e.Ordem).FirstOrDefault();
        }

        public EtapaCronograma EtapaAnterior
        {
            get => RetonarEtapaAnterior(EtapaAtual);
        }

        public EtapaCronograma EtapaPosterior
        {
            get
            {
                if (EtapaAtual == null)
                {
                    // Se ainda não iniciou o processo, a próxima etapa é a primeira etapa do cronograma
                    if (Cronograma.All(e => e.PosicaoEtapa == EPosicaoEtapa.Futura))
                        return Cronograma.FirstOrDefault();
                    else if (Cronograma.All(e => e.PosicaoEtapa == EPosicaoEtapa.Passada))
                        return null;
                    else
                        throw new CustomException("Cronograma inconsistente.");
                }
                return RetornarEtapaPosterior(EtapaAtual);
            }
        }

        public void GerarCronograma()
        {
            var ordem = 1;
            var data = DataInicio;
            foreach (var etapaPadrao in Conta.EtapasPadroes.OrderBy(e => e.Ordem))
            {
                var etapa = new EtapaCronograma(etapaPadrao.Nome, etapaPadrao.Descricao, ordem, Id, data, etapaPadrao.EtapaObrigatoriaId)
                {
                    Eleicao = this,
                    EtapaObrigatoria = etapaPadrao.EtapaObrigatoria
                };
                _cronograma.Add(etapa);
                data = data.AddDays(etapaPadrao.DuracaoPadrao);
                ordem++;
            }
            DataFinalizacaoPrevista = data.AddDays(-1);
        }

        public void AtualizarCronograma(EtapaCronograma etapaCronograma)
        {
            var etapaExistente = Cronograma.FirstOrDefault(e => etapaCronograma.Equals(e));
            if (etapaExistente == null) throw new NotFoundException("Etapa não encontrada no cronograma.");
            etapaCronograma.EtapaObrigatoria = etapaExistente.EtapaObrigatoria;
            etapaCronograma.EtapaObrigatoriaId = etapaExistente.EtapaObrigatoriaId;

            if (etapaExistente.DataRealizada.HasValue)
                throw new CustomException("Não é possível alterar uma etapa já finalizada.");

            var etapaPosterior = RetornarEtapaPosterior(etapaExistente);
            var etapaAnterior = RetonarEtapaAnterior(etapaExistente);

            ValidarAlteracaoDataEtapaCronograma(etapaCronograma, etapaAnterior, etapaPosterior);

            etapaExistente.Nome = etapaCronograma.Nome;
            etapaExistente.Descricao = etapaCronograma.Descricao;
            etapaExistente.DataPrevista = etapaCronograma.DataPrevista;
        }

        private void ValidarAlteracaoDataEtapaCronograma(EtapaCronograma etapaAtualizada, EtapaCronograma etapaAnterior, EtapaCronograma etapaPosterior)
        {
            if (etapaAnterior != null)
            {
                // Valida se a data é maior que a data da etapa anterior
                if (etapaAtualizada.DataPrevista <= etapaAnterior.Data)
                    throw new CustomException($"A data deve ser maior que a data da etapa anterior ({etapaAnterior.Data.ToString("dd/MM/yyyy")})!");

                // Valida se etapa anterior possui duração mínima
                if (!PossuiDuracaoMinima(etapaAnterior, etapaAtualizada.DataPrevista))
                    throw new CustomException($"A etapa anterior ({etapaAnterior.Nome}) deve ter a duração mínima de {etapaAnterior.EtapaObrigatoria.DuracaoMinima} dias!");
            }

            if (etapaPosterior != null)
            {
                // Valida se a data é menor que a data da etapa posterior
                if (etapaAtualizada.DataPrevista >= etapaPosterior.DataPrevista)
                    throw new CustomException($"A data deve ser menor que a data da próxima etapa ({etapaPosterior.DataPrevista.ToString("dd/MM/yyyy")})!");

                // Valida se possui duração mínima
                if (!PossuiDuracaoMinima(etapaAtualizada, etapaPosterior.DataPrevista))
                    throw new CustomException($"Essa etapa deve ter a duração mínima de {etapaAtualizada.EtapaObrigatoria.DuracaoMinima} dias!");
            }
        }

        private bool PossuiDuracaoMinima(EtapaCronograma etapa, DateTime dataProximaEtapa)
        {
            if (etapa?.EtapaObrigatoria != null && etapa.EtapaObrigatoria.DuracaoMinima.HasValue)
            {
                var difData = (dataProximaEtapa - etapa.DataPrevista).Days;
                if (difData < etapa.EtapaObrigatoria.DuracaoMinima.Value)
                    return false;
            }
            return true;
        }

        private void AtualizarDimensionamento()
        {
            _dimensionamento = new Dimensionamento(Dimensionamento.Maximo, Dimensionamento.Minimo, Dimensionamento.QtdaEfetivos, Dimensionamento.QtdaSuplentes)
            {
                QtdaInscricoesAprovadas = QtdaInscricoes(StatusInscricao.Aprovada),
                QtdaInscricoesReprovadas = QtdaInscricoes(StatusInscricao.Reprovada),
                QtdaInscricoesPendentes = QtdaInscricoes(StatusInscricao.Pendente),
                QtdaVotos = Votos.Count,
                QtdaEleitores = Eleitores.Count,
            };
        }

        private void AtualizarDimensionamento(LinhaDimensionamento linhaDimensionamento)
        {
            Dimensionamento.QtdaEfetivos = linhaDimensionamento.QtdaEfetivos;
            Dimensionamento.QtdaSuplentes = linhaDimensionamento.QtdaSuplentes;
            Dimensionamento.Minimo = linhaDimensionamento.Minimo;
            Dimensionamento.Maximo = linhaDimensionamento.Maximo;
            AtualizarDimensionamento();
        }

        public bool JaUltrapassouEtapa(ECodigoEtapaObrigatoria etapaObrigatoria)
        {
            var etapa = Cronograma.FirstOrDefault(e => e.EtapaObrigatoriaId == etapaObrigatoria);
            if (etapa == null) throw new NotFoundException("Etapa não encontrada.");
            return etapa.PosicaoEtapa == EPosicaoEtapa.Passada;
        }

        public bool AindaNaoUltrapassouEtapa(ECodigoEtapaObrigatoria etapaObrigatoria)
        {
            var etapa = Cronograma.FirstOrDefault(e => e.EtapaObrigatoriaId == etapaObrigatoria);
            if (etapa == null) throw new CustomException("Etapa não encontrada.");
            return etapa.PosicaoEtapa == EPosicaoEtapa.Futura;
        }

        public EtapaCronograma BuscarEtapaObrigatoria(ECodigoEtapaObrigatoria etapaObrigatoria)
        {
            var etapa = Cronograma.FirstOrDefault(e => e.EtapaObrigatoriaId == etapaObrigatoria);
            if (etapa == null) throw new CustomException("Etapa não encontrada.");
            return etapa;
        }

        public EtapaCronograma RetonarEtapaAnterior(EtapaCronograma etapa)
        {
            if (etapa == null) return null;
            return Cronograma.Where(e => e.Ordem < etapa.Ordem).OrderByDescending(e => e.Ordem).FirstOrDefault();
        }

        public EtapaCronograma RetornarEtapaPosterior(EtapaCronograma etapa) =>
            Cronograma.Where(e => e.Ordem > etapa.Ordem).OrderBy(e => e.Ordem).FirstOrDefault();


        public DateTime DataTerminoEtapa(EtapaCronograma etapa)
        {
            var etapaPosterior = RetornarEtapaPosterior(etapa);
            if (etapaPosterior == null)
            {
                if (etapa == UltimaEtapa)
                {
                    return DataFinalizacao ?? DataFinalizacaoPrevista;
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

        public Eleitor BuscarEleitorPeloEmail(string email) => Eleitores.FirstOrDefault(e => e.Email == email);

        public Eleitor BuscarEleitorPeloUsuarioId(int id) => Eleitores.FirstOrDefault(e => e.UsuarioId == id);

        private void ValidarMudancaEtapa()
        {
            if (EtapaAtual == null) return;
            if (EtapaAtual.EtapaObrigatoriaId == ECodigoEtapaObrigatoria.Inscricao)
            {
                if (!Dimensionamento.PossuiQtdaMinimaInscritos)
                    throw new CustomException("Esta eleição ainda não possui a quantidade mínima de inscritos!");

                var inscricoesPendentes = Dimensionamento.QtdaInscricoesPendentes;
                if (inscricoesPendentes > 0)
                    throw new CustomException($"Ainda há {inscricoesPendentes} {(inscricoesPendentes > 1 ? "inscrições pendentes" : "inscrição pendente")} de aprovação!");

                _dimensionamento = Dimensionamento;
            }

            if (EtapaAtual.EtapaObrigatoriaId == ECodigoEtapaObrigatoria.Votacao && !Dimensionamento.PossuiQtdaMinimaVotos)
            {
                throw new CustomException("Esta eleição ainda não atingiu os 50% de participação de todos os funcionários, conforme exigido pela NR-5.");
            }
        }

        public void PassarParaProximaEtapa(bool registrarErro = false)
        {
            try
            {
                ValidarMudancaEtapa();
                var data = DateTime.Today;
                var proximaEtapa = EtapaPosterior;
                if (EtapaAtual != null)
                {
                    if (EtapaAtual.EtapaObrigatoriaId == ECodigoEtapaObrigatoria.Votacao)
                        RegistrarResultadoApuracao();
                    EtapaAtual.ErroMudancaEtapa = null;
                    EtapaAtual.PosicaoEtapa = EPosicaoEtapa.Passada;
                }

                if (proximaEtapa != null)
                {
                    ProcessamentosEtapas.Add(new ProcessamentoEtapa(this, proximaEtapa, EtapaAtual));
                    proximaEtapa.DataRealizada = data;
                    proximaEtapa.PosicaoEtapa = EPosicaoEtapa.Atual;
                }
                else // Eleição finalizada
                {
                    DataFinalizacao = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                if (registrarErro && EtapaAtual != null)
                    EtapaAtual.ErroMudancaEtapa = ex.Message;
                throw;
            }
        }

        public Eleitor AdicionarEleitor(Eleitor eleitor)
        {
            eleitor.Email = eleitor.Email.Trim().ToLower();

            if (JaUltrapassouEtapa(ECodigoEtapaObrigatoria.Votacao))
                throw new CustomException("Não é permitido cadastrar eleitores após o período de votação.");

            if (Eleitores.Any(e => e.Email == eleitor.Email))
                throw new CustomException("Já existe um eleitor cadastrado com o mesmo e-mail para essa eleição.");

            LinhaDimensionamento novoDimensionamento = Grupo.CalcularDimensionamento(Dimensionamento.QtdaEleitores + 1);
            if (JaUltrapassouEtapa(ECodigoEtapaObrigatoria.Inscricao) && Dimensionamento.QtdaInscricoesAprovadas < novoDimensionamento.TotalCipeiros)
                throw new CustomException($"Não é possível adicionar esse novo eleitor, pois sua inclusão altera o dimensionamento da eleição e com isso a quantidade mínima de inscritos passa a ser superior à quantidade atual de inscritos.");

            Eleitores.Add(eleitor);
            AtualizarDimensionamento(novoDimensionamento);

            return eleitor;
        }

        public Inscricao FazerInscricao(Eleitor eleitor, string objetivos)
        {
            if (EtapaAtual?.EtapaObrigatoriaId != ECodigoEtapaObrigatoria.Inscricao)
                throw new CustomException("As inscrições podem ser realizadas somente no período de inscrição. Confira o cronograma da eleição.");

            if (Inscricoes.Any(i => i.Eleitor == eleitor))
                throw new CustomException("Esse eleitor já está inscrito na eleição.");

            var inscricao = new Inscricao(this, eleitor, objetivos);
            Inscricoes.Add(inscricao);
            AtualizarDimensionamento();
            return inscricao;
        }

        public Inscricao BuscarInscricaoPeloId(int inscricaoId) => Inscricoes.FirstOrDefault(i => i.Id == inscricaoId);

        public Inscricao BuscarInscricaoPeloEleitorId(int eleitorId) => Inscricoes.FirstOrDefault(i => i.EleitorId == eleitorId);

        public Inscricao AtualizarInscricao(int eleitorId, string objetivos)
        {
            if (EtapaAtual?.EtapaObrigatoriaId != ECodigoEtapaObrigatoria.Inscricao)
                throw new CustomException("As inscrições não podem ser alteradas fora do período de inscrição.");

            var inscricao = BuscarInscricaoPeloEleitorId(eleitorId);
            if (inscricao == null) throw new NotFoundException("Inscrição não encontrada.");
            inscricao.AtualizarInscricao(objetivos);
            AtualizarDimensionamento();
            return inscricao;
        }

        public Inscricao AprovarInscricao(int inscricaoId, Usuario usuarioAprovador)
        {
            if (EtapaAtual?.EtapaObrigatoriaId != ECodigoEtapaObrigatoria.Inscricao)
                throw new CustomException("As inscrições não podem ser aprovadas fora do período de inscrição.");

            var inscricao = BuscarInscricaoPeloId(inscricaoId);
            if (inscricao == null) throw new NotFoundException("Inscrição não encontrada.");
            inscricao.AprovarInscricao(usuarioAprovador);
            AtualizarDimensionamento();
            return inscricao;
        }

        public Inscricao ReprovarInscricao(int inscricaoId, Usuario usuarioAprovador, string motivoReprovacao)
        {
            if (EtapaAtual?.EtapaObrigatoriaId != ECodigoEtapaObrigatoria.Inscricao)
                throw new CustomException("As inscrições não podem ser reprovadas fora do período de inscrição.");

            var inscricao = BuscarInscricaoPeloId(inscricaoId);
            if (inscricao == null) throw new NotFoundException("Inscrição não encontrada.");
            inscricao.ReprovarInscricao(usuarioAprovador, motivoReprovacao);
            AtualizarDimensionamento();
            return inscricao;
        }

        private bool EleitorJaVotou(Eleitor eleitor) => Votos.Any(v => v.Eleitor == eleitor);

        public bool ExcluirEleitor(Eleitor eleitor)
        {
            if (!Eleitores.Contains(eleitor))
                throw new NotFoundException("Eleitor não encontrado.");

            var inscricao = BuscarInscricaoPeloEleitorId(eleitor.Id);
            if (inscricao != null && inscricao.StatusInscricao != StatusInscricao.Reprovada)
                throw new CustomException("Não é possível excluir esse eleitor pois ele é um dos inscritos nessa eleição!");
            if (EleitorJaVotou(eleitor))
                throw new CustomException("Não é possível excluir esse eleitor pois ele já votou nessa eleição!");

            var excluido = Eleitores.Remove(eleitor);
            LinhaDimensionamento novoDimensionamento = Grupo.CalcularDimensionamento(Dimensionamento.QtdaEleitores - 1);
            AtualizarDimensionamento(novoDimensionamento);
            return excluido;
        }


        public void ExcluirTodosEleitores()
        {
            if (EtapaAtual == null && Cronograma.All(etapa => etapa.PosicaoEtapa == EPosicaoEtapa.Futura))
                Eleitores.ToList().ForEach(e => ExcluirEleitor(e));
            else
                throw new CustomException("A exclusão em massa dos eleitores é permitida somente antes do início do processo.");
        }

        private Voto RegistrarVoto(Eleitor eleitor, string ip)
        {
            if (EleitorJaVotou(eleitor))
                throw new CustomException("Eleitor já registrou seu voto nessa eleição. Não é possível votar mais de uma vez.");

            if (EtapaAtual?.EtapaObrigatoriaId != ECodigoEtapaObrigatoria.Votacao)
                throw new CustomException("Essa eleição não está no período de votação.");

            var registroVoto = new Voto(eleitor, ip);
            Votos.Add(registroVoto);
            AtualizarDimensionamento();
            return registroVoto;
        }

        public Voto RegistrarVoto(int inscricaoId, Eleitor eleitor, string ip)
        {
            var inscricao = BuscarInscricaoPeloId(inscricaoId);
            if (inscricao == null || inscricao.StatusInscricao != StatusInscricao.Aprovada)
                throw new NotFoundException("Inscrição não encontrada.");

            inscricao.Votos++;
            return RegistrarVoto(eleitor, ip);
        }

        public Voto VotarEmBranco(Eleitor eleitor, string ip) => RegistrarVoto(eleitor, ip);

        public Voto BuscarVotoEleitor(Eleitor eleitor) => Votos.FirstOrDefault(v => v.Eleitor == eleitor);

        private IEnumerable<Inscricao> OrdenaInscricoesPorQtdaVotos() =>
            Inscricoes.OrderByDescending(i => i.Votos).ThenBy(i => i.Eleitor.DataAdmissao)
                .ThenBy(i => i.Eleitor.DataNascimento);

        private void RegistrarResultadoApuracao()
        {
            foreach (var inscricaoApurada in ApurarVotos())
            {
                var inscricao = Inscricoes.FirstOrDefault(i => i == inscricaoApurada);
                if (inscricao != null) inscricao.ResultadoApuracao = inscricaoApurada.ResultadoApuracao;
            }
        }

        public IEnumerable<Inscricao> ApurarVotos()
        {
            List<Inscricao> apuracao = OrdenaInscricoesPorQtdaVotos().ToList();

            var qtdaVotosEmBranco = Votos.Count - Inscricoes.Sum(i => i.Votos);
            var votosEmBranco = new Inscricao(this, new Eleitor("(Em Branco)", "(Em Branco)"), "(Em Branco)")
            {
                Votos = qtdaVotosEmBranco
            };
            apuracao.Add(votosEmBranco);

            if (!Dimensionamento.PossuiQtdaMinimaVotos) return apuracao;

            var efetivos = apuracao
               .Take(Dimensionamento.QtdaEfetivos)
               .Select(inscricao => MapeiaInscricaoComResultado(inscricao, ResultadoApuracao.Efetivo));

            var suplentes = apuracao
                .Skip(Dimensionamento.QtdaEfetivos)
                .Take(Dimensionamento.QtdaSuplentes)
                .Select(inscricao => MapeiaInscricaoComResultado(inscricao, ResultadoApuracao.Suplente));

            var naoEleitos = apuracao
                .Skip(Dimensionamento.QtdaEfetivos + Dimensionamento.QtdaSuplentes)
                .Select(inscricao => MapeiaInscricaoComResultado(inscricao, ResultadoApuracao.NaoEleito));

            return efetivos.Union(suplentes).Union(naoEleitos);
        }

        private Inscricao MapeiaInscricaoComResultado(Inscricao inscricao, ResultadoApuracao resultado)
        {
            inscricao.ResultadoApuracao = resultado;
            return inscricao;
        }

        public Eleitor AtualizarEleitor(Eleitor eleitor)
        {
            var eleitorExistente = BuscarEleitor(eleitor.Id);
            if (eleitorExistente == null) throw new NotFoundException("Eleitor não encontrado.");

            if (eleitorExistente.Email != eleitor.Email && Eleitores.Any(e => e.Email == eleitor.Email))
                throw new CustomException("Já existe um eleitor cadastrado com o mesmo e-mail para essa eleição.");

            eleitorExistente.Atualizar(eleitor);

            return eleitorExistente;
        }

        public ProcessamentoEtapa RetornarUltimoProcessamentoEtapa() =>
            ProcessamentosEtapas.OrderBy(p => p.HorarioMudancaEtapa).LastOrDefault();
    }

}