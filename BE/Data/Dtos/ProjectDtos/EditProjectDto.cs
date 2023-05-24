﻿using BE.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.ProjectDtos
{
    public class EditProjectDto
    {
        [Required]
        [MaxLength(10)]
        public string ProjectCode { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string SubProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int Leader { get; set; }
        [Required]
        public int UserCreated { get; set; }
        [Required]
        public int UserUpdate { get; set; }
        public bool IsOnGitlab { get; set; }
    }
}
