using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/admin-titles")]
    [ApiController]
    public class AdminTitleController : ControllerBase
    {
        private readonly AdminTitleService _adminTitleService;

        public AdminTitleController(AdminTitleService adminTitleService)
        {
            _adminTitleService = adminTitleService;
        }

        // ✅ [GET] Lấy danh sách tất cả AdminTitle
        [HttpGet("all")]    
        public async Task<ActionResult<List<AdminTitle>>> GetAllAdminTitles()
        {
            var adminTitles = await _adminTitleService.GetAllAdminTitlesAsync();
            return Ok(adminTitles);
        }

        // ✅ [GET] Lấy AdminTitle theo ID
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<AdminTitle>> GetAdminTitleById(string id)
        {
            var adminTitle = await _adminTitleService.GetAdminTitleByIdAsync(id);
            return adminTitle != null ? Ok(adminTitle) : NotFound(new { message = $"AdminTitle với ID '{id}' không tồn tại." });
        }

        // ✅ [POST] Thêm hoặc cập nhật AdminTitle
        [HttpPost("create-or-update")]
        public async Task<IActionResult> SetAdminTitle([FromBody] AdminTitle adminTitle)
        {
            var result = await _adminTitleService.SetAdminTitleAsync(adminTitle);
            return result ? Ok(new { message = "AdminTitle đã được lưu thành công." }) : StatusCode(500, new { message = "Lỗi khi lưu AdminTitle." });
        }

        // ✅ [PATCH] Cập nhật một số trường của AdminTitle theo ID
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateAdminTitle(string id, [FromBody] Dictionary<string, object> updates)
        {
            var result = await _adminTitleService.UpdateAdminTitleAsync(id, updates);
            return result ? Ok(new { message = "AdminTitle đã được cập nhật thành công." }) : NotFound(new { message = $"Không tìm thấy AdminTitle có ID '{id}'." });
        }

        // ✅ [DELETE] Xóa AdminTitle theo ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAdminTitle(string id)
        {
            var result = await _adminTitleService.DeleteAdminTitleAsync(id);
            return result ? Ok(new { message = "AdminTitle đã bị xóa thành công." }) : NotFound(new { message = $"Không tìm thấy AdminTitle có ID '{id}'." });
        }
    }
}
