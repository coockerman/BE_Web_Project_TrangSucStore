using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BEWebPNJ.Models;

namespace BEWebPNJ.Services
{
    public class OrderService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Orders"; // Tên collection trên Firestore


        public OrderService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }


        // 🔹 Tạo đơn hàng mới
        public async Task<string> CreateOrder(Order order)
        {
            try
            {
                order.id = _firestoreDb.Collection(CollectionName).Document().Id; // Tạo ID tự động

                // 🔹 Lấy thời gian hiện tại theo giờ Việt Nam (UTC+7)
                order.timeOrder = DateTime.UtcNow.AddHours(7);

                DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(order.id);
                await docRef.SetAsync(order);
                return order.id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo đơn hàng: {ex.Message}");
                return string.Empty;
            }
        }
        // 🔹 Cập nhật paymentId cho đơn hàng
        public async Task<bool> UpdateOrderPaymentId(string orderId, string paymentId)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(orderId);
                await docRef.UpdateAsync("paymentId", paymentId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật paymentId cho đơn hàng: {ex.Message}");
                return false;
            }
        }


        // 🔹 Lấy đơn hàng theo ID
        public async Task<Order?> GetOrderById(string orderId)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(orderId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    return snapshot.ConvertTo<Order>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy đơn hàng: {ex.Message}");
                return null;
            }
        }

        // 🔹 Lấy đơn hàng theo paymentId
        public async Task<Order?> GetOrderByPaymentId(string paymentId)
        {
            try
            {
                Query query = _firestoreDb.Collection(CollectionName).WhereEqualTo("paymentId", paymentId);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                if (snapshot.Documents.Count > 0)
                {
                    return snapshot.Documents[0].ConvertTo<Order>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy đơn hàng theo paymentId: {ex.Message}");
                return null;
            }
        }

        // 🔹 Lấy danh sách đơn hàng theo idUserOrder
        public async Task<List<Order>> GetOrdersByUserId(string userId)
        {
            try
            {
                Query query = _firestoreDb.Collection(CollectionName)
                                          .WhereEqualTo("idUserOrder", userId);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                return snapshot.Documents.Select(doc => doc.ConvertTo<Order>()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy đơn hàng theo idUserOrder: {ex.Message}");
                return new List<Order>();
            }
        }


        // 🔹 Lấy danh sách tất cả đơn hàng
        public async Task<List<Order>> GetAllOrders()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Order>()).ToList();
            //try
            //{
            //    Query query = _firestoreDb.Collection(CollectionName);
            //    QuerySnapshot snapshot = await query.GetSnapshotAsync();

            //    List<Order> orders = new();
            //    foreach (DocumentSnapshot doc in snapshot.Documents)
            //    {
            //        orders.Add(doc.ConvertTo<Order>());
            //    }
            //    return orders;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
            //    return new List<Order>();
            //}
        }

        // 🔹 Cập nhật trạng thái đơn hàng
        public async Task<bool> UpdateOrderStatus(string orderId, string newStatus)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(orderId);
                await docRef.UpdateAsync("status", newStatus);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}");
                return false;
            }
        }

        // 🔹 Xóa đơn hàng
        public async Task<bool> DeleteOrder(string orderId)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(orderId);
                await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa đơn hàng: {ex.Message}");
                return false;
            }
        }
    }
}
