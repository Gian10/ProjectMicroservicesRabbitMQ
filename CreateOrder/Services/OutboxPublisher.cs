
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Data;
using System.Text.Json;

namespace Order.Services
{
    public class OutboxPublisher : BackgroundService
    {

        private readonly IServiceProvider _services;
        private readonly ILogger<OutboxPublisher> _logger;

        public OutboxPublisher(IServiceProvider services, ILogger<OutboxPublisher> logger)
        {
            _services = services;
            _logger = logger;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessages(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no OutboxPublisher");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }


        private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var messages = await db.OutboxMessage
                .Where(m => m.ProcessedAt == null && m.RetryCount < 3)
                .OrderBy(m => m.CreatedAt)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                try
                {
                    var eventType = Type.GetType(message.Type);
                    if (eventType == null)
                    {
                        message.Error = "Tipo não encontrado";
                        message.ProcessedAt = DateTime.UtcNow;
                        continue;
                    }

                    var evento = JsonSerializer.Deserialize(message.Content, eventType);
                    await publishEndpoint.Publish(evento, eventType, stoppingToken);

                    message.ProcessedAt = DateTime.UtcNow;
                    message.Error = null;

                    _logger.LogInformation("✅ Mensagem {MessageId} publicada", message.Id);
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;
                    _logger.LogError(ex, "❌ Falha ao publicar mensagem {MessageId}", message.Id);
                }             
            }
        }
    }
}
