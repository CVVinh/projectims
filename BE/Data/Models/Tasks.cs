using BE.Data.Enums.TaskEnums;

namespace BE.Data.Models
{
    public class Tasks
    {
        public int idTask { get; set; }
        public int? assignee { get; set; }
        public string taskCode { get; set; }
        public string taskName { get; set; } //Title
        public string? description { get; set; }
        public int? workTime { get; set; }// bỏ không sài
		public Status? status { get; set; }
        public DateTime? startTaskDate { get; set; }// bỏ không sài
		public DateTime? endTaskDate { get; set; }// bỏ không sài
		public DateTime createTaskDate { get; set; } // bỏ không sài
        public DateTime estimate_DateCreated { get; set; } 
        public int idProject { get; set; }
        //Date: 10/3/2023
        //Modifile: add field typeTask,iid_issue,labels,Duedate,time_estimate,total_time_spent in table Tasks
        public string? typeTask { get; set; }
        public int? iid_issue { get; set; }
        public List<string>? labels { get; set; } 
        public DateTime? Duedate { get; set; }
        public string? time_estimate { get; set; }
        public string? total_time_spent { get; set; }
		//Date: 12/3/2023
		//Modifile: add field isDeleted,userUpdated,userDeleted,dateCreated,dateUpdated,dateDeleted in table Tasks
		public bool isDeleted { get; set; } = false;
		public int? createUser { get; set; }
		public int? userUpdated { get; set; }
		public int? userDeleted { get; set; }
        public DateTime? dateCreated { get; set; } = DateTime.Now;
		public DateTime? dateUpdated { get; set; }
		public DateTime? dateDeleted { get; set; }
        public bool isOnGitLab { get; set; } = false;
	}
}
