using Microsoft.EntityFrameworkCore;
using NagypapaHazai.API.Data;

var builder = WebApplication.CreateBuilder(args);

// DbContext regisztráció (SQLite használata)
builder.Services.AddDbContext<NagypapaHazaiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS beállítása, hogy a Blazor app hívhassa a végpontokat
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:44369", "http://localhost:5000") // Ide a Blazor appod URL-jét tedd!
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Adatbázis automatikus létrehozása (csak fejlesztési célra!)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NagypapaHazaiContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthorization();
app.MapControllers();

app.Run();