using Google.Cloud.Firestore;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class Evaluation
    {
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Content { get; set; } = string.Empty;

        [FirestoreProperty]
        public string IdProduct { get; set; } = string.Empty;

        [FirestoreProperty]
        public int Size { get; set; }

        [FirestoreProperty]
        public string EmailUser { get; set; } = string.Empty;

        [FirestoreProperty]
        public string IdUser { get; set; } = string.Empty;

        [FirestoreProperty]
        public string NameProduct { get; set; } = string.Empty;

        [FirestoreProperty]
        public int PriceProduct { get; set; }

        [FirestoreProperty]
        public int Star { get; set; }

        public Evaluation() { }
    }
}
