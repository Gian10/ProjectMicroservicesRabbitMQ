namespace Contracts
{
    public record class CreatedOrderEvent
    {

        public Guid PedidoId { get; init; }
        public string Cliente { get; init; }
        public decimal Valor { get; init; }
        public DateTime DataCriacao { get; init; }
        public string Produto { get; init; }

    }
}
