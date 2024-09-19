using Atak_API.DTOs;
using Atak_API.Models;

namespace Atak_API.Abstraction
{
    public interface IBrowsingHistoryService
    {
        Task<BrowsingHistoryResponse> GetUserBrowsingHistory(string userId);
        Task<bool> DeleteProductFromHistory(string userId, string productId);
    }
}
