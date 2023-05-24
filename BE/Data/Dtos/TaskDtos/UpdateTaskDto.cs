using BE.Data.Enums.TaskEnums;

namespace BE.Data.Dtos.TaskDtos
{
    public class UpdateTaskDto
    {
        public int? assignee { get; set; }
        public string taskName { get; set; }
        public string? description { get; set; }
        public Status? status { get; set; }
        public List<string>? labels { get; set; }
        public DateTime? Duedate { get; set; }
        public string? time_estimate { get; set; }
        public int? userUpdated { get; set; }
        public DateTime? dateUpdated { get; set; } = DateTime.Now;
    }
}
