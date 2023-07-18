using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineGameStore.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace OnlineGameStore.Data
{
    public class OnlineGameStoreContext : IdentityDbContext<ApplicationUser>
    {
        public OnlineGameStoreContext (DbContextOptions<OnlineGameStoreContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<OnlineGameStore.Models.Game> Game { get; set; }
        public DbSet<OnlineGameStore.Models.AuditRecord> AuditRecords { get; set; }
        public DbSet<OnlineGameStore.Models.Customer> Customers { get; set; }
    }
}
