namespace PermissionsApi.Models.Dto
{
    public class CreatePermissionDto
    {
        public string EmployeeForename { get; set; } = string.Empty;
        public string EmployeeSurname { get; set; } = string.Empty;
        public int PermissionTypeId { get; set; }
        public DateTime PermissionDate { get; set; }
    }
}