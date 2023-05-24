namespace BE.Data.Dtos.GruopDtos
{
    public class AddGroupModuleActionDto
    {
        public int idGroup { get; set; }
        public int idModule { get; set; }
        public int idAction { get; set; }
        public bool isDeleted { get; set; }
        public int userCreated { get; set; }
    }
}
