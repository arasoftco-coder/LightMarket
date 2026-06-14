using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LampEcommerce.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Build configuration from appsettings.json
        var basePath = Directory.GetCurrentDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();
        
        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost;Database=LampEcommerceDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;";
        
        optionsBuilder.UseSqlServer(connectionString);
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
