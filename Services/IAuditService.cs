public interface IAuditService
{
    Task<IEnumerable<SecurityEventDto>> GetSecurityEvents();

    Task LoginSuccessEvent(string email, string provider);

    Task LogoutEvent(string email);

    Task RoleAssignedEvent(string authorUserEmail, string affectedUserEmail, string fromRoleName, string toRoleName);
}