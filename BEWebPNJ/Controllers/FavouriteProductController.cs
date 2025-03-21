using Microsoft.AspNetCore.Mvc;
using BEWebPNJ.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/users/{userId}/favourites")]
    [ApiController]
    public class FavouriteProductController : ControllerBase
    {
        private readonly FavouriteProductService _favouriteProductService;

        public FavouriteProductController(FavouriteProductService favouriteProductService)
        {
            _favouriteProductService = favouriteProductService;
        }

        // ✅ [GET] Lấy danh sách ID sản phẩm yêu thích của user
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetFavouriteProducts(string userId)
        {
            var favouriteProducts = await _favouriteProductService.GetFavouriteProductsAsync(userId);
            return Ok(favouriteProducts);
        }

        // ✅ [POST] Thêm sản phẩm vào danh sách yêu thích
        [HttpPost("add/{productId}")]
        public async Task<IActionResult> AddFavouriteProduct(string userId, string productId)
        {
            var result = await _favouriteProductService.AddFavouriteProductAsync(userId, productId);
            return result ? Ok(new { message = "Sản phẩm đã được thêm vào danh sách favourite." })
                          : StatusCode(500, new { message = "Lỗi khi thêm sản phẩm." });
        }

        // ✅ [DELETE] Xóa sản phẩm khỏi danh sách yêu thích
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFavouriteProduct(string userId, string productId)
        {
            var result = await _favouriteProductService.RemoveFavouriteProductAsync(userId, productId);
            return result ? Ok(new { message = "Sản phẩm đã được xóa khỏi danh sách favourite." })
                          : NotFound(new { message = "Không tìm thấy sản phẩm trong danh sách favourite." });
        }
    }
}
