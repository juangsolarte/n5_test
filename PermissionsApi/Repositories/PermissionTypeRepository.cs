using Microsoft.EntityFrameworkCore;
using PermissionsApi.Data;
using PermissionsApi.Models.Entities;

namespace PermissionsApi.Repositories
{
    public class PermissionTypeRepository : IPermissionTypeRepository
    {
        private readonly DataContext _context;

        public PermissionTypeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.PermissionTypes.AnyAsync(pt => pt.Id == id);
        }

        public async Task<IEnumerable<PermissionType>> GetPermissionTypes()
        {
            return await _context.PermissionTypes.ToListAsync();
        }
    }
}