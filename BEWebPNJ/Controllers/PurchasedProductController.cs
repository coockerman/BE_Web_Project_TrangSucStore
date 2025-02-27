using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BEWebPNJ.Services;

namespace BEWebPNJ.Controllers
{
    [Route("api/users/{userId}/purchased")]
    [ApiController]
    public class PurchasedController : ControllerBase
    {
        private readonly PurchasedService _purchasedService;

        public PurchasedController(PurchasedService purchasedService)
        {
            _purchasedService = purchasedService;
        }

        // ✅ [GET] Lấy danh sách ID sản phẩm đã mua của user
        [HttpGet]
        public async Task<ActionResult<Dictionary<string, string>>> GetPurchasedProducts(string userId)
        {
            var purchasedProducts = await _purchasedService.GetPurchasedProductsAsync(userId);
            return Ok(purchasedProducts);
        }

        // ✅ [POST] Thêm sản phẩm vào danh sách đã mua
        [HttpPost("add/{productId}")]
        public async Task<IActionResult> AddPurchasedProduct(string userId, string productId)
        {
            var result = await _purchasedService.AddPurchasedProductAsync(userId, productId);
            return result ? Ok(new { message = "Sản phẩm đã được thêm vào danh sách purchased." })
                          : StatusCode(500, new { message = "Lỗi khi thêm sản phẩm." });
        }

        // ✅ [DELETE] Xóa sản phẩm khỏi danh sách đã mua
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemovePurchasedProduct(string userId, string productId)
        {
            var result = await _purchasedService.RemovePurchasedProductAsync(userId, productId);
            return result ? Ok(new { message = "Sản phẩm đã được xóa khỏi danh sách purchased." })
                          : NotFound(new { message = "Không tìm thấy sản phẩm trong danh sách purchased." });
        }
    }
}
