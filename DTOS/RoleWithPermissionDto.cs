namespace App_Proyecto.DTOS
{
    public class RoleWithPermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = [];
    }
}
