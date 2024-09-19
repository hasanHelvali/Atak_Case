using Atak_API.Abstraction;
using Atak_API.Context;
using Atak_API.DTOs;
using Atak_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atak_API.Service
{
    public class BrowsingHistoryService : IBrowsingHistoryService
    {
        private readonly APIDbContext _context;

        public BrowsingHistoryService(APIDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteProductFromHistory(string userId, string productId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId null veya boş olamaz!", nameof(userId));

            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("ProductId null veya boş olamaz!", nameof(productId));

            var productView = await _context.ProductViews
                .Include(pv => pv.User)//Eager loading ile ilişkisel verilere erişim
                .Include(pv => pv.Product)//Eager loading ile ilişkisel verilere erişim
                .FirstOrDefaultAsync(pv => pv.User.UserId == userId && pv.Product.ProductId == productId);//id lerin uyuştugu ilk veriyi getir.

            if (productView == null)//null ise 
                return false;//ilgili ilişkisel kayıt bulunamadı yani silme islemi yapılamadı

            _context.ProductViews.Remove(productView);//varsa sil. Bu silme işlemi ilişkisel verileri de uçuracaktır.
            await _context.SaveChangesAsync();//degisiklikleri kaydet yani execute işlemi

            return true;//kaydın basarıyla silindiğine dair bilgi dön
        }

        [HttpGet("history/{userId}")]
        public async Task<BrowsingHistoryResponse> GetUserBrowsingHistory(string userId)
        {
            if (string.IsNullOrEmpty(userId))//id yoksa
                throw new ArgumentException("UserId null veya boş olamaz.", nameof(userId));//Hata fırlat

            var productViews = await _context.ProductViews
                .Where(pv => pv.User.UserId == userId)//Belirli bir kullanıcıya ait bilgileri filtrele
                .OrderByDescending(pv => pv.Timestamp)//Verileri zamana gore en yeniden en eskiye yani azalan olarak sıralar.
                //En son gorulenleri filtrelemek icindir.

                .Take(10)//İlk 10 kaydı (Desc sıralandıgı icin aslında son bakılan 10 urunu) al
                .Include(pv => pv.Product)//Product bilgisiyle beraber
                .ToListAsync();//execute et

            if (productViews.Count < 5)
            {
                //Herhangi bir öneri listesi için minimum ürün sayısı 5 olarak belirtilmiş.
                // Eğer ürün sayısı 5'ten azsa, API boş bir liste döndürmelidir
                return new BrowsingHistoryResponse
                {
                    UserId = userId,
                    Products = new List<string>(),
                    Type = "personalized"
                };
            }

            var products = productViews
                .Select(pv => pv.Product?.ProductId)
                .Where(pid => !string.IsNullOrEmpty(pid))
                .ToList();//ProductId listesini elde ederiz.

            return new BrowsingHistoryResponse
            {
                UserId = userId,
                Products = products,
                Type = "personalized"  
            };
        }


    }
}
