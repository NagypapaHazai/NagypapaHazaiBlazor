using Microsoft.EntityFrameworkCore;
using NagypapaHazaiBlazor.Data;

namespace NagypapaHazaiBlazor.Data
{
    public static class EventDateFixer
    {
        public static async Task ShiftEventsToTodayAsync(NagypapaContext db)
        {
            var events = await db.Events.ToListAsync();
            if (!events.Any()) return;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var random = new Random();

            // Minden esemény kap egy egyedi eltolást (0, 7, 14, 21... nap)
            int offset = 0;

            foreach (var ev in events)
            {
                if (ev.StartDate == null || ev.EndDate == null) continue;

                // Az esemény hosszát megőrizzük
                var duration = ev.EndDate.Value.DayNumber - ev.StartDate.Value.DayNumber;

                // Mindenki más napra kerül (0, 5-15 nap különbséggel egymástól)
                ev.StartDate = today.AddDays(offset);
                ev.EndDate = today.AddDays(offset + duration);

                // A következő esemény 5-15 nappal később kezdődik
                offset += random.Next(5, 15);
            }

            await db.SaveChangesAsync();
        }
    }
}
