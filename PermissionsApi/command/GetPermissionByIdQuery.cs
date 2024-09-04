
using PermissionsApi.Mapper;
using PermissionsApi.Models.Dto;
using PermissionsApi.Repositories;

namespace PermissionsApi.command
{
    public class GetPermissionByIdQuery(int id) : IQuery<PermissionDto>
    {
        public int Id { get; } = id;
    }

    public class GetPermissionByIdQueryHandler(IUnitOfWork unitOfWork) : IQueryHandler<GetPermissionByIdQuery, PermissionDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<PermissionDto> HandleAsync(GetPermissionByIdQuery query)
        {
            var permission = await _unitOfWork.PermissionRepository.GetPermissionByIdAsync(query.Id) ?? throw new Exception("Permission not found");
            var permissionTypes = await _unitOfWork.PermissionTypeRepository.GetPermissionTypes();
            await _unitOfWork.CompleteAsync();
            permission.PermissionType = permissionTypes.FirstOrDefault(pt => pt.Id == permission.PermissionTypeId) ?? throw new Exception("PermissionType not found");
            return PermissionMapper.ToDto(permission);
        }
    }

}