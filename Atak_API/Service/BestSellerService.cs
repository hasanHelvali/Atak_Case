using Atak_API.Abstraction;
using Atak_API.Context;
using Atak_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Atak_API.Service
{
    public class BestSellerService : IBestSellerService
    {
        private readonly APIDbContext _context;

        public BestSellerService(APIDbContext context)
        {
            _context = context;//context inject edildi.
        }


        public async Task<List<Product>> GetBestSellers(string userId)
        {
            // Her iki durumda da genel en iyi satıcıları döndüreceğiz.
            var bestSellers = await _context.ProductViews
                .Where(pv => pv.UserId == userId) // Belirli bir kullanıcıya ait verileri filtreliyoruz
                .GroupBy(pv => pv.ProductId)          // Ürünlere göre gruplama
                .OrderByDescending(g => g.Count())    // En çok görüntülenenleri sıralama
                .Take(10)                             // İlk 10 ürünü alıyoruz
                .Select(g => g.First().Product)       // Her gruptan ilk ürünü alıyoruz
                .ToListAsync();//Execute islemi ve sonucları koleksiyon olarak elde ediyoruz.
            return bestSellers;
        }
    }
}


