using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        // ✅ Lấy danh sách tất cả bình luận
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }

        // ✅ Lấy thông tin bình luận theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(string id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            return comment != null ? Ok(comment) : NotFound("Bình luận không tồn tại.");
        }

        // ✅ Lấy danh sách bình luận theo sản phẩm
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetCommentsByProductId(string productId)
        {
            var comments = await _commentService.GetCommentsByProductIdAsync(productId);
            return Ok(comments);
        }

        // ✅ Thêm hoặc cập nhật bình luận
        [HttpPost]
        public async Task<IActionResult> SetComment([FromBody] Comment comment)
        {
            if (comment == null) return BadRequest("Dữ liệu không hợp lệ.");

            string commentId = await _commentService.SetCommentAsync(comment);
            return Ok(new { message = "Bình luận đã được lưu.", commentId });
        }

        // ✅ Cập nhật bình luận
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateComment(string id, [FromBody] Dictionary<string, object> updates)
        {
            if (updates == null || updates.Count == 0) return BadRequest("Dữ liệu cập nhật không hợp lệ.");

            bool result = await _commentService.UpdateCommentAsync(id, updates);
            return result ? Ok("Bình luận đã được cập nhật.") : NotFound("Bình luận không tồn tại.");
        }

        // ✅ Xóa bình luận theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            bool result = await _commentService.DeleteCommentAsync(id);
            return result ? Ok("Bình luận đã được xóa.") : NotFound("Bình luận không tồn tại.");
        }
    }
}
