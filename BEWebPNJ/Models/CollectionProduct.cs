using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class CollectionProduct
    {
        [FirestoreProperty]
        public string id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string name { get; set; } = string.Empty;
        [FirestoreProperty]
        public string urlImage { get; set; } = string.Empty;
        [FirestoreProperty]
        public List<string> listProduct { get; set; } = new List<string>();
    }
}
