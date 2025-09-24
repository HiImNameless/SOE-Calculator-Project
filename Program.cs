using Microsoft.EntityFrameworkCore;
using SOE_Calculator_Project.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Brandon Lombaard 223021599 
// For connecting the application to the pgsql database
builder.Services.AddDbContext<CalculatorDbContext>(options =>
{
    options.UseNpgsql("User ID=postgres;Password=!#!65953Mvz!#!;Host=localhost;Port=5432;Database=CalculatorDB;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=20;");
});

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CalculatorDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
 ));

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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
