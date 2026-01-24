using Auth.Core.Abstractions;
using Auth.Core.Models;
using Microsoft.EntityFrameworkCore;
using TesterLab.Data;

namespace TesterLab.Repositories
{
  public class RoleRepository : IRoleRepository
  {
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Role?> GetByIdAsync(string roleId)
    {
      return await _context.Roles
          .Include(r => r.UserRoles)
          .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
      return await _context.Roles
          .Include(r => r.UserRoles)
          .FirstOrDefaultAsync(r => r.Name == roleName);
    }

    public async Task<List<Role>> GetAllAsync()
    {
      return await _context.Roles.ToListAsync();
    }

    public async Task<Role> CreateAsync(Role role)
    {
      _context.Roles.Add(role);
      await _context.SaveChangesAsync();
      return role;
    }

    public async Task UpdateAsync(Role role)
    {
      _context.Roles.Update(role);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string roleId)
    {
      var role = await GetByIdAsync(roleId);
      if (role != null)
      {
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
      }
    }

    public async Task AddUserRoleAsync(UserRole userRole)
    {
      _context.UserRoles.Add(userRole);
      await _context.SaveChangesAsync();
    }

    public async Task RemoveUserRoleAsync(string userId, string roleId)
    {
      var userRole = await _context.UserRoles
          .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

      if (userRole != null)
      {
        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
      }
    }

    public async Task RemoveAllUserRolesAsync(string userId)
    {
      var userRoles = await _context.UserRoles
          .Where(ur => ur.UserId == userId)
          .ToListAsync();

      _context.UserRoles.RemoveRange(userRoles);
      await _context.SaveChangesAsync();
    }

    public async Task<bool> UserHasRoleAsync(string userId, string roleId)
    {
      return await _context.UserRoles
          .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<List<string>> GetUserRoleNamesAsync(string userId)
    {
      return await _context.UserRoles
          .Where(ur => ur.UserId == userId)
          .Include(ur => ur.Role)
          .Select(ur => ur.Role.Name)
          .ToListAsync();
    }

    public async Task<List<Role>> GetUserRolesAsync(string userId)
    {
      return await _context.UserRoles
          .Where(ur => ur.UserId == userId)
          .Include(ur => ur.Role)
          .Select(ur => ur.Role)
          .ToListAsync();
    }
  }
}
