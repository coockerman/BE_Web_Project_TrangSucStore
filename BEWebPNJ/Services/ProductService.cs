using BEWebPNJ.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEWebPNJ.Services
{
    public class ProductService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Products";

        public ProductService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // ✅ Lấy danh sách tất cả sản phẩm
        public async Task<List<Product>> GetAllProductsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Product>()).ToList();
        }

        // ✅ Lấy thông tin sản phẩm theo ID
        public async Task<Product?> GetProductByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Product>() : null;
        }

        // ✅ Thêm hoặc cập nhật sản phẩm
        public async Task<bool> SetProductAsync(Product product)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(product.Id);
            await docRef.SetAsync(product, SetOptions.MergeAll);
            return true;
        }

        // ✅ Cập nhật sản phẩm (chỉ cập nhật trường cần thiết)
        public async Task<bool> UpdateProductAsync(string id, Dictionary<string, object> updates)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            // Đảm bảo ListEvaluation chỉ nhận List<string> (tránh lỗi kiểu dữ liệu)
            if (updates.ContainsKey("ListEvaluation") && updates["ListEvaluation"] is List<object> rawList)
            {
                updates["ListEvaluation"] = rawList.Select(x => x.ToString()).ToList();
            }

            await docRef.UpdateAsync(updates);
            return true;
        }

        // ✅ Xóa sản phẩm theo ID
        public async Task<bool> DeleteProductAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }
    }
}
