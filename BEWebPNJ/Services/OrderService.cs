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
                order.timeOrder = DateTime.UtcNow;

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

        // 🔹 Lấy danh sách tất cả đơn hàng
        public async Task<List<Order>> GetAllOrders()
        {
            try
            {
                Query query = _firestoreDb.Collection(CollectionName);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                List<Order> orders = new();
                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    orders.Add(doc.ConvertTo<Order>());
                }
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
                return new List<Order>();
            }
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
