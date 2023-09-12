using Cipa.Application.Helpers;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Factories.Interfaces;
using Cipa.Domain.Helpers;
using Cipa.Application.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using Cipa.Application.Services.Interfaces;
using System;
using Cipa.Domain.Services.Interfaces;
using System.Threading.Tasks;

namespace Cipa.Application.Implementation
{
    public class EleicaoAppService : AppServiceBase<Eleicao>, IEleicaoAppService
    {
        private readonly IArquivoAppService _arquivoAppService;
        private readonly IFormatadorEmailServiceFactory _formatadorFactory;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IExcelService _excelService;
        private const string PATH_FOTOS = @"fotos/";
        private const string PATH_RELATORIOS = @"StaticFiles/Relatorios/";

        public EleicaoAppService(
            IUnitOfWork unitOfWork,
            IArquivoAppService arquivoAppService,
            IFormatadorEmailServiceFactory formatadorFactory,
            EmailConfiguration emailConfiguration,
            IExcelService excelService) : base(unitOfWork, unitOfWork.EleicaoRepository)
        {
            _arquivoAppService = arquivoAppService;
            _formatadorFactory = formatadorFactory;
            _emailConfiguration = emailConfiguration;
            _excelService = excelService;
        }

        public override Eleicao Adicionar(Eleicao eleicao)
        {
            var estabelecimento = _unitOfWork.EstabelecimentoRepository.BuscarPeloId(eleicao.EstabelecimentoId);
            if (estabelecimento == null)
                throw new NotFoundException($"Estabelecimento {eleicao.EstabelecimentoId} não encontrado.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarPeloId(eleicao.UsuarioCriacaoId);
            if (usuario == null)
                throw new NotFoundException($"Usuário {eleicao.UsuarioCriacaoId} não encontrado.");

            var grupo = _unitOfWork.GrupoRepository.BuscarPeloId(eleicao.GrupoId);
            if (grupo == null)
                throw new NotFoundException($"Grupo {eleicao.GrupoId} não encontrado.");

            var conta = _unitOfWork.ContaRepository.BuscarPeloId(eleicao.ContaId);
            if (conta == null)
                throw new NotFoundException($"Conta {eleicao.ContaId} não encontrada.");

            var novaEleicao = new Eleicao(
                eleicao.DataInicio,
                eleicao.DuracaoGestao,
                eleicao.TerminoMandatoAnterior,
                usuario, estabelecimento, grupo, conta);
            novaEleicao.GerarCronograma();

            return base.Adicionar(novaEleicao);
        }

        public override void Atualizar(Eleicao eleicao)
        {
            Eleicao eleicaoExistente = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicao.Id);
            if (eleicaoExistente == null) throw new NotFoundException("Código de eleição não encontrado.");

            eleicaoExistente.DataInicio = eleicao.DataInicio;
            eleicaoExistente.TerminoMandatoAnterior = eleicao.TerminoMandatoAnterior;
            eleicaoExistente.DuracaoGestao = eleicao.DuracaoGestao;

            var grupo = _unitOfWork.GrupoRepository.BuscarPeloId(eleicao.GrupoId);
            if (grupo == null)
                throw new NotFoundException($"Grupo {eleicao.GrupoId} não encontrado.");

            eleicaoExistente.Grupo = grupo;

            base.Atualizar(eleicaoExistente);
        }

        public override Eleicao Excluir(int id)
        {
            Eleicao eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(id);
            if (eleicao == null) throw new NotFoundException("Código de eleição não encontrado.");

            if (eleicao.EtapaAtual != null || eleicao.DataFinalizacao.HasValue)
            {
                throw new CustomException("Um eleição só pode ser excluída antes do início do processo.");
            }
            _unitOfWork.EleicaoRepository.Excluir(eleicao);
            _unitOfWork.Commit();
            return eleicao;
        }

        public IEnumerable<EtapaCronograma> BuscarCronograma(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");
            return eleicao.Cronograma.OrderBy(e => e.Ordem);
        }

