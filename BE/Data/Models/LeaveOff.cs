﻿using static BE.Data.Enum.LeaveOff.Status;

namespace BE.Data.Models
{
    public class LeaveOff
    {
        public int id { get; set; }
        public int idLeaveUser { get; set; }
        public int? idAcceptUser { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string reasons { get; set; }
        public StatusLO status { get; set; }
        public DateTime createTime { get; set; }
        public DateTime? acceptTime { get; set; }

        //Date: 8/2/2023
        //Modifile: add field ReasonNotAccept in table LeaveOff
        public string? ReasonNotAccept { get; set; }

        //Date: 22/2/2023
        //Modifile: add field IdCompanyBranh in table LeaveOff
        public int? idCompanyBranh { get; set; }

        //Date: 9/3/2023
        //Modifile: add field ReasonAccept in table LeaveOff
        public string? ReasonAccept { get; set; }

        public bool flagOnDay { get; set; } = false;
    }
}
