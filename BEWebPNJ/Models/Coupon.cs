using Google.Cloud.Firestore;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class Coupon
    {
        [FirestoreProperty]
        public string id { get; set; } = "";

        [FirestoreProperty]
        public int discount { get; set; } = 0;

        [FirestoreProperty]
        public string name { get; set; } = "";

        [FirestoreProperty]
        public int stock { get; set; } = 0;        
    }
}