        public IEnumerable<Eleitor> BuscarEleitores(int eleicaoId) =>
            _unitOfWork.EleicaoRepository.BuscarEleitores(eleicaoId).OrderBy(e => e.Nome);

        public Eleitor BuscarEleitorPeloIdUsuario(int eleicaoId, int usuarioId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            return eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
        }

        public IEnumerable<Inscricao> BuscarInscricoes(int eleicaoId, StatusInscricao? status) =>
            _unitOfWork.EleicaoRepository.BuscarInscricoes(eleicaoId, status);

        public Inscricao BuscarInscricaoPeloUsuario(int eleicaoId, int usuarioId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            return eleicao.BuscarInscricaoPeloEleitorId(eleitor.Id);
        }

        public IEnumerable<Eleicao> BuscarPelaConta(int contaId)
        {
            return _unitOfWork.EleicaoRepository.BuscarPelaConta(contaId);
        }

        public IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId)
        {
            return _unitOfWork.EleicaoRepository.BuscarPeloUsuario(usuarioId);
        }

        public Eleitor ExcluirEleitor(int eleicaoId, int eleitorId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitor(eleitorId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var eleitorExcluido = eleicao.ExcluirEleitor(eleitor);
            if (!eleitorExcluido)
                throw new CustomException("Erro ao excluir eleitor.");

            base.Atualizar(eleicao);
            return eleitor;
        }

        public void ExcluirTodosEleitores(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            eleicao.ExcluirTodosEleitores();
            base.Atualizar(eleicao);
        }

        public Eleicao PassarParaProximaEtapa(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            eleicao.PassarParaProximaEtapa();
            base.Atualizar(eleicao);
            return eleicao;
        }

        public Eleicao AtualizarConfiguracoes(int eleicaoId, ConfiguracaoEleicao configuracao)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            eleicao.AtualizarConfiguracoes(configuracao);
            base.Atualizar(eleicao);
            return eleicao;
        }

        public bool VerificarSeUsuarioEhEleitor(int eleicaoId, int usuarioId) =>
            (_repositoryBase as IEleicaoRepository).VerificarSeUsuarioEhEleitor(eleicaoId, usuarioId);

        public Eleitor AdicionarEleitor(int eleicaoId, Eleitor eleitor)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarUsuario(eleitor.Email);
            if (usuario == null)
                usuario = new Usuario(eleitor.Email, eleitor.Nome, eleitor.Cargo);
            eleitor.Usuario = usuario;

