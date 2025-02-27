using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/coupons")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly CouponService _couponService;

        public CouponController(CouponService couponService)
        {
            _couponService = couponService;
        }

        // ✅ [GET] Lấy danh sách tất cả Coupons
        [HttpGet("all")]
        public async Task<ActionResult<List<Coupon>>> GetAllCoupons()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return Ok(coupons);
        }

        // ✅ [GET] Lấy Coupon theo ID
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<Coupon>> GetCouponById(string id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            return coupon != null ? Ok(coupon) : NotFound(new { message = $"Coupon với ID '{id}' không tồn tại." });
        }

        // ✅ [POST] Thêm hoặc cập nhật Coupon
        [HttpPost("create-or-update")]
        public async Task<IActionResult> SetCoupon([FromBody] Coupon coupon)
        {
            var result = await _couponService.SetCouponAsync(coupon);
            return result ? Ok(new { message = "Coupon đã được lưu thành công." }) : StatusCode(500, new { message = "Lỗi khi lưu Coupon." });
        }

        // ✅ [PATCH] Cập nhật một số trường của Coupon theo ID
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateCoupon(string id, [FromBody] Dictionary<string, object> updates)
        {
            var result = await _couponService.UpdateCouponAsync(id, updates);
            return result ? Ok(new { message = "Coupon đã được cập nhật thành công." }) : NotFound(new { message = $"Không tìm thấy Coupon có ID '{id}'." });
        }

        // ✅ [DELETE] Xóa Coupon theo ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCoupon(string id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            return result ? Ok(new { message = "Coupon đã bị xóa thành công." }) : NotFound(new { message = $"Không tìm thấy Coupon có ID '{id}'." });
        }
    }
}
