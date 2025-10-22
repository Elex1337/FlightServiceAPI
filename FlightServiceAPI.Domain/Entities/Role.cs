namespace FlightServiceAPI.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Code  { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}