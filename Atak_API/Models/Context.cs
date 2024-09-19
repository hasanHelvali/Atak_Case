namespace Atak_API.Models
{
    public class Context
    {
        public int? Id { get; set; }
        public string? Source { get; set; }

        // Navigation properties
        public ICollection<ProductView>? ProductViews { get; set; }
    }
}
