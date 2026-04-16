using Microsoft.EntityFrameworkCore;
using NagypapaHazaiBlazor.Data;
using NagypapaHazaiBlazor.Components;
using NagypapaHazaiBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Szolgáltatások (Services) regisztrálása ---

builder.Services.AddDbContext<NagypapaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Session és gyorsítótár beállításai
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Saját service-ek
builder.Services.AddScoped<AuthState>();

var app = builder.Build();

// --- 2. HTTP Kérés Pipeline konfigurálása ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// A Session és Antiforgery mindig a routing/mapping elõtt kell, hogy legyen
app.UseSession();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// --- 3. Adatbázis inicializálás és módosítások induláskor ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<NagypapaContext>();

        // Adatok inicializálása (feltöltése)
        DbInitializer.Initialize(context);

        // Események dátumának frissítése
        await EventDateFixer.ShiftEventsToTodayAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Hiba történt az adatbázis induláskori beállításakor.");
    }
}

app.Run(); 