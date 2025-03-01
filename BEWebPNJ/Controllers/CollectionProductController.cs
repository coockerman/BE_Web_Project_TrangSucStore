using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/collections")]
    [ApiController]
    public class CollectionProductController : ControllerBase
    {
        private readonly CollectionProductService _collectionService;

        public CollectionProductController(CollectionProductService collectionService)
        {
            _collectionService = collectionService;
        }

        // ✅ [GET] Lấy danh sách tất cả bộ sưu tập sản phẩm
        [HttpGet("all")]
        public async Task<ActionResult<List<CollectionProduct>>> GetAllCollections()
        {
            var collections = await _collectionService.GetAllCollectionsAsync();
            return Ok(collections);
        }

        // ✅ [GET] Lấy bộ sưu tập theo ID
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<CollectionProduct>> GetCollectionById(string id)
        {
            var collection = await _collectionService.GetCollectionByIdAsync(id);
            return collection != null ? Ok(collection) : NotFound(new { message = $"Bộ sưu tập với ID '{id}' không tồn tại." });
        }

        // ✅ [POST] Tạo hoặc cập nhật bộ sưu tập
        [HttpPost("create-or-update")]
        public async Task<IActionResult> SetCollection([FromBody] CollectionProduct collection)
        {
            var result = await _collectionService.SetCollectionAsync(collection);
            return result ? Ok(new { message = "Bộ sưu tập đã được lưu thành công." }) : StatusCode(500, new { message = "Lỗi khi lưu bộ sưu tập." });
        }

        // ✅ [PATCH] Cập nhật danh sách sản phẩm trong bộ sưu tập
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateCollection(string id, [FromBody] Dictionary<string, object> updates)
        {
            var result = await _collectionService.UpdateCollectionAsync(id, updates);
            return result ? Ok(new { message = "Bộ sưu tập đã được cập nhật thành công." }) : NotFound(new { message = $"Không tìm thấy bộ sưu tập có ID '{id}'." });
        }

        // ✅ [DELETE] Xóa bộ sưu tập theo ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCollection(string id)
        {
            var result = await _collectionService.DeleteCollectionAsync(id);
            return result ? Ok(new { message = "Bộ sưu tập đã bị xóa thành công." }) : NotFound(new { message = $"Không tìm thấy bộ sưu tập có ID '{id}'." });
        }
    }
}
