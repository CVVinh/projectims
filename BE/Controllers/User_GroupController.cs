using BE.Data.Contexts;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Dtos.UserDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.RulesServices;
using BE.Services.UserServices;
using BE.shared.Interface;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: userGroups")]
    public class User_GroupController : ControllerBase
    {
        private readonly IUserGroupServices _userGroupServices;
        private readonly IPaginationServices<User_Group> _paginationService;

        public User_GroupController(IUserGroupServices userGroupServices, IPaginationServices<User_Group> paginationService)
        {
            _userGroupServices = userGroupServices;
            _paginationService = paginationService;
        }

        [HttpGet("getAllUserGroup")]
        public async Task<IActionResult> GetAllUserGroupAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _userGroupServices.GetAllUserGroupAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getUserGroupWithGroupId/{idGroup}")]
        public async Task<IActionResult> GetUserGroupWithGroupId([FromRoute] int idGroup)
        {
            var response = await _userGroupServices.GetUserGroupWithGroupId(idGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getAllUserNameGroupWithGroupId/{idGroup}")]
        public async Task<IActionResult> GetAllUserNameGroupWithGroupId([FromRoute] int idGroup)
        {
            var response = await _userGroupServices.GetAllUserNameGroupWithGroupId(idGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpGet("getAllSearchUserNameGroupWithGroupId/{idGroup}/{userName}")]
        public async Task<IActionResult> GetAllSearchUserNameGroupWithGroupId([FromRoute] int idGroup, string userName)
        {
            var response = await _userGroupServices.GetAllSearchUserNameGroupWithGroupId(idGroup, userName);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getUserGroupWithUserId/{idUser}")]
        public async Task<IActionResult> GetUserGroupWithUserId([FromRoute] int idUser)
        {
            var response = await _userGroupServices.GetUserGroupWithUserId(idUser);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getUserObjectGroupWithUserId/{idUser}")]
        public async Task<IActionResult> GetUserObjectGroupWithUserId([FromRoute] int idUser)
        {
            var response = await _userGroupServices.GetUserObjectGroupWithUserId(idUser);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createUserGroup")]
        [Authorize(Roles = "module:userGroups action:add")]
        public async Task<IActionResult> CreateUserGroup(List<UserGroupCreatedDto> userGroupCreatedDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userGroupServices.CreateUserGroup(userGroupCreatedDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateUserGroup/{idUser}/{idGroup}")]
        [Authorize(Roles = "module:userGroups action:update")]
        public async Task<IActionResult> UpdateUserGroup([FromRoute] int idUser, [FromRoute] int idGroup, UserGroupUpdatedDto userGroupUpdatedDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userGroupServices.UpdateUserGroup(idUser, idGroup, userGroupUpdatedDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateUserGroupMultiRoleDefault/{idUser}/{idUserCreated}")]
        [Authorize(Roles = "module:userGroups action:update")]
        public async Task<IActionResult> UpdateUserGroupMultiRoleDefault(int idUser, int idUserCreated, List<int> listIdGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userGroupServices.UpdateUserGroupMultiRoleDefault(idUser, idUserCreated, listIdGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateUserGroupMultiRoleUpgrade/{idUser}/{idUserUpdated}")]
        [Authorize(Roles = "module:userGroups action:update")]
        public async Task<IActionResult> UpdateUserGroupMultiRoleUpgrade(int idUser, int idUserUpdated, List<int> listIdGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userGroupServices.UpdateUserGroupMultiRoleUpgrade(idUser, idUserUpdated, listIdGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("deleteUserGroup")]
        [Authorize(Roles = "module:userGroups action:delete")]
        public async Task<IActionResult> DeleteUserGroup([FromQuery] UserGroupDeletedDto userGroupDeletedDto)
        {
            var response = await _userGroupServices.DeleteUserGroup(userGroupDeletedDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPut("deleteMultiUserGroup")]
        [Authorize(Roles = "module:userGroups action:delete")]
        public async Task<IActionResult> DeleteMultiUserGroup(List<UserGroupDeletedDto> userGroupDeletedDto)
        {
            var response = await _userGroupServices.DeleteMultiUserGroup(userGroupDeletedDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }
}
