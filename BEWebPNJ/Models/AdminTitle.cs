using Google.Cloud.Firestore;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class AdminTitle
    {
        [FirestoreProperty]
        public string id { get; set; }

        [FirestoreProperty]
        public string phone1 { get; set; }

        [FirestoreProperty]
        public string nameShop { get; set; }

        [FirestoreProperty]
        public string phone2 { get; set; }

        [FirestoreProperty]
        public string address { get; set; }

        [FirestoreProperty]
        public string acronym { get; set; }

        [FirestoreProperty]
        public List<string> advertisement { get; set; } = new List<string>();
    }
}
