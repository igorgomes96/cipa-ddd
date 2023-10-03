using System;
using System.Threading.Tasks;
using Cipa.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cipa.WebApi.BackgroundTasks {
    public class AlteracaoEtapaService: ScopedProcessor {

        public AlteracaoEtapaService(IServiceProvider serviceScopeFactory, ILogger<ScopedProcessor> logger) : base(serviceScopeFactory, logger)
        {
        }

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IEleicaoAppService>();
                scopedProcessingService.AtualizarCronogramas();
            }
            return Task.CompletedTask;
        }

        protected override int Delay => (int)TimeSpan.FromMinutes(15).TotalMilliseconds;
    }
}
