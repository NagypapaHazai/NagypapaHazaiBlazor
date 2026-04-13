using NagypapaHazaiBlazor.Data;
using NagypapaHazaiBlazor.MODELS;

namespace NagypapaHazaiBlazor.Data
{
    public static class DbInitializer
    {
        public static void Initialize(NagypapaContext context)
        {
            context.Database.EnsureCreated();

            if (context.Properties.Any()) return;

            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@nagypapahazai.hu",
                    // A jelszót hashelve mentjük el! Jelszó: Admin123!
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = "Admin"
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            var properties = new Property[]
            {
                new Property
                {
                    Name        = "Faház a Bakonyban",
                    Location    = "Bakonybél",
                    Description = "Hangulatos faház az erdő szélén.",
                    Capacity    = 10,
                    Status      = "available",
                    ImageUrl = "/images/bakonyiHaz.png",
                    CreatedAt   = DateTime.Now
                },
                new Property
                {
                    Name        = "Balatoni Nyaraló",
                    Location    = "Balatonalmádi",
                    Description = "Közvetlen vízparti nyaraló saját stéggel.",
                    Capacity    = 6,
                    Status      = "available",
                    ImageUrl = "/images/balatoniHaz.png",
                    CreatedAt   = DateTime.Now
                },
                new Property
                {
                    Name        = "Pinceház Villányban",
                    Location    = "Villány",
                    Description = "Borospince feletti vendégház.",
                    Capacity    = 4,
                    Status      = "maintenance",
                    ImageUrl    = "images/villanyiHaz.png",
                    CreatedAt   = DateTime.Now
                }
            };

            context.Properties.AddRange(properties);
            context.SaveChanges();

            var events = new Event[]
            {
                new Event
                {
                    Title           = "Fafaragó Workshop",
                    Description     = "Kezdő fafaragó tanfolyam.",
                    StartDate       = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),  // ✅
                    EndDate         = DateOnly.FromDateTime(DateTime.Now.AddDays(12)),  // ✅
                    MaxParticipants = 8,
                    PropertyId      = properties[0].Id,
                    CreatedAt       = DateTime.Now
                },
                new Event
                {
                    Title           = "Jóga Tábor",
                    Description     = "Hétvégi elvonulás.",
                    StartDate       = DateOnly.FromDateTime(DateTime.Now.AddDays(20)),  // ✅
                    EndDate         = DateOnly.FromDateTime(DateTime.Now.AddDays(22)),  // ✅
                    MaxParticipants = 15,
                    PropertyId      = properties[1].Id,
                    CreatedAt       = DateTime.Now
                }
            };

            context.Events.AddRange(events);
            context.SaveChanges();
        }
    }
}
