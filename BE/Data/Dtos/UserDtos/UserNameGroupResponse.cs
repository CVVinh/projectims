namespace BE.Data.Dtos.UserDtos
{
    public class UserNameGroupResponse
    {
        public int id { get; set; }
        public int idUser { get; set; }
        public string firstName { get; set; }
        public string fullNameUser { get; set; }
        public int idGroup { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
