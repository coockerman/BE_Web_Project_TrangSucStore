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
        // ✅ Lấy danh sách sản phẩm theo type
        public async Task<List<Product>> GetProductsByTypeAsync(string type)
        {
            Query query = _firestoreDb.Collection(CollectionName).WhereEqualTo("type", type);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(doc => doc.ConvertTo<Product>()).ToList();
        }

        // ✅ Lấy thông tin sản phẩm theo ID
        public async Task<Product?> GetProductByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Product>() : null;
        }

        // ✅ Lấy danh sách sản phẩm theo danh sách ID
        public async Task<List<Product>> GetProductsByIdsAsync(List<string> ids)
        {
            var tasks = ids.Select(async id =>
            {
                DocumentSnapshot snapshot = await _firestoreDb.Collection(CollectionName).Document(id).GetSnapshotAsync();
                return snapshot.Exists ? snapshot.ConvertTo<Product>() : null;
            });

            var products = await Task.WhenAll(tasks);
            return products.Where(p => p != null).ToList();
        }


        public async Task<string> SetProductAsync(Product product)
        {
            CollectionReference collection = _firestoreDb.Collection(CollectionName);

            if (string.IsNullOrEmpty(product.id))
            {
                // Tạo document mới với ID tự sinh
                DocumentReference docRef = await collection.AddAsync(product);
                product.id = docRef.Id; // Gán lại ID vừa sinh cho product

                // Cập nhật ID vào Firestore để đảm bảo dữ liệu đầy đủ
                await docRef.UpdateAsync(new Dictionary<string, object> { { "id", product.id } });

                System.Console.WriteLine($"[INFO] Thêm sản phẩm mới: {product.id}");
            }
            else
            {
                DocumentReference docRef = collection.Document(product.id);
                await docRef.SetAsync(product, SetOptions.MergeAll);

                System.Console.WriteLine($"[INFO] Cập nhật sản phẩm: {product.id}");
            }

            return product.id;
        }
        public async Task<bool> DecreaseProductStockAsync(string productId, int size, long quantity)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(productId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            Product product = snapshot.ConvertTo<Product>();

            if (product.sizePrice == null || product.sizePrice.Count == 0) return false;

            // Tìm size tương ứng
            var sizePrice = product.sizePrice.FirstOrDefault(sp => sp.size == size);
            if (sizePrice == null || sizePrice.stock < quantity) return false;

            // Giảm số lượng tồn kho
            sizePrice.stock -= quantity;

            // Cập nhật lại Firestore
            await docRef.UpdateAsync(new Dictionary<string, object>
    {
        { "sizePrice", product.sizePrice }
    });

            return true;
        }

        public async Task<bool> AddEvaluationToProductAsync(string productId, string evaluationId)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(productId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            // Lấy danh sách đánh giá hiện tại
            Product product = snapshot.ConvertTo<Product>();

            if (product.listEvaluation == null)
            {
                product.listEvaluation = new List<string>();
            }

            // Thêm id đánh giá vào danh sách (tránh trùng lặp)
            if (!product.listEvaluation.Contains(evaluationId))
            {
                product.listEvaluation.Add(evaluationId);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "listEvaluation", product.listEvaluation } });
            }

            return true;
        }

        public async Task<bool> AddCommentToProductAsync(string productId, string commentId)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(productId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            // Lấy danh sách đánh giá hiện tại
            Product product = snapshot.ConvertTo<Product>();

            if (product.listComments == null)
            {
                product.listComments = new List<string>();
            }

            // Thêm id đánh giá vào danh sách (tránh trùng lặp)
            if (!product.listComments.Contains(commentId))
            {
                product.listComments.Add(commentId);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "listComments", product.listComments } });
            }

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

        public async Task<List<Product>> SearchProductsByNameAsync(string keyword)
        {
            Query query = _firestoreDb.Collection(CollectionName)
                .OrderBy("nameProduct")
                .StartAt(keyword)
                .EndAt(keyword + "\uf8ff"); // Ký tự đặc biệt giúp tìm kiếm gần đúng

            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Product>()).ToList();
        }

    }
}
