﻿namespace BE.Data.Dtos.GruopDtos
{
    public class DeleteMultiGroupModuleActionDto
    {
        public int idGroup { get; set; }
        public int idModule { get; set; }
        public int idAction { get; set; }
        public int? userDeleted { get; set; }
    }
}
