using Google.Cloud.Firestore;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class Coupon
    {
        [FirestoreProperty]
        public string id { get; set; }

        [FirestoreProperty]
        public int discount { get; set; }

        [FirestoreProperty]
        public string name { get; set; }

        [FirestoreProperty]
        public int stock { get; set; }        
    }
}