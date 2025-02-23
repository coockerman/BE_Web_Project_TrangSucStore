using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class Product
    {
        [FirestoreProperty]
        public string id { get; set; }

        [FirestoreProperty]
        public string nameProduct { get; set; }

        [FirestoreProperty]
        public string gender { get; set; }

        [FirestoreProperty]
        public List<SizePrice> sizePrice { get; set; }

        [FirestoreProperty]
        public List<string> listEvaluation { get; set; } = new List<string>();

        [FirestoreProperty]
        public List<string> productImg { get; set; }

        [FirestoreProperty]
        public string type { get; set; }

        [FirestoreProperty]
        public string category { get; set; }

        [FirestoreProperty]
        public string material { get; set; }

        [FirestoreProperty]
        public string description { get; set; }

        [FirestoreProperty]
        public string karat { get; set; }

        [FirestoreProperty]
        public bool show { get; set; }
    }

    [FirestoreData]
    public class SizePrice
    {
        [FirestoreProperty]
        public int size { get; set; }

        [FirestoreProperty]
        public long price { get; set; }

        [FirestoreProperty]
        public long stock { get; set; }

        // Constructor mặc định để Firestore có thể deserialize
        public SizePrice() { }
    }
}
