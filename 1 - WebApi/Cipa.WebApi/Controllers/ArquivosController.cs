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

        [HttpGet("templateimportacao")]
        public ActionResult DonwloadTemplateImportacao()
        {
            var arquivo = @"Assets/Template Importacao.xlsx";
            return PhysicalFile(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), arquivo), @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(arquivo));
        }


        [HttpGet("nr5")]
        public ActionResult DonwloadNR5()
        {
            var arquivo = @"Assets/NR5.pdf";
            return PhysicalFile(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), arquivo), @"application/pdf", Path.GetFileName(arquivo));
        }

    }
}