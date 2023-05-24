using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using Group = BE.Data.Models.Group;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using BE.Services.PaginationServices;
using BE.Services.UserServices;
using BE.Services.GroupServices;
using BE.Data.Enum;
using BE.Data.Dtos.UserDtos;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupServices _groupServices;
        private readonly IPaginationServices<Group> _paginationService;

        public GroupController(IGroupServices groupServices, IPaginationServices<Group> paginationService)
        {
            _groupServices = groupServices;
            _paginationService = paginationService;
        }

        [HttpGet("getListGroup")]
        public async Task<IActionResult> GetAllGroupAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _groupServices.GetAllGroupAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }


        [HttpGet("getUserByGroup/{idGroup}")]
        public async Task<IActionResult> GetGroupById([FromRoute] int idGroup)
        {
            var response = await _groupServices.GetGroupById(idGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("addGroup")]
        [Authorize(Roles = "module:groups action:add")]
        public async Task<IActionResult> CreateGroup(AddGroupDtos addGroupDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _groupServices.CreateGroup(addGroupDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("updateGroup/{idGroup}")]
        [Authorize(Roles = "module:groups action:update")]
        public async Task<IActionResult> UpdateGroup([FromRoute] int idGroup, UpdateGroupDtos updateGroupDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _groupServices.UpdateGroup(idGroup, updateGroupDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("deleteGroup/{idGroup}")]
        [Authorize(Roles = "module:groups action:delete")]
        public async Task<IActionResult> DeleteGroup(int idGroup)
        {
            var response = await _groupServices.DeleteGroup(idGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("deleteMultiGroup")]
        public async Task<IActionResult> DeleteMultiGroup(List<int> listIdGroup)
        {
            var response = await _groupServices.DeleteMultiGroup(listIdGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [Route("exportExcel")]
        public async Task<IActionResult> DownloadFile()
        {
            var response = await _groupServices.DownloadFile();
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("SearchGroupByName")]
        public async Task<IActionResult> SearchGroupByName(string? name, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _groupServices.SearchGroupByName(name);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

    }
}
