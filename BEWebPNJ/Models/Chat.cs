using Google.Cloud.Firestore;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class ChatMessage
    {
        [FirestoreProperty]
        public string id { get; set; } 

        [FirestoreProperty]
        public string question { get; set; } 

        [FirestoreProperty]
        public string result { get; set; }

        [FirestoreProperty]
        public int stats { get; set; } 
    }
}
