﻿using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Hubs;
using BE.Services.LeaveOffServices;
using BE.Services.PaginationServices;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static BE.Data.Enum.LeaveOff.Status;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/leaveOff")]
    [Authorize(Roles = "permission_group: True module: leaveOffs")]
    public class LeaveOffController : Controller
    {
        private readonly ILeaveOffServices _leaveOffServices;
        private readonly IPaginationServices<LeaveOff> _paginationServices;
        private readonly IPaginationServices<ListLeaveOffInfo> _paginationServicesinfo;

        IHubContext<NotificationHub> _notificationHubs;

        public LeaveOffController(ILeaveOffServices leaveOffServices, IPaginationServices<ListLeaveOffInfo> paginationServicesinfo, IPaginationServices<LeaveOff> paginationServices, IHubContext<NotificationHub> notificationHubs)
        {
            _leaveOffServices = leaveOffServices;
            _paginationServices = paginationServices;
            _notificationHubs = notificationHubs;
            _paginationServicesinfo = paginationServicesinfo;
        }

        //GET: get all leave off
        [HttpGet("getAllLeaveOff")]
        public async Task<IActionResult> getAllLeaveOff(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _leaveOffServices.GetAllAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesinfo.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getAllLeaveOffByLead/{id}")]
        public async Task<IActionResult> getAllLeaveOffByLead(int? pageIndex, PageSizeEnum pageSizeEnum, int id)
        {
            var response = await _leaveOffServices.GetAllAsyncByLead(id);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesinfo.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getAllLeaveOffByPM/{id}")]
        public async Task<IActionResult> getAllLeaveOffByPM(int? pageIndex, PageSizeEnum pageSizeEnum, int id)
        {
            var response = await _leaveOffServices.GetAllAsyncByPm(id);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesinfo.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }


        //GET: get all leave off by status
        [HttpGet("getAllLeaveOffByStatus")]
        public async Task<IActionResult> getAllLeaveOffByStatus(StatusLO statusLO, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _leaveOffServices.GetAllLeaveOffByStatus(statusLO);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        //GET: get all leave off of register by status
        [HttpGet("getAllLeaveOffOfidUserByStatus/{idLeaveOffUser}")]
        public async Task<IActionResult> getAllLeaveOffOfIdUserByStatus(int idLeaveOffUser, StatusLO statusLO, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _leaveOffServices.GetAllLeaveOffOfIdUserByStatus(idLeaveOffUser, statusLO);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        //GET: get leave off by idLeaveOff
        [HttpGet("getLeaveOffById/{idLeaveOff}")]
        public async Task<IActionResult> getLeaveOffById(int idLeaveOff)
        {
            var response = await _leaveOffServices.GetLeaveOffByIdAsync(idLeaveOff);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        
        //POST: add a new leave off
        [HttpPost("addNewLeaveOff")]
        [Authorize(Roles = "module:leaveOffs action:add")]
        public async Task<IActionResult> addNewLeaveOff(AddNewLeaveOffDto addNewLeaveOffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _leaveOffServices.AddNewLeaveOffAsync(addNewLeaveOffDto);
            if (response._success)
            {
                await _notificationHubs.Clients.All.SendAsync("ReceiveLeaveOff");
                return Ok(response);
            }
            return BadRequest(response);
        }

        //PUT: edit form leave off of registering
        [HttpPut("editRegisterLeaveOff/{id}")]
        [Authorize(Roles = "module:leaveOffs action:update")]
        public async Task<IActionResult> editRegisterLeaveOff(int id, EditRegisterLeaveOffDtos editRegisterLeaveOffDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leaveOff = await _leaveOffServices.EditRegisterLeaveOffAsync(id, editRegisterLeaveOffDtos);
            if (leaveOff._success)
            {
                return Ok(leaveOff);
            }

            return BadRequest(leaveOff);
        }

        //PUT: accept form leave off 
        [HttpPut("accceptLeaveOff/{idLeaveOff}")]
        [Authorize(Roles = "module:leaveOffs action:confirm")]
        public async Task<IActionResult> acceptRegisterLeaveOff(int idLeaveOff, AccepterLeaveOffDto accepterLeaveOffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var leaveOff = await _leaveOffServices.AccepterLeaveOffAsync(idLeaveOff, accepterLeaveOffDto);
            if (leaveOff._success)
            {
                return Ok(leaveOff);
            }

            return BadRequest(leaveOff);
        }

        //PUT: not accept form leave off 
        [HttpPut("notAcceptLeaveOff/{idLeaveOff}")]
        [Authorize(Roles = "module:leaveOffs action:refuse")]
        public async Task<IActionResult> notAcceptRegisterLeaveOff(int idLeaveOff, NotAcceptLeaveOffDto notAcceptLeaveOffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leaveOff = await _leaveOffServices.NotAccepterLeaveOffAsync(idLeaveOff, notAcceptLeaveOffDto);
            if (leaveOff._success)
            {
                return Ok(leaveOff);
            }

            return BadRequest(leaveOff);
        }

        //DELETE: delete leave off by idLeaveOff
        [HttpDelete("deleteLeaveOffById/{idLeaveOff}")]
        [Authorize(Roles = "module:leaveOffs action:delete")]
        public async Task<IActionResult> deleteLeaveOffById(int idLeaveOff)
        {
            var response = await _leaveOffServices.DeleteLeaveOffByIdAsync(idLeaveOff);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("GetByNameStatusDate")]
        public async Task<IActionResult> getLeaveOffByNameStatusDate(int? pageIndex, PageSizeEnum pageSizeEnum, FindByNameStatusDateDtos findByNameStatusDateDtos)
        {
            var response = await _leaveOffServices.GetAllLeaveOffByNameStatusDate(findByNameStatusDateDtos);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesinfo.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllLeaveOffByYear")]
        public async Task<IActionResult> getAllLeaveOffByYear(int? pageIndex, PageSizeEnum pageSizeEnum, FindByNameStatusDateDtos findByNameStatusDateDtos)
        {
            var response = await _leaveOffServices.GetAllLeaveOffByYear(findByNameStatusDateDtos);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesinfo.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getAllLeaveOffInfo")]
        public async Task<IActionResult> getAllLeaveOffInfo(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _leaveOffServices.getAllLeaveOffInfo();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesinfo.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpPost("GetByNameStatusDateLead/{id}")]
        public async Task<IActionResult> GetByNameStatusDateLead(int id, int? pageIndex, PageSizeEnum pageSizeEnum, FindByNameStatusDateDtos findByNameStatusDateDtos)
        {
            var response = await _leaveOffServices.GetAllLeaveOffByNameStatusDateLead(id, findByNameStatusDateDtos);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesinfo.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }
        [HttpPost("GetByNameStatusDatePM/{id}")]
        public async Task<IActionResult> GetByNameStatusDatePM(int id, int? pageIndex, PageSizeEnum pageSizeEnum, FindByNameStatusDateDtos findByNameStatusDateDtos)
        {
            var response = await _leaveOffServices.GetAllLeaveOffByNameStatusDatePM(id, findByNameStatusDateDtos);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesinfo.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }
        [HttpGet("GetInfoUserLeaveOff/{idUser}")]
        public async Task<IActionResult> GetInfoUserLeaveOff(int idUser, DateTime date)
        {
            var response = await _leaveOffServices.GetInfoUserLeaveOff(idUser, date);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
