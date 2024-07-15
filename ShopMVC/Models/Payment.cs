using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopMVC.Models {
    public class Payment {
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("count")]
        public int Count { get; set; }
    }
}
