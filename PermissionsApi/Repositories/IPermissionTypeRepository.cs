using PermissionsApi.Models.Entities;

namespace PermissionsApi.Repositories
{
    public interface IPermissionTypeRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<PermissionType>> GetPermissionTypes();
    }
}