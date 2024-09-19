namespace Atak_API.Models
{
    public class User
    {
        public string? UserId { get; set; }

        // Navigation properties
        public ICollection<ProductView>? ProductViews { get; set; }
    }
}
