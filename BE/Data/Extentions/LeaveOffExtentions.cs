﻿using AutoMapper;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Models;
using static BE.Data.Enum.LeaveOff.Status;

namespace BE.Data.Extentions
{
    public static class LeaveOffExtentions
    {

        public static LeaveOff addNewLeaveOffExtention(this LeaveOff leaveOff)
        {
            leaveOff.createTime = DateTime.Now;
            var date = DateTime.Now;
            if ((leaveOff.startTime <= date && leaveOff.endTime <= date) ||
                              (leaveOff.startTime.Date <= date.Date && leaveOff.endTime <= date))
            //if((leaveOff.endTime - leaveOff.startTime).TotalHours > 24 && leaveOff.endTime <= date)
            {
                leaveOff.status = StatusLO.Done;
            }
            else
            {
                leaveOff.status = StatusLO.Waiting;
            }
            return leaveOff;
        }

        public static LeaveOff editRegisterLeaveOffExtention(this EditRegisterLeaveOffDtos editLeaveOff, LeaveOff leaveOff)
        {
            leaveOff.startTime = editLeaveOff.startTime;
            leaveOff.endTime = editLeaveOff.endTime;
            leaveOff.reasons = editLeaveOff.reasons;
            leaveOff.idCompanyBranh = editLeaveOff.idCompanyBranh;
            leaveOff.flagOnDay= editLeaveOff.flagOnDay;
            return leaveOff;
        }

        public static LeaveOff AccepterLeaveOffExtention(this LeaveOff leaveOff, string reasonAccept, int idAccepter, StatusLO status)
        {
            leaveOff.status = status;
            leaveOff.idAcceptUser = idAccepter;
            leaveOff.acceptTime = DateTime.UtcNow;
            leaveOff.ReasonAccept = reasonAccept;
            return leaveOff;
        }
        public static LeaveOff NotAcceptLeaveOffExtention(this LeaveOff leaveOff, NotAcceptLeaveOffDto notAcceptLeaveOffDto, StatusLO status)
        {
            leaveOff.status = status;
            leaveOff.idAcceptUser = notAcceptLeaveOffDto.idAcceptUser;
            leaveOff.acceptTime = DateTime.UtcNow;
            leaveOff.ReasonAccept = notAcceptLeaveOffDto.ReasonNotAccept;
            return leaveOff;
        }
    }
}
