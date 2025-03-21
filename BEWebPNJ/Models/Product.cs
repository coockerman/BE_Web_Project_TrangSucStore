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
        public string nameProduct { get; set; } = "";

        [FirestoreProperty]
        public string gender { get; set; } = "Nam";

        [FirestoreProperty]
        public List<SizePrice> sizePrice { get; set; } = new List<SizePrice>()
        {
            new SizePrice()
        };

        [FirestoreProperty]
        public List<string> listEvaluation { get; set; } = new List<string>()
        {
            "none"
        };

        [FirestoreProperty]
        public List<string> listComments { get; set; } = new List<string>();

        [FirestoreProperty]
        public List<string> productImg { get; set; } = new List<string>()
        {
            "none"
        };

        [FirestoreProperty]
        public string type { get; set; } = "";

        [FirestoreProperty]
        public string category { get; set; } = "Trang suc";

        [FirestoreProperty]
        public string material { get; set; } = "";

        [FirestoreProperty]
        public string description { get; set; }= "";

        [FirestoreProperty]
        public string karat { get; set; } = "";

        [FirestoreProperty]
        public string show { get; set; } = "true";


    }

    [FirestoreData]
    public class SizePrice
    {
        [FirestoreProperty]
        public int size { get; set; } = 0;

        [FirestoreProperty]
        public long price { get; set; } = 0;

        [FirestoreProperty]
        public long stock { get; set; } = 0;

        // Constructor mặc định để Firestore có thể deserialize
        public SizePrice() {
            
        }

        
    }
}
