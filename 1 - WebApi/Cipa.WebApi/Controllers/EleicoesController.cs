using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.WebApi.Authentication;
using Cipa.WebApi.Filters;
using Cipa.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cipa.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EleicoesController : Controller
    {
        #region Serviços e Construtor
        private readonly IEleicaoAppService _eleicaoAppService;
        private readonly IArquivoAppService _arquivoAppService;
        private readonly IImportacaoAppService _importacaoAppService;
        private readonly IMapper _mapper;
        public EleicoesController(
            IEleicaoAppService eleicaoAppService,
            IMapper mapper,
            IArquivoAppService arquivoAppService,
            IImportacaoAppService importacaoAppService)
        {
            _eleicaoAppService = eleicaoAppService;
            _mapper = mapper;
            _arquivoAppService = arquivoAppService;
            _importacaoAppService = importacaoAppService;
        }
        #endregion

        #region Eleições
        [HttpGet]
        [Pagination]
        public IEnumerable<EleicaoDetalheViewModel> GetEleicoes()
        {
            IEnumerable<Eleicao> eleicoes;
            if (User.IsInRole(PerfilUsuario.SESMT))
                eleicoes = _eleicaoAppService.BuscarPelaConta(ContaId);
            else
                eleicoes = _eleicaoAppService.BuscarPeloUsuario(UsuarioId);
            return _mapper.Map<List<EleicaoDetalheViewModel>>(eleicoes);
        }

        [HttpGet("{id}/usuarioeleitor")]
        public ActionResult<bool> GetUsuarioEhEleitor(int id)
        {
            return _eleicaoAppService.VerificarSeUsuarioEhEleitor(id, UsuarioId);
        }

        [HttpGet("{id}")]
        public ActionResult<EleicaoDetalheViewModel> GetEleicao(int id)
        {
            var eleicao = _eleicaoAppService.BuscarPeloId(id);
            if (eleicao == null)
                return NotFound("Eleição não encontrada.");
            else
                return _mapper.Map<EleicaoDetalheViewModel>(eleicao);
        }

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPost]
        public ActionResult<EleicaoViewModel> PostEleicao(EleicaoViewModel eleicao)
        {
            eleicao.ContaId = ContaId;
            eleicao.UsuarioCriacaoId = UsuarioId;
            var eleicaoModel = _mapper.Map<Eleicao>(eleicao);
            return _mapper.Map<EleicaoViewModel>(_eleicaoAppService.Adicionar(eleicaoModel));
        }

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPut("{id}")]
        public IActionResult PutEleicao(EleicaoViewModel eleicao, int id)
        {
            eleicao.Id = id;
            _eleicaoAppService.Atualizar(_mapper.Map<Eleicao>(eleicao));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public EleicaoViewModel DeleteEleicao(int id)
        {
            return _mapper.Map<EleicaoViewModel>(_eleicaoAppService.Excluir(id));
        }

        [HttpPost("{id}/atualizardimensionamento")]
        public void PostAtualizarDimensionamento(int id)
        {
            _eleicaoAppService.AtualizarDimensionamento(id);
        }
        #endregion

        #region Cronograma
        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpGet("{id}/cronograma")]
        public IEnumerable<EtapaCronogramaViewModel> GetCronograma(int id) =>
            _mapper.Map<List<EtapaCronogramaViewModel>>(_eleicaoAppService.BuscarCronograma(id));

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPost("{id}/proximaetapa")]
        public IEnumerable<EtapaCronogramaViewModel> PassarParaProximaEtapa(int id) =>
             _mapper.Map<List<EtapaCronogramaViewModel>>(_eleicaoAppService.PassarParaProximaEtapa(id)?.Cronograma);

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPut("{id}/cronograma")]
        public IEnumerable<EtapaCronogramaViewModel> AtualizarCronograma(int id, EtapaCronogramaViewModel etapaCronograma) =>
             _mapper.Map<List<EtapaCronogramaViewModel>>(_eleicaoAppService.AtualizarCronograma(id, _mapper.Map<EtapaCronograma>(etapaCronograma)));

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPost("{id}/cronograma/{etapaId}/arquivos"), DisableRequestSizeLimit]
        public ActionResult UploadArquivos(int id, int etapaId)
        {
            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                return BadRequest("Nenhum arquivo foi enviado.");
            foreach (var formFile in Request.Form.Files)
            {
                var fileName = formFile.FileName;
                byte[] arquivo = null;
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    arquivo = ms.ToArray();
                }
                _eleicaoAppService.FazerUploadArquivo(id, etapaId, UsuarioId, arquivo, fileName, formFile.ContentType);
            }
            return Ok();
        }

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpGet("{id}/cronograma/{etapaId}/arquivos")]
        public IEnumerable<ArquivoViewModel> BuscarArquivos(int id, int etapaId) =>
            _mapper.Map<List<ArquivoViewModel>>(_arquivoAppService.BuscaArquivos(DependencyFileType.DocumentoCronograma, etapaId));
        #endregion

        #region Eleitores
        [HttpGet("{id}/eleitores")]
        [Query("Nome", "Cargo")]
        [Pagination]
        public IEnumerable<EleitorViewModel> GetEleitores(int id) =>
            _mapper.Map<List<EleitorViewModel>>(_eleicaoAppService.BuscarEleitores(id));

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPost("{id}/eleitores")]
        public EleitorViewModel PostEleitor(int id, EleitorViewModel eleitorViewModel)
        {
            var eleitor = _mapper.Map<Eleitor>(eleitorViewModel);
            return _mapper.Map<EleitorViewModel>(_eleicaoAppService.AdicionarEleitor(id, eleitor));
        }

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpDelete("{eleicaoId}/eleitores")]
        public void DeleteEleitor(int eleicaoId) => _eleicaoAppService.ExcluirTodosEleitores(eleicaoId);


        [HttpDelete("{eleicaoId}/eleitores/{eleitorId}")]
        public EleitorViewModel DeleteEleitor(int eleicaoId, int eleitorId)
        {
            return _mapper.Map<EleitorViewModel>(_eleicaoAppService.ExcluirEleitor(eleicaoId, eleitorId));
        }

        [HttpGet("{id}/eleitores/{eleitorId}")]
        public EleitorViewModel GetEleitor(int id, int eleitorId)
        {
            return _mapper.Map<EleitorViewModel>(_eleicaoAppService.BuscarEleitor(id, eleitorId));
        }

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPut("{eleicaoId}/eleitores/{eleitorId}")]
        public EleitorViewModel PutEleitor(int eleicaoId, int eleitorId, EleitorViewModel eleitor)
        {
            eleitor.Id = eleitorId;
            return _mapper.Map<EleitorViewModel>(_eleicaoAppService.AtualizarEleitor(eleicaoId, _mapper.Map<Eleitor>(eleitor)));
        }

        [HttpGet("{id}/eleitor")]
        public ActionResult<EleitorViewModel> GetEleitorLogado(int id)
        {
            return _mapper.Map<EleitorViewModel>(_eleicaoAppService.BuscarEleitorPeloIdUsuario(id, UsuarioId));
        }
        #endregion

        #region Inscrições
        [Query("Eleitor.Nome")]
        [HttpGet("{id}/inscricoes")]
        public ActionResult<IEnumerable<InscricaoViewModel>> GetInscricoes(int id, string status, int? seedOrder = null)
        {
            StatusInscricao statusInscricao = StatusInscricao.Pendente;
            try
            {
                statusInscricao = (StatusInscricao)Enum.Parse(typeof(StatusInscricao), status);
            }
            catch
            {
                return BadRequest($"Status inválido: {status}");
            }
            var inscricoes = _eleicaoAppService.BuscarInscricoes(id, statusInscricao).AsQueryable();
            if (seedOrder.HasValue)
            {
                Random rnd = new Random(seedOrder.Value);
                return Ok(_mapper.Map<List<InscricaoViewModel>>(inscricoes.OrderBy(_ => rnd.Next())));
            }
            return Ok(_mapper.Map<List<InscricaoViewModel>>(inscricoes.OrderBy(i => i.Eleitor.Nome)));
        }

        [HttpPost("{id}/inscricoes")]
        public InscricaoViewModel PostInscricao(int id, InscricaoViewModel inscricao)
        {
            return _mapper.Map<InscricaoViewModel>(_eleicaoAppService.FazerInscricao(id, UsuarioId, inscricao.Objetivos));
        }

        [HttpPut("{id}/inscricoes")]
        public InscricaoViewModel PutInscricao(int id, InscricaoViewModel inscricao)
        {
            return _mapper.Map<InscricaoViewModel>(_eleicaoAppService.AtualizarInscricao(id, UsuarioId, inscricao.Objetivos));
        }

        [HttpPost("{id}/inscricoes/foto"), DisableRequestSizeLimit]
        public ActionResult<InscricaoViewModel> PosttAtualizaFotoInscricao(int id)
        {
            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                return BadRequest("Nenhuma foto foi enviada.");

            if (Request.Form.Files.Count > 1)
                return BadRequest("Somente uma foto pode ser enviada.");

            var formFile = Request.Form.Files.First();
            var fileName = formFile.FileName;
            byte[] foto = null;
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                foto = ms.ToArray();
            }
            return _mapper.Map<InscricaoViewModel>(_eleicaoAppService.AtualizarFotoInscricao(id, UsuarioId, foto, fileName));
        }

        [HttpGet("{id}/inscricoes/{inscricaoId}/foto")]
        public IActionResult GetFotoInscricacao(int id, int inscricaoId)
        {
            return new FileStreamResult(_eleicaoAppService.BuscarFotoInscricao(id, inscricaoId), "image/jpeg");
        }

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPut("{id}/inscricoes/{inscricaoId}/aprovar")]
        public InscricaoViewModel PutAprovarInscricao(int id, int inscricaoId)
        {
            return _mapper.Map<InscricaoViewModel>(_eleicaoAppService.AprovarInscricao(id, inscricaoId, UsuarioId));
        }

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPut("{id}/inscricoes/{inscricaoId}/reprovar")]
        public InscricaoDetalhesViewModel PutReprovarInscricao(int id, int inscricaoId, ReprovacaoViewModel reprovacao)
        {
            return _mapper.Map<InscricaoDetalhesViewModel>(_eleicaoAppService.ReprovarInscricao(id, inscricaoId, UsuarioId, reprovacao.MotivoReprovacao));
        }

        [HttpGet("{id}/inscricao")]
        public ActionResult<InscricaoDetalhesViewModel> GetInscricaoUsuario(int id)
        {
            return _mapper.Map<InscricaoDetalhesViewModel>(_eleicaoAppService.BuscarInscricaoPeloUsuario(id, UsuarioId));
        }
        #endregion

        #region Votação
        [HttpPost("{id}/inscricoes/{inscricaoId}/votar")]
        public VotoViewModel PostVotar(int id, int inscricaoId)
        {
            return _mapper.Map<VotoViewModel>(_eleicaoAppService.RegistrarVoto(id, inscricaoId, UsuarioId, IpRequisicao));
        }

        [HttpPost("{id}/votarbranco")]
        public VotoViewModel PostVotarEmBranco(int id)
        {
            return _mapper.Map<VotoViewModel>(_eleicaoAppService.VotarEmBranco(id, UsuarioId, IpRequisicao));
        }

        [HttpGet("{id}/votos")]
        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [Pagination]
        public IEnumerable<VotoViewModel> GetVotos(int id) =>
            _eleicaoAppService.BuscarVotos(id).AsQueryable().ProjectTo<VotoViewModel>(_mapper.ConfigurationProvider);

        [HttpGet("{id}/votousuario")]
        public VotoViewModel GetVotoUsuario(int id)
        {
            return _mapper.Map<VotoViewModel>(_eleicaoAppService.BuscarVotoUsuario(id, UsuarioId));
        }

        [HttpGet("{id}/apuracao")]
        public IEnumerable<ApuracaoViewModel> GetApuracao(int id) =>
            _mapper.Map<List<ApuracaoViewModel>>(_eleicaoAppService.ApurarVotos(id));

        [HttpPost("{id}/registrarresultado")]
        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        public ResultadoApuracaoViewModel RegistrarResultadoApuracao(int id) =>
            _mapper.Map<ResultadoApuracaoViewModel>(_eleicaoAppService.RegistrarResultadoApuracao(id));

        [HttpGet("{id}/resultado")]
        public ResultadoApuracaoViewModel GetResultadoApuracao(int id) =>
            _mapper.Map<ResultadoApuracaoViewModel>(_eleicaoAppService.ApurarVotos(id));
        #endregion

        #region Importação
        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpGet("{id}/importacoes/ultima")]
        public ImportacaoViewModel GetUltimaImportacao(int id) =>
            _mapper.Map<ImportacaoViewModel>(_importacaoAppService.RetornarUltimaImportacaoDaEleicao(id));

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpGet("{id}/importacoes/{idImportacao}/inconsistencias")]
        public IEnumerable<InconsistenciaViewModel> GetInconsistenciasImportacao(int id, int idImportacao) =>
            _mapper.Map<List<InconsistenciaViewModel>>(_importacaoAppService.RetornarInconsistenciasDaImportacao(idImportacao));

        [Authorize(PoliticasAutorizacao.UsuarioSESMTContaValida)]
        [HttpPost("{id}/importacoes"), DisableRequestSizeLimit]
        public ActionResult<ImportacaoViewModel> ImportarFuncionarios(int id)
        {
            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                return BadRequest("Nenhum arquivo foi enviado.");

            if (Request.Form.Files.Count > 1)
                return BadRequest("Somente um arquivo pode ser enviado.");

            var formFile = Request.Form.Files.First();
            var fileName = formFile.FileName;
            byte[] arquivo = null;
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                arquivo = ms.ToArray();
            }
            return _mapper.Map<ImportacaoViewModel>(_eleicaoAppService
                .ImportarEleitores(id, UsuarioId, arquivo, fileName, formFile.ContentType));
        }
        #endregion

        #region Relatórios
        [HttpGet("{id}/relatorios/inscricoes")]
        public IActionResult GetRelatorioInscricoes(int id) =>
            new FileStreamResult(_eleicaoAppService.GerarRelatorioInscricoes(id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        [HttpGet("{id}/relatorios/votos")]
        public IActionResult GetRelatorioVotos(int id) =>
            new FileStreamResult(_eleicaoAppService.GerarRelatorioVotos(id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        #endregion

    }
}