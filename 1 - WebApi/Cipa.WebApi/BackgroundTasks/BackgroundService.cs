using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cipa.WebApi.BackgroundTasks
{
    public abstract class BackgroundService : IHostedService
    {
        private Task _executingTask;
        protected readonly ILogger<BackgroundService> _logger;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public BackgroundService(ILogger<BackgroundService> logger)
        {
            _logger = logger;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Tarefa em backgound iniciada: {this.GetType().Name}.");

            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Tarefa em backgound sendo finalizada: {this.GetType().Name}.");

            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                try
                {
                    _logger.LogInformation($"Tarefa em backgound sendo processada: {this.GetType().Name}.");
                    await Process();
                    _logger.LogInformation($"Tarefa executada com sucesso: {this.GetType().Name}.");
                    await Task.Delay(Delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar tarefa em backgound.");
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        protected abstract int Delay { get; }
        protected abstract Task Process();
    }
}
