using BE.Data.Dtos.ActionModuleDtos;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class GetAllPermissionActionModuleDto
    {
        public int id { get; set; }
        public int moduleId { get; set; }
        public string nameModule { get; set; }
        public string note { get; set; }
        public List<ResponsePermissionActionModuleDto> actionModule { get; set; }
    }
}
