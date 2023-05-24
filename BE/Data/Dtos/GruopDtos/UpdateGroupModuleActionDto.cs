namespace BE.Data.Dtos.GruopDtos
{
    public class UpdateGroupModuleActionDto
    {
        public int idGroup { get; set; }
        public int idModule { get; set; }
        public int idAction { get; set; }
        public int? userUpdated { get; set; }
    }
}
