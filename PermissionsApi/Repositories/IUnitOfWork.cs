namespace PermissionsApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPermissionRepository PermissionRepository { get; }
        IPermissionTypeRepository PermissionTypeRepository { get; }
        Task<int> CompleteAsync();
    }
}