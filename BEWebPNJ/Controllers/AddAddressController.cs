using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BEWebPNJ.Models;
using BEWebPNJ.Services;

namespace BEWebPNJ.Controllers
{
    [Route("api/users/{userId}/addAddress")]
    [ApiController]
    public class AddAddressController : ControllerBase
    {
        private readonly AddAddressService _addressService;

        public AddAddressController(AddAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AddAddress>>> GetUserAddresses(string userId)
        {
            var addresses = await _addressService.GetUserAddressesAsync(userId);
            return addresses.Any() ? Ok(addresses) : NotFound(new { message = "Không có địa chỉ nào." });
        }


        // ✅ [GET] Lấy địa chỉ theo ID
        [HttpGet("{addressId}")]
        public async Task<ActionResult<AddAddress>> GetAddressById(string userId, string addressId)
        {
            var address = await _addressService.GetAddressByIdAsync(userId, addressId);
            return address != null ? Ok(address) : NotFound(new { message = $"Địa chỉ {addressId} không tồn tại." });
        }

        // ✅ [POST] Thêm địa chỉ mới (ID tự sinh)
        [HttpPost]
        public async Task<IActionResult> AddAddress(string userId, [FromBody] AddAddress address)
        {
            var addressId = await _addressService.AddAddressAsync(userId, address);
            return addressId != null
                ? Ok(new { message = "Thêm địa chỉ thành công.", addressId })
                : StatusCode(500, new { message = "Lỗi khi thêm địa chỉ." });
        }

        // ✅ [PUT] Cập nhật địa chỉ có ID
        [HttpPut("{addressId}")]
        public async Task<IActionResult> UpdateAddress(string userId, string addressId, [FromBody] AddAddress address)
        {
            var result = await _addressService.UpdateAddressAsync(userId, addressId, address);
            return result
                ? Ok(new { message = "Cập nhật địa chỉ thành công." })
                : StatusCode(500, new { message = "Lỗi khi cập nhật địa chỉ." });
        }

        // ✅ [DELETE] Xóa địa chỉ theo ID
        [HttpDelete("{addressId}")]
        public async Task<IActionResult> DeleteAddress(string userId, string addressId)
        {
            var result = await _addressService.DeleteAddressAsync(userId, addressId);
            return result ? Ok(new { message = "Xóa địa chỉ thành công." }) : NotFound(new { message = $"Không tìm thấy địa chỉ {addressId}." });
        }
    }
}
