using Cipa.Domain.Entities;

namespace Cipa.Application.Interfaces
{
    public interface IEmailAppService : IAppServiceBase<Email>
    {
        void EnviarEmailsBackground();
    }
}