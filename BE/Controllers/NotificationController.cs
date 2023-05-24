using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.NotificationDtos;
using BE.Data.Dtos.ProjectDtos;
using BE.Data.Models;
using BE.Hubs;
using BE.Response;
using BE.Services.MemberProjectServices;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using static BE.Data.Constand.ConfigEntity;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        IHubContext<NotificationHub> _notificationHubs;
        private readonly IMemberProjectServices _memberProjectServices;
        private readonly IMapper _mapper;
        IHubContext _hubContext;

        public NotificationController(IMemberProjectServices memberProjectServices, AppDbContext context, IHubContext<NotificationHub> notificationHubs, IMapper mapper)
        {
            _context = context;
            _notificationHubs = notificationHubs;
            _mapper = mapper;
            _memberProjectServices = memberProjectServices;
        }
        [HttpGet("GetAllListNotification")]
        public async Task<IActionResult> GetNotificationByPMId()
        {
            var data = await _context.Notifications.OrderByDescending(s => s.dateCreated).Where(x => x.isDeleted == false).ToListAsync();

            return Ok(data);
        }
        [HttpPut("WatchNotification/{id}")]
        public async Task<IActionResult> isWatchNotification(int id)
        {
            var check = await _context.Notifications.FindAsync(id);
            if (check != null)
            {
                check.isWatched = true;
                _context.Update(check);
                await _context.SaveChangesAsync();
                return Ok(check);
            }
            return BadRequest();
        }
        [HttpPost("RequireDeleteApplication")]
        [Authorize(Roles = "module:notifications action:add")]
        public async Task<IActionResult> CreateRequireDeleteApplication(CreateRequireDeleteApplicationDTO createRequireDeleteApplicationDTO)
        {
            if (ModelState.IsValid)
            {
                var findUser = await _context.Users.SingleOrDefaultAsync(x => x.userCode == createRequireDeleteApplicationDTO.usercode);

                if (findUser == null)
                {
                    return NotFound(); // user with the given usercode does not exist
                }

                var map = _mapper.Map<Notification>(createRequireDeleteApplicationDTO);
                map.isRequireDelete = true;
                map.dateCreated = DateTime.Now;
                _context.Notifications.Add(map);
                await _context.SaveChangesAsync();
                //await _notificationHubs.Clients.Groups(findUser.userCode).SendAsync("DeleteApplication", map.message);
                await _notificationHubs.Clients.All.SendAsync("DeleteApplication", map.message);
                return Ok(map);
            }
            return BadRequest();
        }
        [HttpGet("GetRequireNotification")]
        public async Task<IActionResult> GetRequireDeleteNotification(string userCode)
        {
            var getNoti = await _context.Notifications.Where(x => x.isRequireDelete == true && userCode == userCode).OrderBy(x => x.dateCreated).LastOrDefaultAsync();
            var map = _mapper.Map<CreateRequireDeleteApplicationDTO>(getNoti);
            return Ok(map);
        }
        [HttpDelete("deleteNotiById/{id}")]
        [Authorize(Roles = "module:notifications action:delete")]
        public async Task<IActionResult> deleteNotiById(int id)
        {
            var check = await _context.Notifications.FindAsync(id);
            if (check != null)
            {
                check.isWatched = true;
                _context.Remove(check);
                await _context.SaveChangesAsync();
                return Ok(check);
            }
            return BadRequest();
        }

        [HttpGet("GetNotificationOfPm")]
        public async Task<IActionResult> GetNotificationOfPm(int idPm)
        {
            var findPM = await _context.Users.FirstOrDefaultAsync(s => s.id.Equals(idPm));
            if (findPM != null)
            {
                var list = _context.Projects.Where(s => s.UserCreated.Equals(findPM.id)).Select(s => s.Leader).ToList();
                var data = _context.Notifications.OrderByDescending(s => s.dateCreated).Where(s => list.Contains(s.userCreated ?? 0) || (s.requestUser==null && s.receiveUser==null)).ToList();
                return Ok(data);
            }
            return BadRequest();
        }
        [HttpGet("GetNotificationOfLead")]
        public async Task<IActionResult> GetNotificationOfLead(int idLead)
        {
            var findLead = await _context.Users.FirstOrDefaultAsync(s => s.id.Equals(idLead));
            if (findLead != null)
            {
                var list = _context.Projects.Where(s => s.Leader.Equals(idLead)).Select(s => s.Id).ToList();

                var member = await _memberProjectServices.GetMembersByListIdProjectAsync(list);
                if (member._success)
                {
                    var userIds = member._Data.Select(m => m.member);
                    var data = _context.Notifications.OrderByDescending(s => s.dateCreated).Where(s => userIds.Contains(s.userCreated ?? 0) || (s.requestUser == null && s.receiveUser == null)).ToList();
                    return Ok(data);
                }
                return BadRequest("Không có thành viên nào");
            }
            return BadRequest("Không tìm thấy Leader");
        }
        [HttpGet("GetNotificationTimeSheetDailys")]
        public async Task<IActionResult> GetNotificationTimeSheetDailys()
        {
            var data = _context.Notifications.OrderByDescending(s => s.dateCreated).Where(s => s.isDeleted == false && (s.requestUser == null && s.receiveUser == null)).ToList();
            return Ok(data);
        }

        [HttpPost("CreateNotificationTimeSheet")]
        [Authorize(Roles = "module:notifications action:add")]
        public async Task<IActionResult> CreateNotificationTimeSheet()
        {
            if (ModelState.IsValid)
            {
                Notification noti = new Notification()
                {
                    message = "Nhập timesheet hôm nay " + DateTime.Now.ToString("dd/MM/yyyyy"),
                    title = "Nhập timesheet hằng ngày!",
                    isWatched = false,
                    userCreated = 53,
                    link = "/timeSheetDailys",
                    dateCreated = DateTime.Now,
                };
                _context.Notifications.Add(noti);
                await _context.SaveChangesAsync();

                return Ok(noti);
            }
            return BadRequest();
        }

    }
}
