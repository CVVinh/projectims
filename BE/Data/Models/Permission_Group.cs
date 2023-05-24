﻿namespace BE.Data.Models
{
    public class Permission_Group
    {
        public int Id { get; set; }
        public int IdGroup { get; set; }
        public int IdModule { get; set; }
        public bool Access { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
