// Data/NagypapaContext.cs
using Microsoft.EntityFrameworkCore;
using NagypapaHazaiBlazor.MODELS;

public class NagypapaContext : DbContext
{
    public NagypapaContext(DbContextOptions<NagypapaContext> options) : base(options) { }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>()
            .HasOne(e => e.Property)
            .WithMany(p => p.Events)
            .HasForeignKey(e => e.PropertyId);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Property)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PropertyId);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Event)
            .WithMany(e => e.Bookings)
            .HasForeignKey(b => b.EventId)
            .IsRequired(false);

        modelBuilder.Entity<Event>()
            .HasIndex(e => e.PropertyId)
            .HasDatabaseName("IX_Events_PropertyId");

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.PropertyId)
            .HasDatabaseName("IX_Bookings_PropertyId");

        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.StartDate, b.EndDate })
            .HasDatabaseName("IX_Bookings_Date");
    }
}
