using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NagypapaHazai.API.Data;
using NagypapaHazai.API.DTOs; // Ha máshol vannak a DTO-k, ezt írd át!
using NagypapaHazai.Shared.MODELS;

namespace NagypapaHazai.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly NagypapaHazaiContext _context;

        public AuthController(NagypapaHazaiContext context)
        {
            _context = context;
        }

        // Regisztráció (POST: api/Auth/register)
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Ellenőrzés: Foglalt-e az email?
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("Ez az email cím már foglalt.");
            }

            var newUser = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Role = "user", // Alapértelmezetten sima user
                // A jelszót biztonságosan hashelve mentjük el!
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("Sikeres regisztráció!");
        }

        // Bejelentkezés (POST: api/Auth/login)
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Ha nincs ilyen user, vagy a jelszó hash nem egyezik
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Hibás email vagy jelszó!");
            }

            // Később ide jön majd a JWT Token generálás, egyelőre csak visszaadjuk a user adatait (jelszó nélkül)
            user.PasswordHash = "";
            return Ok(user);
        }
    }
}