using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

public class InventoryContext : DbContext
{
    public InventoryContext(DbContextOptions<InventoryContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
}


   