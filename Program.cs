using Microsoft.EntityFrameworkCore;
using SOE_Calculator_Project.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Single source of truth for connection string
builder.Services.AddDbContext<CalculatorDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

app.MapGet("/_db", async (IConfiguration cfg, IServiceProvider sp) =>
{
    var cs = cfg.GetConnectionString("DefaultConnection") ?? "(null)";
    var parts = cs.Split(';', StringSplitOptions.RemoveEmptyEntries);
    var host = parts.FirstOrDefault(p => p.StartsWith("Host=", StringComparison.OrdinalIgnoreCase)) ?? "Host=?";
    var port = parts.FirstOrDefault(p => p.StartsWith("Port=", StringComparison.OrdinalIgnoreCase)) ?? "Port=?";
    try
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SOE_Calculator_Project.Data.CalculatorDbContext>();
        var ok = await db.Database.CanConnectAsync();
        return Results.Text($"[DB] {host}; {port}; CanConnect={ok}");
    }
    catch (Exception ex)
    {
        return Results.Text($"[DB] {host}; {port}; CanConnect=FALSE; {ex.GetType().Name}: {ex.Message}");
    }
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calculator}/{action=Index}/{id?}");

app.Run();
