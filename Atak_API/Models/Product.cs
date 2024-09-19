namespace Atak_API.Models
{
    public class Product
    {
        //public int? Id { get; set; }
        public string? ProductId { get; set; }

        // Navigation properties
        public ICollection<ProductView>? ProductViews { get; set; }
    }
}
