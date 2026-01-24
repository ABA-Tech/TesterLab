using Auth.Core.Abstractions;
using Auth.Core.Models;
using Microsoft.EntityFrameworkCore;
using TesterLab.Data;

namespace TesterLab.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    //public async Task<ApplicationUser?> GetByIdAsync(string userId)
    //{
    //  return await _context.Users.FindAsync(userId);
    //}

    //public async Task<ApplicationUser?> GetByUsernameAsync(string username)
    //{
    //  return await _context.Users
    //      .FirstOrDefaultAsync(u => u.Username == username);
    //}

    //public async Task<ApplicationUser?> GetByEmailAsync(string email)
    //{
    //  return await _context.Users
    //      .FirstOrDefaultAsync(u => u.Email == email);
    //}

    public async Task<ApplicationUser?> GetByPasswordResetTokenAsync(string token)
    {
      return await _context.Users
          .FirstOrDefaultAsync(u => u.PasswordResetToken == token);
    }

    public async Task<ApplicationUser?> GetByEmailConfirmationTokenAsync(string token)
    {
      return await _context.Users
          .FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
    }

    public async Task<ApplicationUser> CreateAsync(ApplicationUser user)
    {
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return user;
    }

    public async Task UpdateAsync(ApplicationUser user)
    {
      _context.Users.Update(user);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string userId)
    {
      var user = await GetByIdAsync(userId);
      if (user != null)
      {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
      }
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
      return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
      return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<ApplicationUser?> GetByIdAsync(string userId)
    {
      return await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username)
    {
      return await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
      return await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<ApplicationUser>> GetAllAsync()
    {
      return await _context.Users.ToListAsync();
    }
  }
}
