using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // ✅ [GET] Lấy danh sách tất cả sản phẩm
        [HttpGet("all")]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // ✅ [GET] Lấy sản phẩm theo ID
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product != null ? Ok(product) : NotFound(new { message = $"Sản phẩm với ID '{id}' không tồn tại." });
        }

        // ✅ [POST] Lấy danh sách sản phẩm theo danh sách ID
        [HttpPost("list-by-ids")]
        public async Task<ActionResult<List<Product>>> GetProductsByIds([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest(new { message = "Danh sách ID không hợp lệ." });

            var products = await _productService.GetProductsByIdsAsync(ids);
            return Ok(products);
        }

        // ✅ [POST] Thêm hoặc cập nhật sản phẩm
        [HttpPost("create-or-update")]
        public async Task<IActionResult> SetProduct([FromBody] Product product)
        {
            var result = await _productService.SetProductAsync(product);
            return result ? Ok(new { message = "Sản phẩm đã được lưu thành công." }) : StatusCode(500, new { message = "Lỗi khi lưu sản phẩm." });
        }

        // ✅ [PATCH] Cập nhật một số trường của sản phẩm theo ID
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Dictionary<string, object> updates)
        {
            var result = await _productService.UpdateProductAsync(id, updates);
            return result ? Ok(new { message = "Sản phẩm đã được cập nhật thành công." }) : NotFound(new { message = $"Không tìm thấy sản phẩm có ID '{id}'." });
        }

        // ✅ [DELETE] Xóa sản phẩm theo ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return result ? Ok(new { message = "Sản phẩm đã bị xóa thành công." }) : NotFound(new { message = $"Không tìm thấy sản phẩm có ID '{id}'." });
        }
    }
}
