using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.Data;
using System.Text.Json;

namespace Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderController> _logger;

        public OrderController(AppDbContext db, IPublishEndpoint publishEndpoint, ILogger<OrderController> logger)
        {
            _db = db;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido(CreateOrderRequest request)
        {

            var pedido = new Orders
            {
                Id = Guid.NewGuid(),
                Cliente = request.Cliente,
                Valor = request.Valor,
                Produto = request.Produto,
                DataCriacao = DateTime.UtcNow,
                Status = "Aguardando Processamento"
            };

            // CRIA O EVENTO
            var evento = new CreatedOrderEvent
            {
                PedidoId = pedido.Id,
                Cliente = pedido.Cliente,
                Valor = pedido.Valor,
                Produto = pedido.Produto,
                DataCriacao = pedido.DataCriacao
            };

            // CRIA A MENSAGEM DA OUTBOX
            var outboxMessage = new Outbox
            {
                Id = Guid.NewGuid(),
                Type = typeof(CreatedOrderEvent).AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(evento),
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = null,
                RetryCount = 0
            };

            // TRANSAÇÃO ÚNICA - TUDO OU NADA!
            await using var transaction = await _db.Database.BeginTransactionAsync();

            await _db.Pedidos.AddAsync(pedido);
            await _db.OutboxMessage.AddAsync(outboxMessage);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("📦 Pedido {PedidoId} criado e na Outbox", pedido.Id);

            return Ok(new
            {
                pedido.Id,
                Status = pedido.Status,
                Mensagem = "Pedido criado e será processado em breve"
            });
        }



        [HttpGet]
        public async Task<IActionResult> ListarPedidos()
        {
            var pedidos = await _db.Pedidos
             .OrderByDescending(p => p.DataCriacao)
             .ToListAsync();
            return Ok(pedidos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPedido(Guid id)
        {
            var pedido = await _db.Pedidos.FindAsync(id);
            return pedido is null ? NotFound() : Ok(pedido);
        }


        public record CreateOrderRequest
        {
            public string Cliente { get; init; }
            public decimal Valor { get; init; }
            public string Produto { get; init; }
        }

    }
}
