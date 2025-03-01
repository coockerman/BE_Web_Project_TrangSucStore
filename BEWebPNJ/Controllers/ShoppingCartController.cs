using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BEWebPNJ.Services;

namespace BEWebPNJ.Controllers
{
    [Route("api/users/{userId}/shopping-cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartController(ShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        // ✅ [GET] Lấy giỏ hàng của user
        [HttpGet]
        public async Task<ActionResult<Dictionary<string, Dictionary<string, object>>>> GetShoppingCart(string userId)
        {
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(userId);
            return Ok(shoppingCart);
        }

        // ✅ [POST] Thêm sản phẩm vào giỏ hàng
        [HttpPost("add/{productId}")]
        public async Task<IActionResult> AddToShoppingCart(string userId, string productId, [FromBody] Dictionary<string, object> productData)
        {
            var result = await _shoppingCartService.AddToShoppingCartAsync(userId, productId, productData);
            return result ? Ok(new { message = "Sản phẩm đã được thêm vào giỏ hàng." })
                          : StatusCode(500, new { message = "Lỗi khi thêm sản phẩm." });
        }

        // ✅ [DELETE] Xóa sản phẩm khỏi giỏ hàng
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromShoppingCart(string userId, string productId)
        {
            var result = await _shoppingCartService.RemoveFromShoppingCartAsync(userId, productId);
            return result ? Ok(new { message = "Sản phẩm đã được xóa khỏi giỏ hàng." })
                          : NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
        }
    }
}
