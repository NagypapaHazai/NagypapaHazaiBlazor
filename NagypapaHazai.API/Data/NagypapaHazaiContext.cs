using Microsoft.EntityFrameworkCore;
using NagypapaHazai.Shared.MODELS;

namespace NagypapaHazai.API.Data
{
    public class NagypapaHazaiContext : DbContext
    {
        public NagypapaHazaiContext(DbContextOptions<NagypapaHazaiContext> options) : base(options)
        {
        }

        // Itt vannak az adatbázis táblák (A Blazor hiba miatt hozzáadva az Events és EventRegistrations is!)
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. ADMIN USER SEED (Fix jelszó hash-el)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserName = "Admin",
                    Email = "admin@nagypapahazai.hu",
                    Role = "admin",
                    // Ez a hash az "admin123" jelszót jelenti! Fix érték, így az EF Core nem fog panaszkodni.
                    PasswordHash = "$2a$11$sQTbILtZbCELlsh/GEqZtOewFM0K0Ijk1VWokeEM0tEF.Ia3dDJjG"
                }
            );

            // 2. INGATLANOK SEED (A 3 eredeti házad)
            modelBuilder.Entity<Property>().HasData(
                new Property
                {
                    Id = 1,
                    Name = "Faház a Bakonyban",
                    Location = "Bakony",
                    Description = "Csendes kis faház az erdő közepén.",
                    Capacity = 10,
                    Status = "Aktív",
                    PricePerNight = 25000,
                    ImageUrl = "",
                    ExtraInfo = "",
                    CreatedAt = new DateTime(2026, 1, 1) // Fix dátum kell az EF Core warning elkerüléséhez!
                },
                new Property
                {
                    Id = 2,
                    Name = "Vízparti Nyaraló",
                    Location = "Balatonlelle",
                    Description = "Közvetlen vízparti élmény.",
                    Capacity = 6,
                    Status = "Aktív",
                    PricePerNight = 35000,
                    ImageUrl = "",
                    ExtraInfo = "",
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Property
                {
                    Id = 3,
                    Name = "Villányi Vendégház",
                    Location = "Villány",
                    Description = "Hangulatos borház a dűlők között.",
                    Capacity = 4,
                    Status = "Aktív",
                    PricePerNight = 20000,
                    ImageUrl = "",
                    ExtraInfo = "",
                    CreatedAt = new DateTime(2026, 1, 1)
                }
            );
        }
    }
}