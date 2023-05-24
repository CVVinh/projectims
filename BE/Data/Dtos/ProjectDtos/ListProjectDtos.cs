namespace BE.Data.Dtos.ProjectDtos
{
    public class ListProjectDtos
    {
        public int id { get; set; }
        public DateTime? dateCreated { get; set; }
        public DateTime? dateUpdate { get; set; }
        public string description { get; set; }
        public DateTime? endDate { get; set; }
        public bool isDeleted { get; set; }
        public bool isFinished { get; set; }
        public bool isOnGitlab { get; set; }
        public string leader { get; set; }
        public int idLeader { get; set; }
        public string name { get; set; }
        public string projectCode { get; set; }
        public string subProjectCode { get; set; }
        public string projectName { get; set; }
        public DateTime startDate { get; set; }
        public int userCreated { get; set; }
        public int userId { get; set; }
        public int userUpdate { get; set; }
        public string? fullNameUserCreated { get; set; }
        public string? fullNameUserUpdate { get; set; }
        public string? fullNameUserId { get; set; }
    }
}
