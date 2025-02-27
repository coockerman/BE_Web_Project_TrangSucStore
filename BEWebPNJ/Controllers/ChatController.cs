using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        // ✅ [GET] Lấy thông tin câu hỏi theo ID và tăng số lần được hỏi
        [HttpGet("question/{id}")]
        public async Task<IActionResult> GetChatMessageById(string id)
        {
            var chatMessage = await _chatService.GetChatMessageById(id);
            if (chatMessage == null)
                return NotFound(new { message = $"Không tìm thấy câu hỏi có ID '{id}'." });

            // Tăng số lần được hỏi
            await _chatService.IncreaseQuestionCount(id);

            return Ok(chatMessage);
        }

        // ✅ [GET] Lấy danh sách câu hỏi được hỏi nhiều nhất (Top N)
        [HttpGet("top/{topN}")]
        public async Task<IActionResult> GetTopAskedQuestions(int topN)
        {
            var topQuestions = await _chatService.GetTopAskedQuestions(topN);
            return Ok(topQuestions);
        }

        // ✅ [PATCH] Cập nhật câu hỏi và câu trả lời
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateQuestionAndAnswer(string id, [FromBody] ChatMessage updatedMessage)
        {
            if (string.IsNullOrWhiteSpace(updatedMessage.question) || string.IsNullOrWhiteSpace(updatedMessage.result))
                return BadRequest(new { message = "Câu hỏi và câu trả lời không được để trống!" });

            var success = await _chatService.UpdateQuestionAndAnswer(id, updatedMessage.question, updatedMessage.result);
            return success
                ? Ok(new { message = "Câu hỏi và câu trả lời đã được cập nhật." })
                : NotFound(new { message = $"Không tìm thấy câu hỏi có ID '{id}'." });
        }

        // ✅ [POST] Thêm bình luận mới
        [HttpPost("add")]
        public async Task<IActionResult> AddChatMessage([FromBody] ChatMessage chatMessage)
        {
            if (string.IsNullOrWhiteSpace(chatMessage.question) || string.IsNullOrWhiteSpace(chatMessage.result))
                return BadRequest(new { message = "Câu hỏi và câu trả lời không được để trống!" });

            var chatId = await _chatService.AddChatMessageAsync(chatMessage);
            return Ok(new { message = "Bình luận đã được thêm!", chatId });
        }
    }
}
