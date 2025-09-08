public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetRoles();
}