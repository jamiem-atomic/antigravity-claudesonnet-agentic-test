using Microsoft.EntityFrameworkCore;
using VehicleMarketplace.Api.Data;
using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Tests;

public class TestBase : IDisposable
{
    protected readonly ApplicationDbContext _context;

    public TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    protected async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new User
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                DisplayName = "Test User",
                IsAdmin = false
            },
            new User
            {
                Id = 2,
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                DisplayName = "Admin User",
                IsAdmin = true
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }
}
