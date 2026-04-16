using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NagypapaHazai.Shared.MODELS;
using NagypapaHazai.API.Data;

namespace NagypapaHazai.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly NagypapaHazaiContext _context;

        public PropertiesController(NagypapaHazaiContext context)
        {
            _context = context;
        }

        // Összes ház lekérdezése (GET: api/Properties)
        [HttpGet]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _context.Properties.ToListAsync();
            return Ok(properties);
        }

        // Egy konkrét ház lekérdezése ID alapján (GET: api/Properties/5)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound("Nincs ilyen ház.");

            return Ok(property);
        }

        // Új ház hozzáadása (POST: api/Properties)
        [HttpPost]
        public async Task<IActionResult> CreateProperty(Property property)
        {
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return Ok(property);
        }

        // Ház törlése (DELETE: api/Properties/5)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return Ok("Ház sikeresen törölve.");
        }
    }
}