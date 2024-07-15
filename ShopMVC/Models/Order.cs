using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopMVC.Models {
    public class Order {
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("person_id")]
        public int PersonId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

    }
}
