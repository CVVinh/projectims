using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("Projects")]
    public class Projects 
	{
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string ProjectCode { get; set; }
        [Required]

        public string Name { get; set; }

        [Required]
        public string SubProjectCode { get; set; }
        
        public string? ProjectName { get; set; }

        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public bool IsDeleted { get; set; } = false;
        [Required]
        public bool IsFinished { get; set; } = false;   
        [Required]
        public int UserId { get; set; }
        [Required]
        public int Leader {get; set;}
        [Required]
        public int UserCreated {get; set;}
        [Required]
        public DateTime DateCreated {get; set;}
        [Required]
        public int UserUpdate {get; set;}
        [Required]
        public DateTime DateUpdate {get; set;}
        //public Person Person { get; set; }
        public bool IsOnGitlab { get; set; } = false;
	}
}