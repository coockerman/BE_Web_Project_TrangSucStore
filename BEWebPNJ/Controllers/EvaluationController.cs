using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/evaluations")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly EvaluationService _evaluationService;

        public EvaluationController(EvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        // ✅ [GET] Lấy danh sách tất cả đánh giá
        [HttpGet("all")]
        public async Task<ActionResult<List<Evaluation>>> GetAllEvaluations()
        {
            var evaluations = await _evaluationService.GetAllEvaluationsAsync();
            return Ok(evaluations);
        }

        // ✅ [GET] Lấy đánh giá theo ID
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<Evaluation>> GetEvaluationById(string id)
        {
            var evaluation = await _evaluationService.GetEvaluationByIdAsync(id);
            return evaluation != null ? Ok(evaluation) : NotFound(new { message = $"Đánh giá với ID '{id}' không tồn tại." });
        }
        // ✅ [POST] Lấy danh sách đánh giá theo danh sách ID
        [HttpPost("list-by-ids")]
        public async Task<ActionResult<List<Evaluation>>> GetEvaluationsByIds([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest(new { message = "Danh sách ID không hợp lệ." });

            var evaluations = await _evaluationService.GetEvaluationsByIdsAsync(ids);
            return Ok(evaluations);
        }



        // ✅ [POST] Thêm hoặc cập nhật đánh giá
        [HttpPost("create-or-update")]
        public async Task<IActionResult> SetEvaluation([FromBody] Evaluation evaluation)
        {
            bool result = await _evaluationService.SetEvaluationAsync(evaluation);

            if (!result) return StatusCode(500, new { message = "Lỗi khi lưu đánh giá." });

            return Ok(new { id = evaluation.id, message = "Đánh giá đã được lưu thành công." }); // ✅ Trả về ID
        }


        // ✅ [PATCH] Cập nhật một số trường của đánh giá theo ID
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateEvaluation(string id, [FromBody] Dictionary<string, object> updates)
        {
            var result = await _evaluationService.UpdateEvaluationAsync(id, updates);
            return result ? Ok(new { message = "Đánh giá đã được cập nhật thành công." }) : NotFound(new { message = $"Không tìm thấy đánh giá có ID '{id}'." });
        }

        // ✅ [DELETE] Xóa đánh giá theo ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEvaluation(string id)
        {
            var result = await _evaluationService.DeleteEvaluationAsync(id);
            return result ? Ok(new { message = "Đánh giá đã bị xóa thành công." }) : NotFound(new { message = $"Không tìm thấy đánh giá có ID '{id}'." });
        }
    }
}