using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cipa.WebApi.BackgroundTasks
{
    public abstract class ScopedProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        protected ScopedProcessor(IServiceProvider serviceScopeFactory, ILogger<ScopedProcessor> logger) : base(logger)
        {
            _serviceProvider = serviceScopeFactory;
        }

        protected override int Delay { get; }

        protected override async Task Process()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    await ProcessInScope(scope.ServiceProvider);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao processar Background Tasks.");
            }
        }

        public abstract Task ProcessInScope(IServiceProvider serviceProvider);
    }
}
