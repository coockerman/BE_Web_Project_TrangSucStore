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
        public List<productItem> productItems { get; set; } = new();

        [FirestoreProperty]
        public string status { get; set; } = "process"; //process, pending, confirmed, completed

        [FirestoreProperty]
        public string typePayment { get; set; } = "cash"; // cash, credit_card, e-wallet, vnpay, paypal

        [FirestoreProperty]
        public string couponId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string paymentId { get; set; } = string.Empty;
        [FirestoreProperty]
        public string nameUserGet { get; set; } = string.Empty;

        [FirestoreProperty]
        public string emailUserGet { get; set; } = string.Empty;

        [FirestoreProperty]
        public string phoneUserGet { get; set; } = string.Empty;

        [FirestoreProperty]
        public string addressUserGet { get; set; } = string.Empty;

        [FirestoreProperty]
        public double totalAmount { get; set; } = 0;

        [FirestoreProperty]
        public string idUserOrder { get; set; } = string.Empty;

        [FirestoreProperty]
        public double couponDiscount { get; set; } = 0;

        [FirestoreProperty]
        public DateTime timeOrder { get; set; } = DateTime.UtcNow.AddHours(7);
    }

    [FirestoreData]
    public class productItem
    {
        [FirestoreProperty]
        public string idProductNow { get; set; } = string.Empty;

        [FirestoreProperty]
        public string name { get; set; } = string.Empty;

        [FirestoreProperty]
        public int stock { get; set; } = 0;

        [FirestoreProperty]
        public int size { get; set; } = 0;

        [FirestoreProperty]
        public double price { get; set; } = 0;

        


        [FirestoreProperty]
        public string gender { get; set; } = ""; // Nam, Nu, Tre em

        [FirestoreProperty]
        public string type { get; set; } = string.Empty; 

        [FirestoreProperty]
        public string description { get; set; } = string.Empty;

        [FirestoreProperty]
        public string imgURL { get; set; } = string.Empty;
    }
}
