namespace BEWebPNJ.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public string Title { get; set; } = string.Empty;

        public User() { }
    }
}
