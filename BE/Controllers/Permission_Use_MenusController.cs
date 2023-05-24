﻿using BE.Data.Contexts;
using BE.Data.Dtos.Permission_Use_Menus;
using BE.Data.Dtos.UserDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Helpers;
using BE.Services.PaginationServices;
using BE.Services.PermissionUserMenuServices;
using BE.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: permissionUserMenus")]
    public class Permission_Use_MenusController : ControllerBase
    {

        private readonly IPermissionUserMenuServices _permissionUserMenuServices;
        private readonly IPaginationServices<Permission_Use_Menu> _paginationService;

        public Permission_Use_MenusController(IPermissionUserMenuServices permissionUserMenuServices, IPaginationServices<Permission_Use_Menu> paginationService)
        {
            _permissionUserMenuServices = permissionUserMenuServices;
            _paginationService = paginationService;
        }

        [HttpGet("getAllPermissionUserMenuAsync")]
        public async Task<IActionResult> GetAllPermissionUserMenuAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _permissionUserMenuServices.GetAllPermissionUserMenuAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getPermissionUserMenuWithUserId/{idUser}")]
        public async Task<IActionResult> GetPermissionUserMenuWithUserId([FromRoute] int idUser)
        {
            var response = await _permissionUserMenuServices.GetPermissionUserMenuWithUserId(idUser);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getPermissionUserMenuWithModuleId/{idModule}")]
        public async Task<IActionResult> GetPermissionUserMenuWithModuleId([FromRoute] int idModule)
        {
            var response = await _permissionUserMenuServices.GetPermissionUserMenuWithModuleId(idModule);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getPermissionUserMenuWithMenuId/{idMenu}")]
        public async Task<IActionResult> GetPermissionUserMenuWithMenuId([FromRoute] int idMenu)
        {
            var response = await _permissionUserMenuServices.GetPermissionUserMenuWithMenuId(idMenu);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createPermissionUserMenu")]
        public async Task<IActionResult> CreatePermissionUserMenu(List<PermissionUserMenuAddDto> permissionUserMenuAddDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionUserMenuServices.CreatePermissionUserMenu(permissionUserMenuAddDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("addPermissionRoleUserMenuDefault/{idUser}/{idUserCreated}")]
        public async Task<IActionResult> AddPermissionRoleUserMenuDefault([FromRoute] int idUser, [FromRoute] int idUserCreated, List<int> listIdGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionUserMenuServices.AddPermissionRoleUserMenuDefault(idUser, idUserCreated, listIdGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("AddPermissionRoleUserMenuUpgrade/{idUser}/{idUserCreated}")]
        public async Task<IActionResult> AddPermissionRoleUserMenuUpgrade([FromRoute] int idUser, [FromRoute] int idUserCreated, List<int> listIdGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionUserMenuServices.AddPermissionRoleUserMenuUpgrade(idUser, idUserCreated, listIdGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updatePermissionUserMenu/{IdUser}/{idModule}/{idGroup}")]
        public async Task<IActionResult> UpdatePermissionUserMenu([FromRoute] PermissionUserMenuRequestDto permissionUserMenuRequest, PermissionUserMenuEditDto permissionUserMenuEditDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var rules = await _permissionUserMenuServices.UpdatePermissionUserMenu(permissionUserMenuRequest, permissionUserMenuEditDto);
            if (rules._success)
            {
                return Ok(rules);
            }
            return BadRequest(rules);
        }

        [HttpDelete("deletePermissionUserMenu/{IdUser}/{idModule}")]
        public async Task<IActionResult> DeletePermissionUserMenu([FromRoute] PermissionUserMenuRequesNoIdGrouptDto permissionUserMenuRequesNoIdGrouptDto)
        {
            var response = await _permissionUserMenuServices.DeletePermissionUserMenu(permissionUserMenuRequesNoIdGrouptDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("deletePermissionRoleUserMenuDefault/{idUser}")]
        public async Task<IActionResult> DeletePermissionRoleUserMenuDefault([FromRoute] int idUser, List<int> listIdGroup)
        {
            var response = await _permissionUserMenuServices.DeletePermissionRoleUserMenuDefault(idUser, listIdGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("deletePermissionRoleUserMenuUpgrade/{idUser}/{idUserUpdated}")]
        public async Task<IActionResult> DeletePermissionRoleUserMenuUpgrade([FromRoute] int idUser, [FromRoute] int idUserUpdated, List<int> listIdGroup)
        {
            var response = await _permissionUserMenuServices.DeletePermissionRoleUserMenuUpgrade(idUser, idUserUpdated, listIdGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("deleteMultiPermissionUserMenu")]
        public async Task<IActionResult> DeleteMultiPermissionUserMenu(List<PermissionUserMenuRequesNoIdGrouptDto> permissionUserMenuRequesNoIdGrouptDto)
        {
            var response = await _permissionUserMenuServices.DeleteMultiPermissionUserMenu(permissionUserMenuRequesNoIdGrouptDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }
}
