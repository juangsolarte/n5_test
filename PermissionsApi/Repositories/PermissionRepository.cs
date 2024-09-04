using Microsoft.EntityFrameworkCore;
using PermissionsApi.Data;
using PermissionsApi.Models.Entities;

namespace PermissionsApi.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DataContext _context;

        public PermissionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return await _context.Permissions
                .Include(p => p.PermissionType)
                .ToListAsync();
        }

        public async Task AddPermissionAsync(Permission permission)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePermissionAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PermissionExistsAsync(int id)
        {
            return await _context.Permissions.AnyAsync(p => p.Id == id);
        }

        public async Task<Permission?> GetPermissionByIdAsync(int id)
        {
            return await _context.Permissions.FindAsync(id);
        }
    }

}