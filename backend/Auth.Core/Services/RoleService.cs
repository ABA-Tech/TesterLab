using Auth.Core.Abstractions;
using Auth.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Role?> GetRoleByIdAsync(string roleId)
        {
            return await _roleRepository.GetByIdAsync(roleId);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _roleRepository.GetByNameAsync(roleName);
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> CreateRoleAsync(string name, string? description = null)
        {
            var existingRole = await _roleRepository.GetByNameAsync(name);
            if (existingRole != null)
                throw new InvalidOperationException($"Le rôle '{name}' existe déjà");

            var role = new Role
            {
                Name = name,
                Description = description
            };

            await _roleRepository.CreateAsync(role);
            _logger.LogInformation("Rôle créé: {RoleName}", name);

            return role;
        }

        public async Task DeleteRoleAsync(string roleId)
        {
            await _roleRepository.DeleteAsync(roleId);
            _logger.LogInformation("Rôle supprimé: {RoleId}", roleId);
        }

        public async Task AssignRoleToUserAsync(string userId, string roleName, string? assignedBy = null)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Utilisateur non trouvé");

            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
                throw new InvalidOperationException($"Rôle '{roleName}' non trouvé");

            // Vérifier si l'utilisateur a déjà ce rôle
            if (await _roleRepository.UserHasRoleAsync(userId, role.Id))
                return; // Déjà assigné

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = role.Id,
                AssignedBy = assignedBy
            };

            await _roleRepository.AddUserRoleAsync(userRole);
            _logger.LogInformation("Rôle '{RoleName}' assigné à l'utilisateur {UserId}", roleName, userId);
        }

        public async Task RemoveRoleFromUserAsync(string userId, string roleName)
        {
            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
                return;

            await _roleRepository.RemoveUserRoleAsync(userId, role.Id);
            _logger.LogInformation("Rôle '{RoleName}' retiré de l'utilisateur {UserId}", roleName, userId);
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            return await _roleRepository.GetUserRoleNamesAsync(userId);
        }

        public async Task<bool> IsInRoleAsync(string userId, string roleName)
        {
            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
                return false;

            return await _roleRepository.UserHasRoleAsync(userId, role.Id);
        }

        public async Task AssignRolesToUserAsync(string userId, List<string> roleNames, string? assignedBy = null)
        {
            foreach (var roleName in roleNames)
            {
                await AssignRoleToUserAsync(userId, roleName, assignedBy);
            }
        }

        public async Task ReplaceUserRolesAsync(string userId, List<string> roleNames, string? assignedBy = null)
        {
            // Supprimer tous les rôles existants
            await _roleRepository.RemoveAllUserRolesAsync(userId);

            // Assigner les nouveaux rôles
            await AssignRolesToUserAsync(userId, roleNames, assignedBy);

            _logger.LogInformation("Rôles remplacés pour l'utilisateur {UserId}", userId);
        }
    }
}
