

using Microsoft.EntityFrameworkCore;

namespace ChessAPI.Models
{
    [Keyless]
    public class Order {
        
        public string userName { get; set; }
        public Int64 productId { get; set; }
        
    }
}