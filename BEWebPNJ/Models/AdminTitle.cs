using Google.Cloud.Firestore;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class AdminTitle
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string acronym { get; set; }
        [FirestoreProperty]
        public string address { get; set; }
        [FirestoreProperty]
        public string business { get; set; }
        [FirestoreProperty]
        public string buy { get; set; }
        [FirestoreProperty]
        public string complaint { get; set; }
        [FirestoreProperty]
        public string fax { get; set; }

        [FirestoreProperty]
        public string nameShop { get; set; }

        [FirestoreProperty]
        public string numberPhone { get; set; }

    }
}
