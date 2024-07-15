using Microsoft.EntityFrameworkCore;
using Practice;
using ShopMVC.Models;

namespace ShopMVC {

    public class ApplicationContext: DbContext {
        public DbSet<Person> person { get; set; }
        public DbSet<Payment> payment { get; set; }
        public DbSet<Order> order { get; set; }
        public DbSet<Product> product { get; set; }

        public ApplicationContext() =>
            Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql("Host=192.168.98.78;Port=5433;Database=postgres;Username=postgres;Password=practice");
    }

}
