using FlightServiceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightServiceAPI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Flight> Flights { get; }
    DbSet<Role> Roles { get; }
    DbSet<User>  Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}