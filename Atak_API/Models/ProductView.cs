using System.ComponentModel.DataAnnotations;

namespace Atak_API.Models
{
    public class ProductView
    {
        public int? Id { get; set; }
        public string? Event { get; set; }
        public string? MessageId { get; set; }
        public DateTime? Timestamp { get; set; }

        // Foreign keys
        public string? ProductId { get; set; }
        public Product? Product { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        public int? ContextId { get; set; }
        public Context? Context { get; set; }
    }
}

