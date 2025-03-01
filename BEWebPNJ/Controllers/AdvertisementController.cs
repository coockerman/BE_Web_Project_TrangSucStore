using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/advertisements")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly AdvertisementService _advertisementService;

        public AdvertisementController(AdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        // ✅ [GET] Lấy danh sách tất cả quảng cáo
        [HttpGet("all")]
        public async Task<ActionResult<List<Advertisement>>> GetAllAdvertisements()
        {
            var advertisements = await _advertisementService.GetAllAdvertisementsAsync();
            return Ok(advertisements);
        }

        // ✅ [GET] Lấy quảng cáo theo ID
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<Advertisement>> GetAdvertisementById(string id)
        {
            var advertisement = await _advertisementService.GetAdvertisementByIdAsync(id);
            return advertisement != null ? Ok(advertisement) : NotFound(new { message = $"Advertisement với ID '{id}' không tồn tại." });
        }

        // ✅ [POST] Thêm hoặc cập nhật quảng cáo
        [HttpPost("create-or-update")]
        public async Task<IActionResult> SetAdvertisement([FromBody] Advertisement advertisement)
        {
            var result = await _advertisementService.SetAdvertisementAsync(advertisement);
            return result ? Ok(new { message = "Advertisement đã được lưu thành công." }) : StatusCode(500, new { message = "Lỗi khi lưu Advertisement." });
        }

        // ✅ [PATCH] Cập nhật một số trường của quảng cáo theo ID
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateAdvertisement(string id, [FromBody] Dictionary<string, object> updates)
        {
            var result = await _advertisementService.UpdateAdvertisementAsync(id, updates);
            return result ? Ok(new { message = "Advertisement đã được cập nhật thành công." }) : NotFound(new { message = $"Không tìm thấy Advertisement có ID '{id}'." });
        }

        // ✅ [DELETE] Xóa quảng cáo theo ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAdvertisement(string id)
        {
            var result = await _advertisementService.DeleteAdvertisementAsync(id);
            return result ? Ok(new { message = "Advertisement đã bị xóa thành công." }) : NotFound(new { message = $"Không tìm thấy Advertisement có ID '{id}'." });
        }
    }
}
