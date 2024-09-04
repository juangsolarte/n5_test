namespace PermissionsApi.Models.Dto
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string EmployeeForename { get; set; } = string.Empty;
        public string EmployeeSurname { get; set; } = string.Empty;
        public PermissionTypeDto PermissionType { get; set; } = new PermissionTypeDto();
        public DateTime PermissionDate { get; set; }
    }
}