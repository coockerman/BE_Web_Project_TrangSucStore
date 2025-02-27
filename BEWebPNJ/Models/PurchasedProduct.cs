namespace BEWebPNJ.Models
{
    public class PurchasedProduct
    {
        public string id { get; set; } = string.Empty;
        public List<string> purchasedCart { get; set; } = new();
    }
}
