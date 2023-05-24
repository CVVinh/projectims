using BE.Data.Dtos.PermissionActionModuleDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.ActionModuleServices;
using BE.Services.PaginationServices;
using BE.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: permissionActionModules")]
    public class PermissionActionModuleController : ControllerBase
    {
        private readonly IPermissionActionModuleServices _permissionActionModuleServices;
        private readonly IPaginationServices<Permission_Action_Module> _paginationService;
        private readonly IPaginationServices<GetAllPermissionActionModuleDto> _paginationServiceDto;

        public PermissionActionModuleController(IPermissionActionModuleServices permissionActionModuleServices, IPaginationServices<Permission_Action_Module> paginationService, IPaginationServices<GetAllPermissionActionModuleDto> paginationServiceDto)
        {
            _permissionActionModuleServices = permissionActionModuleServices;
            _paginationService = paginationService;
            _paginationServiceDto = paginationServiceDto;
        }

        [HttpGet("getAllPermissionActionModule")]
        public async Task<IActionResult> GetAllPermissionActionModuleAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _permissionActionModuleServices.GetAllPermissionActionModuleAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getNameAllPermissionActionModule")]
        public async Task<IActionResult> GetNameAllPermissionActionModule(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _permissionActionModuleServices.GetNameAllPermissionActionModule();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
               
            }
            return BadRequest(response);
        }

        [HttpGet("getPermissionActionModuleWithModuleId/{moduleId}")]
        public async Task<IActionResult> GetPermissionActionModuleWithModuleId([FromRoute] int moduleId)
        {
            var response = await _permissionActionModuleServices.GetPermissionActionModuleWithModuleId(moduleId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getPermissionActionModuleWithActionId/{actionId}")]
        public async Task<IActionResult> GetPermissionActionModuleWithActionId([FromRoute] int actionId)
        {
            var response = await _permissionActionModuleServices.GetPermissionActionModuleWithActionId(actionId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createPermissionActionModule")]
        [Authorize(Roles = "module:permissionActionModules action:add")]
        public async Task<IActionResult> CreatePermissionActionModule(List<AddPermissionActionModuleDto> addPermissionActionModuleDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionActionModuleServices.CreatePermissionActionModule(addPermissionActionModuleDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateUPermissionActionModule/{moduleId}/{actionModuleId}")]
        [Authorize(Roles = "module:permissionActionModules action:update")]
        public async Task<IActionResult> UpdateUPermissionActionModule([FromRoute] RequestPermissionActionModuleDto requestPermissionActionModuleDto, UpdatePermissionActionModuleDto updatePermissionActionModuleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionActionModuleServices.UpdateUPermissionActionModule(requestPermissionActionModuleDto, updatePermissionActionModuleDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("deletePermissionActionModule/{moduleId}/{actionModuleId}")]
        [Authorize(Roles = "module:permissionActionModules action:delete")]
        public async Task<IActionResult> DeletePermissionActionModule([FromRoute] RequestPermissionActionModuleDto requestPermissionActionModuleDto, DeletePermissionActionModuleDto deletePermissionActionModuleDto)
        {
            var response = await _permissionActionModuleServices.DeletePermissionActionModule(requestPermissionActionModuleDto, deletePermissionActionModuleDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("deleteMultiPermissionActionModule")]
        [Authorize(Roles = "module:permissionActionModules action:delete")]
        public async Task<IActionResult> DeleteMultiPermissionActionModule(List<DeleteMultiPermissionActionModuleDto> deleteMultiPermissionActionModuleDtos)
        {
            var response = await _permissionActionModuleServices.DeleteMultiPermissionActionModule(deleteMultiPermissionActionModuleDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }

}
