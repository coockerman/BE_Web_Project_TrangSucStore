using BEWebPNJ.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEWebPNJ.Services
{
    public class ChatService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Chats";

        public ChatService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // ✅ Lấy thông tin câu hỏi theo ID
        public async Task<ChatMessage?> GetChatMessageById(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<ChatMessage>() : null;
        }

        // ✅ Tăng số lần được hỏi
        public async Task<bool> IncreaseQuestionCount(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            var chatMessage = snapshot.ConvertTo<ChatMessage>();
            chatMessage.stats += 1;

            await docRef.UpdateAsync("stats", chatMessage.stats);
            return true;
        }

        // ✅ Lấy danh sách câu hỏi được hỏi nhiều nhất (top N)
        public async Task<List<ChatMessage>> GetTopAskedQuestions(int topN)
        {
            Query query = _firestoreDb.Collection(CollectionName)
                .OrderByDescending("stats")
                .Limit(topN);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<ChatMessage>()).ToList();
        }

        // ✅ Cập nhật câu hỏi và câu trả lời
        public async Task<bool> UpdateQuestionAndAnswer(string id, string newQuestion, string newAnswer)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            Dictionary<string, object> updates = new()
            {
                { "question", newQuestion },
                { "result", newAnswer }
            };

            await docRef.UpdateAsync(updates);
            return true;
        }
         // ✅ Thêm bình luận mới vào Firestore
        public async Task<string> AddChatMessageAsync(ChatMessage chatMessage)
        {
            chatMessage.id = Guid.NewGuid().ToString(); // Tạo ID mới cho bình luận
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(chatMessage.id);
            await docRef.SetAsync(chatMessage);
            return chatMessage.id;
        }
    }
}
