using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightServiceAPI.Infrastructure.Data;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ApplicationDbContextInitializer(IPasswordHasher passwordHasher, ApplicationDbContext context)
    {
        _passwordHasher = passwordHasher;
        _context = context;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
            if (await _context.Roles.AnyAsync())
            {
                return;
            }

            await SeedRolesAsync();
            await SeedUsersAsync();

        }
        catch (Exception ex)
        {
            throw;
        }
    }
    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new Role { Code = "Moderator" },
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();

    }

    private async Task SeedUsersAsync()
    {
        var moderatorRole = await _context.Roles.FirstAsync(r => r.Code == "Moderator");

        var users = new[]
        {
            new User
            {
                Username = "admin",
                Password = _passwordHasher.HashPassword("12345678"),
                RoleId = moderatorRole.Id
            },
            new User
            {
                Username = "user",
                Password = _passwordHasher.HashPassword("12345678"),
                RoleId = null
            }
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }
}