namespace BEWebPNJ.Models
{
    public class User
    {
        public string id { get; set; } = string.Empty;
        public Dictionary<string, AddAddress> addAddress { get; set; } = new();
        public Dictionary<string, string> favouriteCart { get; set; } = new();
        public Dictionary<string, string> purchasedCart { get; set; } = new();
        public Dictionary<string, ShoppingCart> shoppingCart { get; set; } = new();
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string gender { get; set; } = "Nam";
        public string email { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string role { get; set; } = "user";
        public string timeRegister { get; set; } = string.Empty;

        public User() { }
    }

    public class AddAddress
    {
        public string city { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
        public string street { get; set; } = string.Empty;
        public AddAddress() { }
    }
    public class ShoppingCart
    {
        public string idProduct { get; set; } = string.Empty;
        public int stock { get; set; } = 0;
    }
}
