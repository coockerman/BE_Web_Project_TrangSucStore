namespace BEWebPNJ.Models
{
    public class ListPaymentUser
    {
        public string id { get; set; } = string.Empty;
        public List<string> listPaymentId { get; set; } = new();
    }
}
