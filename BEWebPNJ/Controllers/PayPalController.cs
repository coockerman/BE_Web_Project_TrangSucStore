using Microsoft.AspNetCore.Mvc;
using BEWebPNJ.Models;
using BEWebPNJ.Services;
using PayPal.Api;
using System;
using System.Threading.Tasks;

namespace BEWebPNJ.Controllers
{
    [Route("api/paypal")]
    [ApiController]
    public class PaypalController : ControllerBase
    {
        private readonly PaypalService _paypalService;
        private readonly OrderService _orderService; // Service cập nhật CSDL
        private readonly PurchasedService _purchasedService; // Service cập nhật CSDL
        private readonly ShoppingCartService _shoppingCartService; // Service cập nhật CSDL
        private readonly ProductService _productService; // Service lấy thông tin sản phẩm
        private readonly CouponService _couponService;
        public PaypalController(PaypalService paypalService, 
            OrderService orderService, 
            PurchasedService purchasedService, 
            ShoppingCartService shoppingCartService,
            ProductService productService,
            CouponService couponService
            )
        {
            _paypalService = paypalService;
            _orderService = orderService;
            _purchasedService = purchasedService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _couponService = couponService;
        }

        // Tạo thanh toán
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] BEWebPNJ.Models.Order order)
        {
            if (order == null) return BadRequest(new { message = "Đơn hàng không hợp lệ." });

            try
            {
                // 1️⃣ Lưu đơn hàng vào CSDL trước
                Models.Order orderNewId = order;
                List<ShoppingCart> listOrder = await _shoppingCartService.GetUserShoppingCartAsync(order.idUserOrder);

                foreach (ShoppingCart item in listOrder)
                {
                    Product product = await _productService.GetProductByIdAsync(item.idProduct);
                    if (product != null)
                    {
                        SizePrice sizeInfo = product.sizePrice.Find(o => o.size == item.size);

                        // Kiểm tra size có tồn tại không
                        if (sizeInfo == null)
                        {
                            return BadRequest(new { message = $"Sản phẩm {product.nameProduct} không có size {item.size}." });
                        }

                        // Kiểm tra số lượng tồn kho
                        if (item.stock > sizeInfo.stock)
                        {
                            return BadRequest(new { message = $"Sản phẩm {product.nameProduct} không còn đủ hàng!" });
                        }

                        // Tạo productItem mới từ ShoppingCart
                        productItem productitemNew = new productItem
                        {
                            idProductNow = item.idProduct,
                            size = item.size,
                            stock = item.stock,
                            name = product.nameProduct,
                            price = sizeInfo.price,
                            gender = product.gender,
                            type = product.type,
                            description = product.description,
                            imgURL = product.productImg.Any() ? product.productImg[0] : null // Kiểm tra ảnh có tồn tại
                        };

                        orderNewId.productItems.Add(productitemNew);
                    }
                }

                orderNewId.status = "process"; // Fix typo "precess" -> "process"
                await _orderService.CreateOrder(orderNewId);

                // 2️⃣ Gọi PayPal để tạo thanh toán (trả về paymentId & approvalUrl)
                var (paymentId, approvalUrl) = _paypalService.CreatePayment(order);

                // Kiểm tra nếu PayPal không trả về URL hợp lệ
                if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(approvalUrl))
                {
                    return StatusCode(500, new { message = "Lỗi khi tạo thanh toán qua PayPal." });
                }

                // 3️⃣ Cập nhật đơn hàng với paymentId của PayPal
                await _orderService.UpdateOrderPaymentId(orderNewId.id, paymentId);

                return Ok(new { message = "Đơn hàng đã được tạo.", url = approvalUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }


        // Xử lý thanh toán thành công
        [HttpGet("payment-success")]
        public async Task<IActionResult> PaymentSuccess(string paymentId, string PayerID)
        {
            var executedPayment = _paypalService.ExecutePayment(paymentId, PayerID);

            if (executedPayment.state.ToLower() == "approved")
            {
                // 🔹 Tìm đơn hàng theo paymentId
                Models.Order order = await _orderService.GetOrderByPaymentId(paymentId);
                if (order == null)
                {
                    return BadRequest(new { message = "Không tìm thấy đơn hàng tương ứng!" });
                }

                foreach(productItem product in order.productItems)
                {
                    bool isDecrease = await _productService.DecreaseProductStockAsync(product.idProductNow, product.size ,product.stock);
                    if(!isDecrease)
                    {
                        return BadRequest(new { message = "Sản phẩm không còn đủ hàng!" });
                    }
                    await _purchasedService.AddPurchasedProductAsync(order.idUserOrder, product.idProductNow);
                    await _shoppingCartService.DeleteShoppingCartAsync(order.idUserOrder, product.idProductNow, product.size);
                }
                if(order.couponDiscount > 0)
                {
                    await _couponService.DecreaseCouponStockAsync(order.couponId, 1);
                }

                // 🔹 Cập nhật trạng thái đơn hàng trong database
                await _orderService.UpdateOrderStatus(order.id, "pending");

                return Redirect($"http://localhost:5173/user-home/payment-success?orderId={order.id}");
            }
            else
            {
                return Redirect("http://localhost:5173/user-home/payment-failed");
            }
        }

        // Xử lý thanh toán bị hủy
        [HttpGet("payment-cancel")]
        public IActionResult PaymentCancel()
        {
            return Redirect("http://localhost:5173/payment-cancel");
        }
    }
}
