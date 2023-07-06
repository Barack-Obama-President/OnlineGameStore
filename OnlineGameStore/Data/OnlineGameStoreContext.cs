using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineGameStore.Models;

namespace OnlineGameStore.Data
{
    public class OnlineGameStoreContext : DbContext
    {
        public OnlineGameStoreContext (DbContextOptions<OnlineGameStoreContext> options)
            : base(options)
        {
        }

        public DbSet<OnlineGameStore.Models.Game> Game { get; set; }
        public DbSet<OnlineGameStore.Models.AuditRecord> AuditRecords { get; set; }
    }
}
