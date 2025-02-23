using BEWebPNJ.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEWebPNJ.Services
{
    public class EvaluationService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Evaluations";

        public EvaluationService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // ✅ Lấy danh sách tất cả đánh giá
        public async Task<List<Evaluation>> GetAllEvaluationsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Evaluation>()).ToList();
        }

        // ✅ Lấy thông tin đánh giá theo ID
        public async Task<Evaluation?> GetEvaluationByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Evaluation>() : null;
        }

        // ✅ Thêm hoặc cập nhật đánh giá
        public async Task<bool> SetEvaluationAsync(Evaluation evaluation)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(evaluation.id);
            await docRef.SetAsync(evaluation, SetOptions.MergeAll);
            return true;
        }

        // ✅ Cập nhật đánh giá (chỉ cập nhật trường cần thiết)
        public async Task<bool> UpdateEvaluationAsync(string id, Dictionary<string, object> updates)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.UpdateAsync(updates);
            return true;
        }

        // ✅ Xóa đánh giá theo ID
        public async Task<bool> DeleteEvaluationAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }
    }
}
