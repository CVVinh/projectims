using BE.Data.Models.Base;

namespace BE.Data.Models
{
    public class Group_Module_Action : BaseEntity
    {
        public int idGroup { get; set; }
        public int idModule { get; set; }
        public int idAction { get; set; }
    }
}
