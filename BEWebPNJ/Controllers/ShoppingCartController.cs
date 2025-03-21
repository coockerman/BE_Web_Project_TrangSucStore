using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BEWebPNJ.Services;
using BEWebPNJ.Models;

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
        public async Task<ActionResult<List<ShoppingCart>>> GetUserShoppingCart(string userId)
        {
            var addresses = await _shoppingCartService.GetUserShoppingCartAsync(userId);
            return addresses.Any() ? Ok(addresses) : NotFound(new { message = "Không có san pham nào." });
        }

        // ✅ [GET] Lấy san pham gio hang theo ID
        [HttpGet("{shoppingCartId}")]
        public async Task<ActionResult<ShoppingCart>> GetShoppingCartById(string userId, string shoppingCartId)
        {
            var address = await _shoppingCartService.GetShoppingCartByIdAsync(userId, shoppingCartId);
            return address != null ? Ok(address) : NotFound(new { message = $"Địa chỉ {shoppingCartId} không tồn tại." });
        }

        // ✅ [POST] Thêm sản phẩm vào giỏ hàng
        [HttpPost("add")]
        public async Task<IActionResult> AddShoppingCart(string userId, [FromBody] ShoppingCart shoppingCart)
        {
            var addressId = await _shoppingCartService.AddShoppingCartAsync(userId, shoppingCart);
            return addressId != null
                ? Ok(new { message = "Thêm san pham thành công.", addressId })
                : StatusCode(500, new { message = "Lỗi khi thêm san pham." });
        }

        [HttpPut("update/{shoppingCartId}")]
        public async Task<IActionResult> UpdateShoppingCart(string userId, string shoppingCartId, [FromBody] ShoppingCart shoppingCart)
        {
            var result = await _shoppingCartService.UpdateShoppingCartAsync(userId, shoppingCartId, shoppingCart);
            return result
                ? Ok(new { message = "Cập nhật san pham thành công." })
                : StatusCode(500, new { message = "Lỗi khi cập nhật san pham." });
        }

        // ✅ [DELETE] Xóa sản phẩm khỏi giỏ hàng theo ID & size (truyền size qua URL)
        [HttpDelete("remove/{shoppingCartId}")]
        public async Task<IActionResult> DeleteShoppingCart(string userId, string shoppingCartId)
        {
            var result = await _shoppingCartService.DeleteShoppingCartByIdAsync(userId, shoppingCartId);
            return result
                ? Ok(new { message = "Xóa sản phẩm thành công." })
                : NotFound(new { message = $"Không tìm thấy sản phẩm {shoppingCartId}." });
        }


    }
}
