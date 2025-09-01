public class Claim
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }

    // Navigation Properties
    public ICollection<RoleClaim> RoleClaims { get; set; }
}