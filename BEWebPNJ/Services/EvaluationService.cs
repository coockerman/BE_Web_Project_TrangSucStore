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
        // ✅ Lấy danh sách đánh giá theo danh sách ID
        public async Task<List<Evaluation>> GetEvaluationsByIdsAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0) return new List<Evaluation>();

            List<Task<DocumentSnapshot>> tasks = ids
                .Select(id => _firestoreDb.Collection(CollectionName).Document(id).GetSnapshotAsync())
                .ToList();

            DocumentSnapshot[] snapshots = await Task.WhenAll(tasks);

            return snapshots
                .Where(snapshot => snapshot.Exists)
                .Select(snapshot => snapshot.ConvertTo<Evaluation>())
                .ToList();
        }

        // ✅ Thêm hoặc cập nhật đánh giá với mốc thời gian
        public async Task<bool> SetEvaluationAsync(Evaluation evaluation)
        {
            CollectionReference collectionRef = _firestoreDb.Collection(CollectionName);

            // Nếu không có id, tạo mới document và lấy ID tự động
            if (string.IsNullOrEmpty(evaluation.id))
            {
                DocumentReference newDocRef = await collectionRef.AddAsync(new { });
                evaluation.id = newDocRef.Id; // ✅ Gán ID mới tạo
            }

            // Cập nhật thời gian đánh giá
            evaluation.timeEvaluation = DateTime.UtcNow.AddHours(7);

            // Lưu dữ liệu với ID mới hoặc cập nhật nếu đã có ID
            DocumentReference docRef = collectionRef.Document(evaluation.id);
            await docRef.SetAsync(evaluation, SetOptions.MergeAll);

            return true; // ✅ Giữ nguyên evaluation.id để API có thể trả về ID
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
