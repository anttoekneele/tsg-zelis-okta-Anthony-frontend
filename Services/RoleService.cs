using FASAD.UI.GraphQL;

public class RoleService : IRoleService
{
    private readonly FASADClient _client;

    public RoleService(FASADClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<RoleDto>> GetRoles()
    {
        var results = await _client.GetRoles.ExecuteAsync();

        var roles = results.Data?.Roles;

        if (roles != null)
        {
            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            });
        }

        return Enumerable.Empty<RoleDto>();
    }
}