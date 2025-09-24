
// put this in your web project for a quick debug, then delete after
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SOE_Calculator_Project.Data;

public class DesignFactory : IDesignTimeDbContextFactory<CalculatorDbContext>
{
    public CalculatorDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<DesignFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("DefaultConnection");
        Console.WriteLine($"[EF Design] ConnectionString = {cs}");

        var opts = new DbContextOptionsBuilder<CalculatorDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new CalculatorDbContext(opts);
    }
}
