namespace BE.Data.Dtos.GruopDtos
{
    public class PermissionGroupAccessAllowMultiDto
    {
        public int idGroup { get; set; }
        public int idModule { get; set; }
        public int idUserUpdateted { get; set; }
        public List<int> listIdAction { get; set; } = new List<int>();
    }
}
