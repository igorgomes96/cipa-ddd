using Cipa.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cipa.WebApi.BackgroundTasks
{
    public class ImportacaoHostedService : ScopedProcessor
    {
        public ImportacaoHostedService(IServiceProvider serviceScopeFactory, ILogger<ImportacaoHostedService> logger) : base(serviceScopeFactory, logger) { }

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IImportacaoAppService>();
                scopedProcessingService.RealizarImportacaoEmBrackground();
            }
            return Task.CompletedTask;
        }

        protected override int Delay { get { return (int)TimeSpan.FromSeconds(10).TotalMilliseconds; } }
    }
}
