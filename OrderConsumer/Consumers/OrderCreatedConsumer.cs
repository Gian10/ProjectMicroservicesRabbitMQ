using Contracts;
using MassTransit;
using OrderConsumer.Data;

namespace OrderConsumer.Consumers
{
    public class OrderCreatedConsumer: IConsumer<CreatedOrderEvent>
    {
        private readonly ProcessadorDbContext _db;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ProcessadorDbContext db, ILogger<OrderCreatedConsumer> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreatedOrderEvent> context)
        {
            var evento = context.Message;

            _logger.LogInformation("📥 PROCESSANDO pedido {PedidoId} para {Cliente}",
           evento.PedidoId, evento.Cliente);

            try
            {
                // SIMULA PROCESSAMENTO (2 segundos)
                await Task.Delay(2000);

                var log = new LogProcessamento
                {
                    Id = Guid.NewGuid(),
                    PedidoId = evento.PedidoId,
                    Cliente = evento.Cliente,
                    Valor = evento.Valor,
                    Produto = evento.Produto,
                    ProcessadoEm = DateTime.UtcNow,
                    Sucesso = true,
                    Observacao = $"Processado com sucesso em {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
                };

                await _db.LogsProcessamento.AddAsync(log);
                await _db.SaveChangesAsync();

                _logger.LogInformation("✅ Pedido {PedidoId} processado com sucesso!",
                evento.PedidoId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao processar pedido {PedidoId}",
               evento.PedidoId);

                // Log do erro
                var log = new LogProcessamento
                {
                    Id = Guid.NewGuid(),
                    PedidoId = evento.PedidoId,
                    Cliente = evento.Cliente,
                    Valor = evento.Valor,
                    Produto = evento.Produto,
                    ProcessadoEm = DateTime.UtcNow,
                    Sucesso = false,
                    Observacao = $"Erro: {ex.Message}"
                };

                await _db.LogsProcessamento.AddAsync(log);
                await _db.SaveChangesAsync();

                throw;
            }

        }
    }
}
