namespace OrderConsumer.Data
{
    public class LogProcessamento
    {
        public Guid Id { get; set; }
        public Guid PedidoId { get; set; }
        public DateTime ProcessadoEm { get; set; }
        public bool Sucesso { get; set; }
        public string? Observacao { get; set; }
        public string? Cliente { get; set; }
        public decimal Valor { get; set; }
        public string? Produto { get; set; }
    }
}
