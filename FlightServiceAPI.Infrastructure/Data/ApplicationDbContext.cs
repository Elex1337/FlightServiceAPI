using System.Reflection;
using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightServiceAPI.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}