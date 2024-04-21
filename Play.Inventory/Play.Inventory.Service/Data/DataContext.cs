using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }

    }
}