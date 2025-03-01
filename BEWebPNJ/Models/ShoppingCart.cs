using System;
using System.Collections.Generic;

namespace BEWebPNJ.Models
{
    public class ShoppingCartt
    {
        public string id { get; set; } = string.Empty;

        // Định nghĩa kiểu Dictionary thay vì List để phù hợp với Map<string, dynamic>
        public Dictionary<string, Dictionary<string, object>> shoppingCart { get; set; } = new();
    }
}
