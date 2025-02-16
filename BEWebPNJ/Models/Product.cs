using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class Product
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string NameProduct { get; set; }

        [FirestoreProperty]
        public string Gender { get; set; }

        [FirestoreProperty]
        public List<SizePrice> SizePrice { get; set; }

        [FirestoreProperty]
        public List<string> ListEvaluation { get; set; } = new List<string>();

        [FirestoreProperty]
        public List<string> ProductImg { get; set; }

        [FirestoreProperty]
        public string Type { get; set; }

        [FirestoreProperty]
        public string Category { get; set; }

        [FirestoreProperty]
        public string Material { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }
    }

    [FirestoreData]
    public class SizePrice
    {
        [FirestoreProperty]
        public int Size { get; set; }

        [FirestoreProperty]
        public long Price { get; set; }

        // Constructor mặc định để Firestore có thể deserialize
        public SizePrice() { }
    }
}
