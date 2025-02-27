using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // ✅ [POST] Tạo đơn hàng mới
        [HttpPost("create")]
        public async Task<ActionResult> CreateOrder([FromBody] Order order)
        {
            if (order.productItem == null || order.productItem.Count == 0)
                return BadRequest(new { message = "Đơn hàng không hợp lệ, không có sản phẩm nào." });

            try
            {
                var orderId = await _orderService.CreateOrder(order);
                return Ok(new { message = "Đơn hàng đã được tạo.", orderId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // ✅ [GET] Lấy thông tin đơn hàng theo ID
        [HttpGet("detail/{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(string orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            return order != null 
                ? Ok(order) 
                : NotFound(new { message = $"Không tìm thấy đơn hàng có ID '{orderId}'." });
        }

        // ✅ [GET] Lấy danh sách tất cả đơn hàng
        [HttpGet("all")]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return orders.Count > 0 
                ? Ok(orders) 
                : NotFound(new { message = "Không có đơn hàng nào trong hệ thống." });
        }

        // ✅ [PATCH] Cập nhật trạng thái đơn hàng
        [HttpPatch("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string newStatus)
        {
            var result = await _orderService.UpdateOrderStatus(orderId, newStatus);
            return result 
                ? Ok(new { message = "Trạng thái đơn hàng đã được cập nhật." }) 
                : NotFound(new { message = $"Không tìm thấy đơn hàng có ID '{orderId}'." });
        }

        // ✅ [DELETE] Xóa đơn hàng theo ID
        [HttpDelete("delete/{orderId}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            var result = await _orderService.DeleteOrder(orderId);
            return result 
                ? Ok(new { message = "Đơn hàng đã được xóa." }) 
                : NotFound(new { message = $"Không tìm thấy đơn hàng có ID '{orderId}'." });
        }
    }
}
