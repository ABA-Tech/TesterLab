using Auth.Core.Models;

namespace Auth.Core.Abstractions
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(string roleId);
        Task<Role?> GetByNameAsync(string roleName);
        Task<List<Role>> GetAllAsync();
        Task<Role> CreateAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(string roleId);

        // UserRole operations
        Task AddUserRoleAsync(UserRole userRole);
        Task RemoveUserRoleAsync(string userId, string roleId);
        Task RemoveAllUserRolesAsync(string userId);
        Task<bool> UserHasRoleAsync(string userId, string roleId);
        Task<List<string>> GetUserRoleNamesAsync(string userId);
        Task<List<Role>> GetUserRolesAsync(string userId);
    }
}
