using NagypapaHazai.Web.Data; // VAGY ami nálad a DbContext namespace-e
using NagypapaHazaiBlazor;
using NagypapaHazaiBlazor.MODELS; // VAGY ami nálad a Model namespace-e

namespace NagypapaHazai.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(NagypapaContext context)
        {
            context.Database.EnsureCreated();

            // Ha már van ingatlan, nem csinálunk semmit
            if (context.Properties.Any())
            {
                return;
            }

            var properties = new Property[]
            {
                new Property{Name="Faház a Bakonyban", Location="Bakonybél", Description="Hangulatos faház az erdő szélén.", Capacity=10, Status="available", ImageUrl="https://images.unsplash.com/photo-1518780664697-55e3ad937233", CreatedAt=DateTime.Now},
                new Property{Name="Balatoni Nyaraló", Location="Balatonalmádi", Description="Közvetlen vízparti nyaraló saját stéggel.", Capacity=6, Status="available", ImageUrl="https://images.unsplash.com/photo-1499793983690-e29da59ef1c2", CreatedAt=DateTime.Now},
                new Property{Name="Pinceház Villányban", Location="Villány", Description="Borospince feletti vendégház.", Capacity=4, Status="maintenance", ImageUrl="https://images.unsplash.com/photo-1510798831971-661eb04b3739", CreatedAt=DateTime.Now}
            };

            context.Properties.AddRange(properties);
            context.SaveChanges();

            var events = new Event[]
            {
                new Event{Title="Fafaragó Workshop", Description="Kezdő fafaragó tanfolyam.", StartDate=DateTime.Now.AddDays(10), EndDate=DateTime.Now.AddDays(12), MaxParticipants=8, PropertyId=properties[0].Id, CreatedAt=DateTime.Now},
                new Event{Title="Jóga Tábor", Description="Hétvégi elvonulás.", StartDate=DateTime.Now.AddDays(20), EndDate=DateTime.Now.AddDays(22), MaxParticipants=15, PropertyId=properties[1].Id, CreatedAt=DateTime.Now}
            };

            context.Events.AddRange(events);
            context.SaveChanges();
        }
    }
}
