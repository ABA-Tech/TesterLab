using Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Abstractions
{
    public interface IRoleService
    {
        // Gestion des rôles
        Task<Role?> GetRoleByIdAsync(string roleId);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> CreateRoleAsync(string name, string? description = null);
        Task DeleteRoleAsync(string roleId);

        // Attribution de rôles
        Task AssignRoleToUserAsync(string userId, string roleName, string? assignedBy = null);
        Task RemoveRoleFromUserAsync(string userId, string roleName);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string roleName);

        // Gestion en masse
        Task AssignRolesToUserAsync(string userId, List<string> roleNames, string? assignedBy = null);
        Task ReplaceUserRolesAsync(string userId, List<string> roleNames, string? assignedBy = null);
    }
}
