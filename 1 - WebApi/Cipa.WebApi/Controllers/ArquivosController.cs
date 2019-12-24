using System.IO;
using System.Reflection;
using Cipa.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cipa.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class ArquivosController : Controller
    {
        private readonly IArquivoAppService _arquivoAppService;
        public ArquivosController(IArquivoAppService arquivoAppService)
        {
            _arquivoAppService = arquivoAppService;
        }

        [HttpDelete("{id}")]
        public ActionResult ExcluirArquivo(int id)
        {
            _arquivoAppService.Excluir(id);
            return Ok();
        }

        [HttpGet("{id}/download")]
        public ActionResult DownloadArquivo(int id)
        {
            var arquivo = _arquivoAppService.BuscarPeloId(id);
            if (string.IsNullOrWhiteSpace(arquivo.Path))
                return BadRequest("Arquivo não encontrado");
            return PhysicalFile(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), arquivo.Path), arquivo.ContentType);
        }

        [HttpGet("templateimportacao")]
        public ActionResult DonwloadTemplateImportacao()
        {
            var arquivo = @"Assets/Template Importacao.xlsx";
            return PhysicalFile(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), arquivo), @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(arquivo));
        }

    }
}