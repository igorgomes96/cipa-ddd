using Cipa.Domain.Helpers;
using Cronos;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cipa.WebApi.BackgroundTasks.Scheduler
{
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        private readonly CronExpression _schedule;
        private DateTime? _nextRun;
        protected abstract string Schedule { get; }
        protected ScheduledProcessor(IServiceProvider serviceScopeFactory, ILogger<ScheduledProcessor> logger) : base(serviceScopeFactory, logger)
        {
            _schedule = CronExpression.Parse(Schedule);
            TimeZoneInfo brasilia = TimeZoneInfo.FindSystemTimeZoneById(FusosHorarios.Brasilia);
            _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow, brasilia);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                try
                {
                    var now = DateTime.Now.HorarioBrasilia().ToUniversalTime();
                    _logger.LogInformation($"Verificando se a tarefa {this.GetType().Name} deve ser executada agora. Próxima execução: {_nextRun.Value.ToString("dd/MM HH:mm:ss")}; Agora: {now.ToString("dd/MM HH:mm:ss")}");
                    if (now > _nextRun)
                    {
                        _logger.LogInformation($"Tarefa em backgound sendo processada: {this.GetType().Name}.");
                        await Process();
                        _logger.LogInformation($"Tarefa executada com sucesso: {this.GetType().Name}.");
                        _nextRun = _schedule.GetNextOccurrence(DateTime.Now.HorarioBrasilia().ToUniversalTime());
                    }
                    await Task.Delay(Delay, stoppingToken);
                } catch (Exception ex) {
                    _logger.LogError(ex, "Erro ao executar processo agendado.");
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        protected override int Delay { get { return (int)TimeSpan.FromSeconds(10).TotalMilliseconds; } }
    }
}
