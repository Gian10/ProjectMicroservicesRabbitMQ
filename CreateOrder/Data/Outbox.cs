namespace Order.Data
{
    public class Outbox
    {

        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;      // Nome do tipo da mensagem
        public string Content { get; set; } = string.Empty;   // JSON da mensagem
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }            // Quando foi publicada
        public int RetryCount { get; set; }                   // Tentativas de publicação
        public string? Error { get; set; }                    // Último erro
    }
}
