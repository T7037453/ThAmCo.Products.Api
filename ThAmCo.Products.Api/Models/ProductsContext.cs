using System;
using Microsoft.EntityFrameworkCore;

namespace ThAmCo.Products.Api.Models
{
    public class ProductsContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null;

        public ProductsContext(DbContextOptions<ProductsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(x =>
            {
                x.Property(p => p.Id).IsRequired();
                x.Property(p => p.Name).IsRequired();

            });
        }
    }
}
