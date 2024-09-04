using PermissionsApi.Models.Entities;

namespace PermissionsApi.Repositories
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetPermissionsAsync();
        Task AddPermissionAsync(Permission permission);
        Task UpdatePermissionAsync(Permission permission);
        Task<bool> PermissionExistsAsync(int id);
        Task<Permission?> GetPermissionByIdAsync(int id);
    }

}