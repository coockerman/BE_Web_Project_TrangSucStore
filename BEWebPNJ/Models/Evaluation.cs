using Google.Cloud.Firestore;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class Evaluation
    {
        [FirestoreProperty]
        public string id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string content { get; set; } = string.Empty;

        [FirestoreProperty]
        public string idProduct { get; set; } = string.Empty;

        [FirestoreProperty]
        public string nameUser { get; set; } = string.Empty;

        [FirestoreProperty]
        public string emailUser { get; set; } = string.Empty;

        [FirestoreProperty]
        public string idUser { get; set; } = string.Empty;

        [FirestoreProperty]
        public string nameProduct { get; set; } = string.Empty;

        [FirestoreProperty]
        public DateTime timeEvaluation { get; set; } = DateTime.UtcNow.AddHours(7);

        [FirestoreProperty]
        public int star { get; set; } = 0;

        public Evaluation() { }
    }
}
