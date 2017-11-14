using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Market.Portal.Models;

namespace Market.Portal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<News> News { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureNews(builder);
        }


        private void ConfigureNews(ModelBuilder builder)
        {
            builder.Entity<News>(entity =>
            {
                entity.ToTable("tb_news");
                entity.HasKey(n => n.Id).HasName("id");
                entity.Property(n => n.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(n => n.Description).HasColumnName("description").HasMaxLength(200).IsRequired();
                entity.Property(n => n.Content).HasColumnName("content").HasMaxLength(1000).IsRequired();
                entity.Property(n => n.DtCreation).HasColumnName("dt_creation").HasDefaultValueSql("getdate()");
            });
        }
    }
}
