using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.Permission_Use_Menus
{
    public class PermissionUserMenuRequesNoIdGrouptDto
    {
        [Required]
        public int idModule { get; set; }
        [Required]
        public int IdUser { get; set; }
        public int? userModified { get; set; } = 0;
    }
}
