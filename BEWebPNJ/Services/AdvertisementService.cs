using BEWebPNJ.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEWebPNJ.Services
{
    public class AdvertisementService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Advertisement";

        public AdvertisementService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // ✅ Lấy danh sách tất cả quảng cáo
        public async Task<List<Advertisement>> GetAllAdvertisementsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Advertisement>()).ToList();
        }

        // ✅ Lấy thông tin quảng cáo theo ID
        public async Task<Advertisement?> GetAdvertisementByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Advertisement>() : null;
        }

        // ✅ Thêm hoặc cập nhật quảng cáo
        public async Task<bool> SetAdvertisementAsync(Advertisement advertisement)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(advertisement.id);
            await docRef.SetAsync(advertisement, SetOptions.MergeAll);
            return true;
        }

        // ✅ Cập nhật quảng cáo (chỉ cập nhật trường cần thiết)
        public async Task<bool> UpdateAdvertisementAsync(string id, Dictionary<string, object> updates)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.UpdateAsync(updates);
            return true;
        }

        // ✅ Xóa quảng cáo theo ID
        public async Task<bool> DeleteAdvertisementAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }
    }
}