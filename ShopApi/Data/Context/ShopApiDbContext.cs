using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data.Models;

namespace ShopApi.Data.Context
{
    public class ShopApiDbContext : IdentityDbContext<ShopUser>
    {
        public ShopApiDbContext(DbContextOptions<ShopApiDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Product> Products { get; set; } = null!;
    }
}
