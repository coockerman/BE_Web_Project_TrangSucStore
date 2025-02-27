using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/revenues")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly RevenueService _revenueService;

        public RevenueController(RevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        // ✅ [GET] Lấy doanh thu trong tuần
        [HttpGet("weekly")]
        public async Task<ActionResult<decimal>> GetWeeklyRevenue()
        {
            try
            {
                var revenue = await _revenueService.GetWeeklyRevenue();
                return Ok(new { message = "Doanh thu tuần", revenue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // ✅ [GET] Lấy doanh thu trong tháng
        [HttpGet("monthly")]
        public async Task<ActionResult<decimal>> GetMonthlyRevenue()
        {
            try
            {
                var revenue = await _revenueService.GetMonthlyRevenue();
                return Ok(new { message = "Doanh thu tháng", revenue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // ✅ [GET] Lấy doanh thu trong quý
        [HttpGet("quarterly")]
        public async Task<ActionResult<decimal>> GetQuarterlyRevenue()
        {
            try
            {
                var revenue = await _revenueService.GetQuarterlyRevenue();
                return Ok(new { message = "Doanh thu quý", revenue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // ✅ [GET] Lấy doanh thu trong năm
        [HttpGet("yearly")]
        public async Task<ActionResult<decimal>> GetYearlyRevenue()
        {
            try
            {
                var revenue = await _revenueService.GetYearlyRevenue();
                return Ok(new { message = "Doanh thu năm", revenue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }
    }
}
