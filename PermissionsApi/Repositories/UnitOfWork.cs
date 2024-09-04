using PermissionsApi.Data;

namespace PermissionsApi.Repositories
{
    public class UnitOfWork(DataContext context,
                      IPermissionRepository permissionRepository,
                      IPermissionTypeRepository permissionTypeRepository) : IUnitOfWork
    {
        private readonly DataContext _context = context;
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        private readonly IPermissionTypeRepository _permissionTypeRepository = permissionTypeRepository;

        public IPermissionRepository PermissionRepository => _permissionRepository;

        public IPermissionTypeRepository PermissionTypeRepository => _permissionTypeRepository;

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}