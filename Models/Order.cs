using System.ComponentModel.DataAnnotations.Schema;

namespace demo.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();
        [NotMapped]
        public decimal TotalAmount { get; set; }
    }
}
