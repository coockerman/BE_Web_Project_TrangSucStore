using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace BEWebPNJ.Models
{
    [FirestoreData]
    public class Order
    {
        [FirestoreProperty]
        public string id { get; set; } = string.Empty;

        [FirestoreProperty]
        public List<productItem> productItem { get; set; } = new();

        [FirestoreProperty]
        public string status { get; set; } = "pending"; // pending, confirmed, shipped, completed, canceled

        [FirestoreProperty]
        public string typePayment { get; set; } = "cash"; // cash, credit_card, e-wallet

        [FirestoreProperty]
        public double totalAmount { get; set; } =0;

        [FirestoreProperty]
        public string idUserOrder { get; set; } = string.Empty;

        [FirestoreProperty]
        public double couponDiscount { get; set; } = 0.0;

        [FirestoreProperty]
        public DateTime timeOrder { get; set; } = DateTime.UtcNow;
    }

    [FirestoreData]
    public class productItem
    {
        [FirestoreProperty]
        public string idProductNow { get; set; } = string.Empty;

        [FirestoreProperty]
        public string name { get; set; } = string.Empty;

        [FirestoreProperty]
        public int stock { get; set; }

        [FirestoreProperty]
        public string size { get; set; } = string.Empty;

        [FirestoreProperty]
        public double price { get; set; }

        [FirestoreProperty]
        public string gender { get; set; } = "unisex"; // male, female, unisex

        [FirestoreProperty]
        public string category { get; set; } = string.Empty; // ví dụ: nhẫn, vòng tay, dây chuyền

        [FirestoreProperty]
        public string description { get; set; } = string.Empty;

        [FirestoreProperty]
        public string imgURL { get; set; } = string.Empty;
    }
}
