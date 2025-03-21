namespace BEWebPNJ.Models
{
    public class User
    {
        public string id { get; set; } = string.Empty;
        public List<string> favouriteCart { get; set; } = new();
        public List<string> purchasedCart { get; set; } = new();
        public string fullName { get; set; } = string.Empty;
        public string sex { get; set; } = "Nam";
        public string email { get; set; } = string.Empty;
        public string numberPhone { get; set; } = string.Empty;
        public string role { get; set; } = "user";

        public User() { }
    }
}
