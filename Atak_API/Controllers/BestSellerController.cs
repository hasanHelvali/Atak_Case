using Atak_API.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Atak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestSellerController : ControllerBase
    {

        /*
             Genel Best Seller'lar: Kategori bilgisi olmadan sadece ProductView tablosuna dayanarak ürünlerin sıklığını değerlendiriyoruz.
            Kişiselleştirilmiş Öneriler: Kullanıcı bazında kategorilere göre öneri sunmak mümkün olmadığından, kişiselleştirilmiş öneri özelliği kullanılamaz.
            Bu çözüm, mevcut veri yapısı ve sağlanan veri türüne uygun olarak en iyi satıcıları listelemenizi sağlar. 
            Kategori bilgisi elde edilemediği için kişiselleştirilmiş önerilerde daha fazla esneklik sağlayamıyoruz.
        */


        private readonly IBestSellerService _bestSellerService;

        public BestSellerController(IBestSellerService bestSellerService)
        {
            _bestSellerService = bestSellerService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBestSellers(string userId)
        {
            var products = await _bestSellerService.GetBestSellers(userId);//Service'den ilgili userId degerine gore kayıtlar alınır

            if (products.Count == 0)
            {
                return Ok(new { userId, products = new List<string>(), type = "non-personalized" });
                //Kayıt 0 ise bos sonuc donulur.
            }

            var response = new//anonimous tip
            {
                userId,
                products = products.Select(p => p.ProductId).ToList(),
                type = string.IsNullOrEmpty(userId) ? "non-personalized" : "personalized"
                //Kayıt varsa ProductId lerin bulundugu liste donulur.
            };

            return Ok(response);



        }
    }
}
