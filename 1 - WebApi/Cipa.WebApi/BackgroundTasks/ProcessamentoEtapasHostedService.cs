using Cipa.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cipa.WebApi.BackgroundTasks
{
    public class ProcesssamentoEtapasHostedService : ScopedProcessor
    {
        public ProcesssamentoEtapasHostedService(IServiceProvider serviceScopeFactory, ILogger<ProcesssamentoEtapasHostedService> logger) : base(serviceScopeFactory, logger) { }

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IProcessamentoEtapaAppService>();
                scopedProcessingService.ProcessarEtapasPendentes();
            }
            return Task.CompletedTask;
        }

        protected override int Delay { get { return (int)TimeSpan.FromMinutes(30).TotalMilliseconds; } }
    }
}
