public class RoleClaim
{
    public Guid RoleId { get; set; }
    public Guid ClaimId { get; set; }

    // Navigation Properties
    public Role Role { get; set; }
    public Claim Claim { get; set; }
}