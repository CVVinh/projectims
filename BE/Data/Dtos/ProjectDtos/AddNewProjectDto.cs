﻿using BE.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.ProjectDtos
{
    public class AddNewProjectDto 
    {
        public string Name { get; set; }
        public string ProjectCode { get; set; }
        public string SubProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int UserId { get; set; }
        public int Leader { get; set; }
        public int UserCreated { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
        public bool IsOnGitlab { get; set; }

    }
}
