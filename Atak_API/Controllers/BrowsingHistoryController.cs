using Atak_API.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Atak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrowsingHistoryController : ControllerBase
    {
         private readonly IBrowsingHistoryService _browsingHistoryService;
        public BrowsingHistoryController(IBrowsingHistoryService browsingHistoryService)
        {
            _browsingHistoryService = browsingHistoryService;
        }
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetUserBrowsingHistory(string userId)
        {
            if (string.IsNullOrEmpty(userId))//id yoka
                return BadRequest(new { message = "UserId değeri boş!" });

            var response = await _browsingHistoryService.GetUserBrowsingHistory(userId);//Ilgili userId degerine karsılık kayıtlar alındı.
            return Ok(response);//cevap donuldu
        }

        [HttpDelete("history/{userId}/{productId}")]
        public async Task<IActionResult> DeleteProductFromHistory(string userId, string productId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))//Parametreler boş ise
                return BadRequest(new { message = "UserId ve ProductId degerleri boş!" });

            var result = await _browsingHistoryService.DeleteProductFromHistory(userId, productId);//Ilgili parametrelere karsılık işlemler yapıldı.

            if (!result)//Sonuc false ise
                return NotFound(new { message = "Ürün Görüntülenmesi Bulunamadı!" });

            //return NoContent();//İşlem başarılı ise 204 don.
            return Ok("Urün Görüntülenme Geçmişinden Silindi");//İşlem başarılı ise 200 don.
        }
    }
}
