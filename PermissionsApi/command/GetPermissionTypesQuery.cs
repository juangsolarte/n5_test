
using PermissionsApi.Mapper;
using PermissionsApi.Models.Dto;
using PermissionsApi.Repositories;

namespace PermissionsApi.command
{
    public class GetPermissionTypesQuery : IQuery<IEnumerable<PermissionTypeDto>>
    {
    }

    public class GetPermissionTypesQueryHandler(IUnitOfWork unitOfWork) : IQueryHandler<GetPermissionTypesQuery, IEnumerable<PermissionTypeDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<PermissionTypeDto>> HandleAsync(GetPermissionTypesQuery query)
        {
            var permissionsType = await _unitOfWork.PermissionTypeRepository.GetPermissionTypes();

            return permissionsType.Select(PermissionMapper.ToDto).ToList();
        }
    }

}