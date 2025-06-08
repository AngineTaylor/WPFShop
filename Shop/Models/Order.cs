using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public List<CartItem>? Items { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
    }


}
