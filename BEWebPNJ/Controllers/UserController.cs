using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BEWebPNJ.Models;
using BEWebPNJ.Services;

namespace BEWebPNJ.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // ✅ [GET] Lấy danh sách tất cả user
        [HttpGet("all")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // ✅ [GET] Lấy user theo ID
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user != null ? Ok(user) : NotFound(new { message = $"User với ID '{id}' không tồn tại." });
        }

        // ✅ [POST] Thêm hoặc cập nhật user
        [HttpPost("create-or-update")]
        public async Task<IActionResult> SetUser([FromBody] User user)
        {
            var result = await _userService.SetUserAsync(user);
            return result ? Ok(new { message = "User đã được lưu thành công." }) : StatusCode(500, new { message = "Lỗi khi lưu user." });
        }

        // ✅ [PATCH] Cập nhật một số trường của user theo ID
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] Dictionary<string, object> updates)
        {
            var result = await _userService.UpdateUserAsync(id, updates);
            return result ? Ok(new { message = "User đã được cập nhật thành công." }) : NotFound(new { message = $"Không tìm thấy user có ID '{id}'." });
        }

        // ✅ [PATCH] Cập nhật thông tin cơ bản của user
        [HttpPatch("update-info/{id}")]
        public async Task<IActionResult> UpdateUserInfo(string id, [FromBody] User userInfo)
        {
            var updates = new Dictionary<string, object>
            {
                { "fullName", userInfo.fullName },
                { "sex", userInfo.sex },
                { "numberPhone", userInfo.numberPhone }
            };

            var result = await _userService.UpdateUserAsync(id, updates);
            return result ? Ok(new { message = "Thông tin người dùng đã được cập nhật." }) : NotFound(new { message = $"Không tìm thấy user có ID '{id}'." });
        }

        // ✅ [DELETE] Xóa user theo ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result ? Ok(new { message = "User đã bị xóa thành công." }) : NotFound(new { message = $"Không tìm thấy user có ID '{id}'." });
        }
    }
}
