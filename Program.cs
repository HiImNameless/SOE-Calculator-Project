using Microsoft.EntityFrameworkCore;
using SOE_Calculator_Project.Data;

var builder = WebApplication.CreateBuilder(args);

// Ensure env vars are included (CreateBuilder already does, but no harm):
builder.Configuration.AddEnvironmentVariables();

// Read from ConnectionStrings section OR raw env var fallback
var cs =
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException(
        "Missing DB connection string. Set env var ConnectionStrings__DefaultConnection on the host.");

builder.Services.AddDbContext<CalculatorDbContext>(o => o.UseNpgsql(cs));

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.Cookie.Name = ".SOECalc.Session";
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.IdleTimeout = TimeSpan.FromMinutes(20);
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapGet("/_db", (IConfiguration cfg) =>
{
    var cs = cfg.GetConnectionString("DefaultConnection") ?? "(null)";
    var parts = cs.Split(';', StringSplitOptions.RemoveEmptyEntries);
    var host = parts.FirstOrDefault(p => p.TrimStart().StartsWith("Host=", StringComparison.OrdinalIgnoreCase)) ?? "Host=?";
    var port = parts.FirstOrDefault(p => p.TrimStart().StartsWith("Port=", StringComparison.OrdinalIgnoreCase)) ?? "Port=?";
    return Results.Text($"[DB] {host}; {port}");
});

app.MapGet("/_env", () =>
{
    var cfgVal = builder.Configuration.GetConnectionString("DefaultConnection") ?? "(null)";
    var envVal = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? "(null)";
    return Results.Text($"CFG(DefaultConnection)={cfgVal}\nENV(ConnectionStrings__DefaultConnection)={envVal}");
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calculator}/{action=Index}/{id?}");

app.Run();
