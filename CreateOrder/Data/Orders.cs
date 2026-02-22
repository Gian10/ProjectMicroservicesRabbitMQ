namespace Order.Data
{
    public class Orders
    {
        public Guid Id { get; set; }
        public string Cliente { get; set; }
        public decimal Valor { get; set; }
        public string Produto { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Status { get; set; } // "Aguardando Processamento", "Processado"
    }
}
