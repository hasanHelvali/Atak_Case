using Atak_API.Models;

namespace Atak_API.Abstraction
{
    public interface IBestSellerService 
    {
        Task<List<Product>> GetBestSellers(string userId);
    }
}
