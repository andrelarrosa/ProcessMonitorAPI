using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Dto;
using WebApplication1.Hubs;

namespace WebApplication1.Services;

public class ProcessBackgroundService : BackgroundService
{
    private readonly IHubContext<ProcessHub> _hubContext;
    private readonly ILogger<ProcessBackgroundService> _logger;

    public ProcessBackgroundService(IHubContext<ProcessHub> hubContext, ILogger<ProcessBackgroundService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 ProcessBroadcastService iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Process[] processes = Process.GetProcesses();

                List<Dictionary<string, string>> namesAndIds = processes
                    .Select(p => new Dictionary<string, string>
                    {
                        { "name", p.ProcessName },
                        { "id", p.Id.ToString() }
                    })
                    .ToList();

                var data = new ProcessDataDto()
                {
                    NamesAndIds = namesAndIds
                };

                await _hubContext.Clients.All.SendAsync("process:update", data, cancellationToken: stoppingToken);

                _logger.LogInformation("📤 Processos enviados para os clientes.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter e enviar processos.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // envia a cada 5 segundos
        }

        _logger.LogInformation("🛑 ProcessBroadcastService finalizado.");
    }
}