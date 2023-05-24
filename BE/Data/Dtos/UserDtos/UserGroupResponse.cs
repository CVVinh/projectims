using BE.Data.Models;

namespace BE.Data.Dtos.UserDtos
{
    public class UserGroupResponse
    {

        public int id { get; set; }
        public int idUser { get; set; }

        public int idGroup { get; set; }
        public Group? group { get; set; } = new Group();
    }
}
