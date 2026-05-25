using Microsoft.EntityFrameworkCore;
using ReserveTicketsWebApp.Domain;

namespace ReserveTicketsWebApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>(e =>
        {
            e.HasKey(ev => ev.Id);
            e.Property(ev => ev.Title).IsRequired().HasMaxLength(200);
            e.Property(ev => ev.Id).ValueGeneratedOnAdd();
            e.Property(ev => ev.RowVersion).IsConcurrencyToken();
            e.Property(ev => ev.BasePrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Ticket>(e =>
        {
           e.HasKey(t => t.Id);
           e.Property(t => t.Id).ValueGeneratedOnAdd();
           e.Property(t => t.Status).HasConversion<string>();
           e.Property(t => t.Price).HasPrecision(18, 2);
        });
    }

    public DbSet<Event> Events { get; set; } = default!;

    public DbSet<Ticket> Tickets { get; set; } = default!;
}