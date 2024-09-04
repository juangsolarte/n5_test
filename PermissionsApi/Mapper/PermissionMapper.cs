using PermissionsApi.Models.Dto;
using PermissionsApi.Models.Entities;

namespace PermissionsApi.Mapper
{
    public static class PermissionMapper
    {
        public static PermissionDto ToDto(Permission permission)
        {
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission), "Permission cannot be null");
            }
            
            return new PermissionDto
            {
                Id = permission.Id,
                EmployeeForename = permission.EmployeeForename,
                EmployeeSurname = permission.EmployeeSurname,
                PermissionType = new PermissionTypeDto
                {
                    Id = permission.PermissionType.Id,
                    Description = permission.PermissionType.Description
                },
                PermissionDate = permission.PermissionDate
            };
        }
        public static PermissionTypeDto ToDto(PermissionType permissionType)
        {
            if (permissionType == null)
            {
                throw new ArgumentNullException(nameof(permissionType), "PermissionType cannot be null");
            }
            
            return new PermissionTypeDto
            {
                Id = permissionType.Id,
                Description = permissionType.Description
            };
        }

        public static Permission FromDto(CreatePermissionDto dto)
        {
            return new Permission
            {
                EmployeeForename = dto.EmployeeForename,
                EmployeeSurname = dto.EmployeeSurname,
                PermissionTypeId = dto.PermissionTypeId,
                PermissionDate = dto.PermissionDate
            };
        }
    }

}