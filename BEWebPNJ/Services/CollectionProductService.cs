using BEWebPNJ.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEWebPNJ.Services
{
    public class CollectionProductService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "CollectionProducts";

        public CollectionProductService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // ✅ Lấy danh sách tất cả CollectionProducts
        public async Task<List<CollectionProduct>> GetAllCollectionsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<CollectionProduct>()).ToList();
        }

        // ✅ Lấy CollectionProduct theo ID
        public async Task<CollectionProduct?> GetCollectionByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<CollectionProduct>() : null;
        }

        // ✅ Tạo hoặc cập nhật CollectionProduct
        public async Task<bool> SetCollectionAsync(CollectionProduct collection)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(collection.id);
            await docRef.SetAsync(collection, SetOptions.MergeAll);
            return true;
        }

        // ✅ Cập nhật danh sách sản phẩm trong CollectionProduct
        public async Task<bool> UpdateCollectionAsync(string id, Dictionary<string, object> updates)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            // Đảm bảo ListProduct chỉ nhận List<string>
            if (updates.ContainsKey("ListProduct") && updates["ListProduct"] is List<object> rawList)
            {
                updates["ListProduct"] = rawList.Select(x => x.ToString()).ToList();
            }

            await docRef.UpdateAsync(updates);
            return true;
        }

        // ✅ Xóa CollectionProduct theo ID
        public async Task<bool> DeleteCollectionAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }
    }
}
