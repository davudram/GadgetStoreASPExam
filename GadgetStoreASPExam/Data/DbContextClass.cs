using GadgetStoreASPExam.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GadgetStoreASPExam.Data
{
    public class DbContextClass : IdentityDbContext<IdentityUser>
    {
        protected readonly IConfiguration Configuration;
        public DbContextClass(DbContextOptions configuration) : base(configuration)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Gadget> Gadgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }
}
