using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NagypapaHazai.Shared.MODELS;
using NagypapaHazai.API.Data;

namespace NagypapaHazai.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly NagypapaHazaiContext _context;

        public BookingsController(NagypapaHazaiContext context)
        {
            _context = context;
        }

        // Egy adott felhasználó foglalásainak lekérdezése (GET: api/Bookings/user/1)
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Property) // Húzzuk be a ház adatait is a foglaláshoz
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return Ok(bookings);
        }

        // Összes foglalás (Adminoknak) (GET: api/Bookings)
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.User)
                .ToListAsync();

            return Ok(bookings);
        }

        // Új foglalás leadása (POST: api/Bookings)
        [HttpPost]
        public async Task<IActionResult> CreateBooking(Booking booking)
        {
            // Ellenőrizzük, hogy létezik-e a ház és a user
            var userExists = await _context.Users.AnyAsync(u => u.Id == booking.UserId);
            var propertyExists = await _context.Properties.AnyAsync(p => p.Id == booking.PropertyId);

            if (!userExists || !propertyExists)
            {
                return BadRequest("Hibás felhasználó vagy ház azonosító!");
            }

            booking.Status = "Függőben"; // Alapértelmezett státusz
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }
    }
}