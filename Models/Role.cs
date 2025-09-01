public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation Properties
    public ICollection<RoleClaim> RoleClaims { get; set; }
}