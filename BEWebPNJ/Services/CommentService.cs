using BEWebPNJ.Controllers;
using BEWebPNJ.Models;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEWebPNJ.Services
{
    public class CommentService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Comments";

        public CommentService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // ✅ Lấy danh sách tất cả bình luận
        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Comment>()).ToList();
        }

        // ✅ Lấy thông tin bình luận theo ID
        public async Task<Comment?> GetCommentByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            return snapshot.Exists ? snapshot.ConvertTo<Comment>() : null;
        }

        // ✅ Lấy danh sách bình luận theo sản phẩm
        public async Task<List<Comment>> GetCommentsByProductIdAsync(string productId)
        {
            Query query = _firestoreDb.Collection(CollectionName).WhereEqualTo("idProduct", productId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(doc => doc.ConvertTo<Comment>()).ToList();
        }

        // ✅ Thêm hoặc cập nhật bình luận
        public async Task<string> SetCommentAsync(Comment comment)
        {
            CollectionReference collectionRef = _firestoreDb.Collection(CollectionName);

            if (string.IsNullOrEmpty(comment.id))
            {
                DocumentReference newDocRef = await collectionRef.AddAsync(new { });
                comment.id = newDocRef.Id;
            }

            comment.timeComment = DateTime.UtcNow.AddHours(7);
            DocumentReference docRef = collectionRef.Document(comment.id);
            await docRef.SetAsync(comment, SetOptions.MergeAll);

            return comment.id;
        }

        public async Task<bool> UpdateCommentAsync(string id, UpdateCommentDto updateData)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            // Chuyển DTO thành Dictionary chỉ chứa các trường cần cập nhật
            var updates = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(updateData.content))
                updates["content"] = updateData.content;

            if (updateData.hasFix)
                updates["hasFix"] = updateData.hasFix;

            if (updates.Count == 0)
                return false; // Không có gì để cập nhật

            await docRef.UpdateAsync(updates);
            return true;
        }


        // ✅ Xóa bình luận theo ID
        public async Task<bool> DeleteCommentAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }
    }
}
