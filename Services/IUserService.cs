public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsers();

    Task CreateUser(string externalId, string email);

    Task AssignUserRole(string email, string roleName);
}