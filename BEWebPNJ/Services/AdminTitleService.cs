using BEWebPNJ.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEWebPNJ.Services
{
    public class AdminTitleService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "AdminTitle";

        public AdminTitleService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // ✅ Lấy danh sách tất cả AdminTitle
        public async Task<List<AdminTitle>> GetAllAdminTitlesAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<AdminTitle>()).ToList();
        }

        // ✅ Lấy thông tin AdminTitle theo ID
        public async Task<AdminTitle?> GetAdminTitleByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<AdminTitle>() : null;
        }

        // ✅ Thêm hoặc cập nhật AdminTitle
        public async Task<bool> SetAdminTitleAsync(AdminTitle adminTitle)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(adminTitle.id);
            await docRef.SetAsync(adminTitle, SetOptions.MergeAll);
            return true;
        }

        // ✅ Cập nhật AdminTitle (chỉ cập nhật trường cần thiết)
        public async Task<bool> UpdateAdminTitleAsync(string id, Dictionary<string, object> updates)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.UpdateAsync(updates);
            return true;
        }

        // ✅ Xóa AdminTitle theo ID
        public async Task<bool> DeleteAdminTitleAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }
    }
}
