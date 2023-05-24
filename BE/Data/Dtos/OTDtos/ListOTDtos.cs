using BE.Data.Models;

namespace BE.Data.Dtos.OTDtos
{
    public class ListOTDtos
    {
        public OTs  x { get; set; }
        public string name { get; set; }
        public string? nameLead { get; set; }
        public string? nameUser { get; set; }
        public string? nameUserUpdate { get; set; }
        public DateTime? dateUpdate { get; set; }
        public string? note { get; set; }
        public string usercode { get; set; }
        public int userCreated { get; set; }
        public int pm { get; set; }
        public string pmName { get; set; }
    }
}
