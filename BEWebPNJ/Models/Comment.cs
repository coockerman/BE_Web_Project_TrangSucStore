using Google.Cloud.Firestore;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class Comment
    {
        [FirestoreProperty]
        public string id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string content { get; set; } = string.Empty;

        [FirestoreProperty]
        public string idProduct { get; set; } = string.Empty;

        [FirestoreProperty]
        public string idUser { get; set; } = string.Empty;

        [FirestoreProperty]
        public bool hasFix { get; set; } = false;

        [FirestoreProperty]
        public DateTime timeComment { get; set; } = DateTime.UtcNow.AddHours(7);

        public Comment() { }
    }
}
