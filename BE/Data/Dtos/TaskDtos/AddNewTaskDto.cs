using BE.Data.Enums.TaskEnums;

namespace BE.Data.Dtos.TaskDtos
{
    public class AddNewTaskDto
    {
        public int? assignee { get; set; }
        public string taskName { get; set; }
        public string? description { get; set; }
        //public int? workTime { get; set; }
        public Status? status { get; set; }
        //public DateTime? startTaskDate { get; set; }
        //public DateTime? endTaskDate { get; set; }
        public int createUser { get; set; }
        public int idProject { get; set; }
		public string? typeTask { get; set; }
		public int? iid_issue { get; set; }
		public List<string>? labels { get; set; }
		public DateTime? Duedate { get; set; }
		public string? time_estimate { get; set; }
		public string? total_time_spent { get; set; }
        public bool isOnGitLab { get; set; } = false;
    }
}
