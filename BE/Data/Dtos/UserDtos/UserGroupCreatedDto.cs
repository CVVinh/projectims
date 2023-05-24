using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.UserDtos
{
    public class UserGroupCreatedDto
    {
        [Required]
        public int idUser { get; set; }

        [Required]
        public int idGroup { get; set; }

        public int? userCreated { get; set; } = 0;

    }
}
