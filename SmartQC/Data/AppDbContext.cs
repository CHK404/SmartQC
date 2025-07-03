using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore;

namespace SmartQC.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserData> UserData { get; set; }
        public DbSet<ProductData> ProductData { get; set; }
        public DbSet<ProductDetail> ProductDetail { get; set; }
        public DbSet<Error> Error { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder opts)
        {
            if (!opts.IsConfigured)
            {
                opts.UseMySQL(
                    "Server=ip-172-31-42-81;Port=3306;Database=SmartQC;User=dbchk;Password=codingon2751;SslMode=Preferred",
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure()
                );
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserData>().ToTable("UserData");
            builder.Entity<ProductData>().ToTable("ProductData");
            builder.Entity<ProductDetail>().ToTable("ProductDetail");
            builder.Entity<Error>().ToTable("Error");
        }
    }
}
