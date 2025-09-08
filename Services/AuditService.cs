using FASAD.UI.GraphQL;

public class AuditService : IAuditService
{
    private readonly FASADClient _client;

    public AuditService(FASADClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<SecurityEventDto>> GetSecurityEvents()
    {
        var results = await _client.GetSecurityEvents.ExecuteAsync();

        var securityEvents = results.Data?.SecurityEvents;

        if (securityEvents != null)
        {
            return securityEvents.Select(se => new SecurityEventDto
            {
                Id = se.Id,
                EventType = se.EventType,
                OccurredUtc = se.OccurredUtc,
                Details = se.Details,
                AuthorUserEmail = se.AuthorUserEmail,
                AffectedUserEmail = se.AffectedUserEmail
            });
        }

        return Enumerable.Empty<SecurityEventDto>();
    }

    public async Task LoginSuccessEvent(string email, string provider)
    {
        await _client.LoginSuccessEvent.ExecuteAsync(email, provider);
    }

    public async Task LogoutEvent(string email)
    {
        await _client.LogoutEvent.ExecuteAsync(email);
    }

    public async Task RoleAssignedEvent(string authorUserEmail, string affectedUserEmail, string fromRoleName, string toRoleName)
    {
        await _client.RoleAssignedEvent.ExecuteAsync(authorUserEmail, affectedUserEmail, fromRoleName, toRoleName);
    }
}