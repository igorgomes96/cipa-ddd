using Cipa.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cipa.WebApi.BackgroundTasks.Scheduler
{
    public class AlteracaoEtapaScheduler : ScheduledProcessor
    {
        public AlteracaoEtapaScheduler(IServiceProvider serviceProvider, ILogger<AlteracaoEtapaScheduler> logger): base(serviceProvider, logger) { } 
        protected override string Schedule => "02 18 * * * ";  // Diariamente à 1:00

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IEleicaoAppService>();
                scopedProcessingService.AtualizarCronogramas();
            }
            return Task.CompletedTask;
        }
    }
}
