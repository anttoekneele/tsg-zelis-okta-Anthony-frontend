using FASAD.UI.GraphQL;

public class UserService : IUserService
{
    private readonly FASADClient _client;

    public UserService(FASADClient client)
    {
        _client = client;
    }
    
    public async Task<IEnumerable<UserDto>> GetUsers()
    {
        var results = await _client.GetUsers.ExecuteAsync();

        var users = results.Data?.Users;

        if (users != null)
        {
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                RoleName = u.RoleName
            });
        }

        return Enumerable.Empty<UserDto>();
    }

    public async Task CreateUser(string externalId, string email)
    {
        await _client.CreateUser.ExecuteAsync(externalId, email);
    }

    public async Task AssignUserRole(string email, string roleName)
    {
        await _client.AssignUserRole.ExecuteAsync(email, roleName);
    }
}