using System;
using System.Collections.Generic;

namespace BEWebPNJ.Models
{
    public class ShoppingCart
    {
        public string id { get; set; } = string.Empty;
        public string idProduct { get; set; } = string.Empty;
        public int size { get; set; } = 0;
        public int price { get; set; } = 0;

        
    }
}