            var eleitorAdicionado = eleicao.AdicionarEleitor(eleitor);
            base.Atualizar(eleicao);
            return eleitorAdicionado;
        }

        private void EnviarNotificacaoInscricao(Inscricao inscricao, ETipoTemplateEmail tipoTemplate)
        {
            var formatador = _formatadorFactory.ObterFormatadorEmailParaNotificacoesInscricoes(tipoTemplate, inscricao);
            foreach (var email in formatador.FormatarEmails())
                _unitOfWork.EmailRepository.Adicionar(email);
        }

        public Inscricao FazerInscricao(int eleicaoId, int usuarioId, string objetivos)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var inscricao = eleicao.FazerInscricao(eleitor, objetivos);
            EnviarNotificacaoInscricao(inscricao, ETipoTemplateEmail.InscricaoRealizada);

            base.Atualizar(eleicao);
            return inscricao;
        }

        public Inscricao AtualizarInscricao(int eleicaoId, int usuarioId, string objetivos)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            bool solicitacaoReanalise = false;
            var inscricaoAntesDaAlteracao = eleicao.BuscarInscricaoPeloEleitorId(eleitor.Id);
            if (inscricaoAntesDaAlteracao.StatusInscricao == StatusInscricao.Reprovada) solicitacaoReanalise = true;

            var inscricao = eleicao.AtualizarInscricao(eleitor.Id, objetivos);

            if (solicitacaoReanalise)
                EnviarNotificacaoInscricao(inscricao, ETipoTemplateEmail.ReanaliseInscricao);

            base.Atualizar(eleicao);
            return inscricao;
        }

        public Inscricao AprovarInscricao(int eleicaoId, int inscricaoId, int usuarioId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarPeloId(usuarioId);
            if (usuario == null) throw new CustomException("Usuário inválido!");

            var inscricaoAprovada = eleicao.AprovarInscricao(inscricaoId, usuario);

            EnviarNotificacaoInscricao(inscricaoAprovada, ETipoTemplateEmail.InscricaoAprovada);

            base.Atualizar(eleicao);
            return inscricaoAprovada;
        }

        public Inscricao ReprovarInscricao(int eleicaoId, int inscricaoId, int usuarioId, string motivoReprovacao)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarPeloId(usuarioId);
            if (usuario == null) throw new CustomException("Usuário inválido!");

            var inscricaoReprovacao = eleicao.ReprovarInscricao(inscricaoId, usuario, motivoReprovacao);
            base.Atualizar(eleicao);

            EnviarNotificacaoInscricao(inscricaoReprovacao, ETipoTemplateEmail.InscricaoReprovada);

            _unitOfWork.Commit();
            return inscricaoReprovacao;
        }

        public Voto RegistrarVoto(int eleicaoId, int inscricaoId, int usuarioId, string ip)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var voto = eleicao.RegistrarVoto(inscricaoId, eleitor, ip);
            base.Atualizar(eleicao);
            return voto;
        }

        public Voto VotarEmBranco(int eleicaoId, int usuarioId, string ip)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            var voto = eleicao.VotarEmBranco(eleitor, ip);
            base.Atualizar(eleicao);
            return voto;
        }

        public IEnumerable<Voto> BuscarVotos(int eleicaoId) => _unitOfWork.EleicaoRepository.BuscarVotos(eleicaoId);

        public Voto BuscarVotoUsuario(int eleicaoId, int usuarioId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitorPeloUsuarioId(usuarioId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");

            return eleicao.BuscarVotoEleitor(eleitor);
        }

        public IEnumerable<Inscricao> ApurarVotos(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");
            return eleicao.ApurarVotos();
        }

        public IEnumerable<Inscricao> RegistrarResultadoApuracao(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");
            eleicao.RegistrarResultadoApuracao();
            base.Atualizar(eleicao);
            return eleicao.ApurarVotos();
        }

        public Eleitor AtualizarEleitor(int eleicaoId, Eleitor eleitor)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarUsuario(eleitor.Email);
            if (usuario == null)
                usuario = new Usuario(eleitor.Email, eleitor.Nome, eleitor.Cargo);
            eleitor.Usuario = usuario;

            var eleitorAtualizado = eleicao.AtualizarEleitor(eleitor);
            base.Atualizar(eleicao);
            return eleitorAtualizado;
        }

        public Eleitor BuscarEleitor(int eleicaoId, int eleitorId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var eleitor = eleicao.BuscarEleitor(eleitorId);
            if (eleitor == null) throw new NotFoundException("Eleitor não encontrado.");
            return eleitor;
        }

        public IEnumerable<EtapaCronograma> AtualizarCronograma(int eleicaoId, EtapaCronograma etapa)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            eleicao.AtualizarCronograma(etapa);
            base.Atualizar(eleicao);
            return eleicao.Cronograma;
        }

        public async Task<Inscricao> AtualizarFotoInscricao(int eleicaoId, int usuarioId, Stream file, string fotoFileName)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var inscricao = eleicao.BuscarEleitorPeloUsuarioId(usuarioId)?.Inscricao;
            if (inscricao == null) throw new NotFoundException("Inscrição não encontrada.");

            var fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}{Path.GetExtension(fotoFileName)}";
            var fileKey = $@"{PATH_FOTOS}{eleicaoId.ToString()}/{fileName}";
            var fotoAnterior = inscricao.Foto;
            inscricao.Foto = await _unitOfWork.ArquivoRepository.RealizarUpload(file, fileKey);
            base.Atualizar(eleicao);

            if (!string.IsNullOrWhiteSpace(fotoAnterior))
            {
                _unitOfWork.ArquivoRepository.ExluirArquivoNuvem(fotoAnterior);
            }
            return inscricao;
        }

        public Stream BuscarFotoInscricao(int eleicaoId, int inscricaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var inscricao = eleicao.BuscarInscricaoPeloId(inscricaoId);
            if (inscricao == null) throw new NotFoundException("Inscrição não encontrada.");

            var file = FileSystemHelpers.GetAbsolutePath(inscricao.Foto);
            if (string.IsNullOrWhiteSpace(inscricao.Foto) || !File.Exists(file))
                throw new NotFoundException("Foto não encontrada.");

            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void ValidaFormatoPlanilha(string file)
        {
            string[] extensoesSuportadas = new string[] { ".xlsx", ".xlsm", ".xltx", ".xltm" };
            if (!extensoesSuportadas.Contains(Path.GetExtension(file)))
            {
                var extensoes = extensoesSuportadas.Aggregate("", (acc, cur) => $"{acc}{cur}, ");
                extensoes = extensoes.Substring(0, extensoes.Length - 2);
                throw new CustomException($"A extensão do arquivo é inválida. Somente as extensões {extensoes} são suportadas.");
            }
        }

        public async Task<Importacao> ImportarEleitores(int eleicaoId, int usuarioId, Stream file, string nomeArquivo, string contentType)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarPeloId(usuarioId);
            if (usuario == null) throw new CustomException("Usuário inválido!");

            var arquivo = await _arquivoAppService.SalvarArquivo(
                file, DependencyFileType.Importacao, eleicao.Id,
                usuario.Email, usuario.Nome, Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(nomeArquivo), contentType);

            ValidaFormatoPlanilha(arquivo.Path);

            var importacao = new Importacao(arquivo, eleicao);
            importacao = _unitOfWork.ImportacaoRepository.Adicionar(importacao);
            _unitOfWork.Commit();
            return importacao;
        }

        public async Task<Arquivo> FazerUploadArquivo(int eleicaoId, int etapaId, int usuarioId, Stream file, string nomeArquivo, string contentType)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            var usuario = _unitOfWork.UsuarioRepository.BuscarPeloId(usuarioId);
            if (usuario == null) throw new CustomException("Usuário inválido!");

            return await _arquivoAppService.SalvarArquivo(
                file, DependencyFileType.DocumentoCronograma, etapaId, usuario.Email, usuario.Nome, nomeArquivo, contentType);
        }

        public void AtualizarCronogramas()
        {
            var eleicoes = (_repositoryBase as IEleicaoRepository).BuscarEleicoesComMudancaEtapaAgendadaParaHoje();
            foreach (var eleicao in eleicoes)
            {
                try
                {
                    eleicao.PassarParaProximaEtapa(true);
                    var formatador = _formatadorFactory
                        .ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(
                            ETipoTemplateEmail.SucessoMudancaEtapaCronograma, eleicao);
                    EnviarNotificacaoMudancaEtapa(formatador, eleicao);
                }
                catch
                {
                    var formatador = _formatadorFactory
                        .ObterFormatadorEmailParaComunicadosGeraisProcessamentoEtapa(
                            ETipoTemplateEmail.ErroMudancaEtapaCronograma, eleicao);
                    EnviarNotificacaoMudancaEtapa(formatador, eleicao);
                }
            }
            _unitOfWork.Commit();
        }

        public void AtualizarDimensionamento(int eleicaoId)
        {
            var eleicao = _unitOfWork.EleicaoRepository.BuscarPeloId(eleicaoId);
            if (eleicao == null) throw new NotFoundException("Eleição não encontrada.");

            LinhaDimensionamento dimensionamentoGrupo = eleicao.Grupo.CalcularDimensionamento(eleicao.Eleitores.Count);
            eleicao.AtualizarDimensionamento(dimensionamentoGrupo);

            base.Atualizar(eleicao);
        }

        private void EnviarNotificacaoMudancaEtapa(IFormatadorEmailService formatador, Eleicao eleicao)
        {
            foreach (var email in formatador.FormatarEmails())
                _unitOfWork.EmailRepository.Adicionar(email);
        }

        public Stream GerarRelatorioInscricoes(int eleicaoId)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Nome");
            dataTable.Columns.Add("Email");
            dataTable.Columns.Add("Matrícula");
            dataTable.Columns.Add("Cargo");
            dataTable.Columns.Add("Área");
            dataTable.Columns.Add("Data de Nascimento");
            dataTable.Columns.Add("Data de Admissão");
            dataTable.Columns.Add("Data de Inscrição");
            dataTable.Columns.Add("Votos");

            var apuracao = ApurarVotos(eleicaoId).AsQueryable().Select(inscricao => new
            {
                inscricao.Eleitor.Nome,
                inscricao.Eleitor.Email,
                inscricao.Eleitor.Matricula,
                inscricao.Eleitor.Cargo,
                inscricao.Eleitor.Area,
                inscricao.Eleitor.DataNascimento,
                inscricao.Eleitor.DataAdmissao,
                inscricao.DataCadastro,
                inscricao.Votos
            }).ToList();

            foreach (var inscricao in apuracao)
            {
                dataTable.Rows.Add(inscricao.Nome, inscricao.Email,
                    inscricao.Matricula, inscricao.Cargo, inscricao.Area,
                    inscricao.DataNascimento, inscricao.DataAdmissao, inscricao.DataCadastro,
                    inscricao.Votos);
            }

            var nomeArquivo = $"Inscricoes - Eleicao {eleicaoId}.xlsx";
            var arquivo = FileSystemHelpers.GetRelativeFileName(FileSystemHelpers.GetAbsolutePath(PATH_RELATORIOS), nomeArquivo);
            _excelService.GravaInformacoesArquivo(arquivo, dataTable, "Inscrições", 1, 1);
            return new FileStream(arquivo, FileMode.Open, FileAccess.Read, FileShare.Read);
        }


        public Stream GerarRelatorioVotos(int eleicaoId)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Nome");
            dataTable.Columns.Add("Email");
            dataTable.Columns.Add("Matrícula");
            dataTable.Columns.Add("Cargo");
            dataTable.Columns.Add("Área");
            dataTable.Columns.Add("Data de Nascimento");
            dataTable.Columns.Add("Data de Admissão");
            dataTable.Columns.Add("Horário do Voto");
            dataTable.Columns.Add("IP");

            var votos = BuscarVotos(eleicaoId).AsQueryable().Select(voto => new
            {
                voto.Eleitor.Nome,
                voto.Eleitor.Email,
                voto.Eleitor.Matricula,
                voto.Eleitor.Cargo,
                voto.Eleitor.Area,
                voto.Eleitor.DataNascimento,
                voto.Eleitor.DataAdmissao,
                voto.DataCadastro,
                voto.IP
            }).OrderBy(voto => voto.Nome).ToList();

            foreach (var voto in votos)
            {
                dataTable.Rows.Add(voto.Nome, voto.Email,
                    voto.Matricula, voto.Cargo, voto.Area,
                    voto.DataNascimento, voto.DataAdmissao,
                    voto.DataCadastro, voto.IP);
            }

            var nomeArquivo = $"Votos - Eleicao {eleicaoId}.xlsx";
            var arquivo = FileSystemHelpers.GetRelativeFileName(FileSystemHelpers.GetAbsolutePath(PATH_RELATORIOS), nomeArquivo);
            _excelService.GravaInformacoesArquivo(arquivo, dataTable, "Votos", 1, 1);
            return new FileStream(arquivo, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

    }
}