using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;

namespace BEWebPNJ.Controllers
{
    [Route("api/users/{userId}/listOrderId")]
    [ApiController]
    public class ListPaymentUserController : ControllerBase
    {
        private readonly ListPaymentUserService _listPaymentUserService;

        public ListPaymentUserController(ListPaymentUserService listPaymentUser)
        {
            _listPaymentUserService = listPaymentUser;
        }

        // ✅ [GET] Lấy danh sách ID sản phẩm đã mua của user
        [HttpGet]
        public async Task<ActionResult<Dictionary<string, string>>> GetListPaymentUser(string userId)
        {
            var purchasedProducts = await _listPaymentUserService.GetOrderUserAsync(userId);
            return Ok(purchasedProducts);
        }

        // ✅ [POST] Thêm sản phẩm vào danh sách đã mua
        [HttpPost("add/{orderId}")]
        public async Task<IActionResult> AddOrderId(string userId, string orderId)
        {
            var result = await _listPaymentUserService.AddOrderUserAsync(userId, orderId);
            return result ? Ok(new { message = "Sản phẩm đã được thêm vào danh sách purchased." })
                          : StatusCode(500, new { message = "Lỗi khi thêm sản phẩm." });
        }

    }
}
