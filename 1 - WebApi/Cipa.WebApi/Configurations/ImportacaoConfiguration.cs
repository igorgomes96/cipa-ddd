using Cipa.Application.Interfaces;
using Cipa.Application.Services.Models;

namespace Cipa.WebApi.Configurations
{
    public class ImportacaoServiceConfiguration: IImportacaoServiceConfiguration
    {
        public DataColumnValidator[] Validators { get; set; }
    }
}
