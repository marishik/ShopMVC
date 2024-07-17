namespace ShopMVC.Models
{
    public class SomeModel
    {
        public int OrderId { get; set; }
        public List<Practice.Client.Order> Orders { get; set; }
        public List<Practice.Client.Payment> Payments { get; set; }
        public List<Practice.Client.Person> Persons { get; set; }
        public List<Practice.Client.Product> Products { get; set; }
    }
}
