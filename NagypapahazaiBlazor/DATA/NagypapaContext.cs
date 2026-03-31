// Data/NagypapaContext.cs
using Microsoft.EntityFrameworkCore;
using NagypapaHazaiBlazor.MODELS;

namespace NagypapaHazaiBlazor.Data
{
    public class NagypapaContext : DbContext
    {
        public NagypapaContext(DbContextOptions<NagypapaContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User → Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); // user törlésénél ne törölje a bookingokat

            // Property → Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Property → Events
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Property)
                .WithMany(p => p.Events)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexek a gyorsabb lekérdezéshez
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
}
