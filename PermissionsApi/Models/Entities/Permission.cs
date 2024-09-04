namespace PermissionsApi.Models.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string EmployeeForename { get; set; } = string.Empty;
        public string EmployeeSurname { get; set; } = string.Empty;
        public int PermissionTypeId { get; set; }
        public PermissionType PermissionType { get; set; } = null!;
        public DateTime PermissionDate { get; set; }
    }
}