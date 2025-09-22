using Microsoft.EntityFrameworkCore;
using SOE_Calculator_Project.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Brandon Lombaard 223021599 
// For connecting the application to the pgsql database
builder.Services.AddDbContext<CalculatorDbContext>(options =>
{
    options.UseNpgsql("User ID=postgres;Password=65953Mvz;Host=localhost;Port=5432;Database=CalculatorDB;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=20;");
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
