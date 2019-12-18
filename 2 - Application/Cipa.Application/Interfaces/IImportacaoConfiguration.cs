
using Cipa.Application.Services.Models;

namespace Cipa.Application.Interfaces
{
    public interface IImportacaoServiceConfiguration
    {
        DataColumnValidator[] Validators { get; }
    }
}
