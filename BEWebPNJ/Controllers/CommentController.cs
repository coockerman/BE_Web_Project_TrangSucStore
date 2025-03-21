using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/comment")]
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
        public async Task<IActionResult> UpdateComment(string id, [FromBody] UpdateCommentDto updateData)
        {
            if (updateData == null) return BadRequest("Dữ liệu cập nhật không hợp lệ.");
            updateData.hasFix = true;
            bool result = await _commentService.UpdateCommentAsync(id, updateData);
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
    public class UpdateCommentDto
    {
        public string content { get; set; } = "";
        public bool hasFix { get; set; } = true;
    }

}
