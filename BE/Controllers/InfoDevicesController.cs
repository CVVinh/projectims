using BE.Data.Dtos.InfoDeviceDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.InfoDeviceServices;
using BE.Services.PaginationServices;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: infoDevices")]
    public class InfoDevicesController : ControllerBase
    {
        private readonly IInfoDeviceService _infoDeviceService;
        private readonly IPaginationServices<AppInDevice> _paginationServices;

        public InfoDevicesController(IInfoDeviceService infoDeviceService, IPaginationServices<AppInDevice> paginationServices)
        {
            _infoDeviceService = infoDeviceService;
            _paginationServices = paginationServices;
        }

        [HttpGet("GetAllDeviceListWithApplication")]
        public async Task<IActionResult> GetAllDeviceList(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _infoDeviceService.GetAllAsync();
            if (response._success)
            {
                var pagesize = (int)pageSizeEnum;
                var resultpage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pagesize);
                if (resultpage._success)
                {
                    return Ok(resultpage);
                }
                return BadRequest(resultpage);
            }
            return BadRequest(response);
            //if (response._success)
            //{
            //    return Ok(response);
            //}
            //return BadRequest(response);
        }
        
        [HttpPost("CreateDeviceWithListApplication")]
        [Authorize(Roles = "module:infoDevices action:add")]
        public async Task<IActionResult> CreateInfoDevice(CreateInfoDeviceDto createInfoDeviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _infoDeviceService.CreateInfoDevice(createInfoDeviceDto);

            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("EditDeviceWithListApplicationByDeviceId/{id}")]
        [Authorize(Roles = "module:infoDevices action:update")]
        public async Task<IActionResult> EditInfoDevice(int id , CreateInfoDeviceDto createInfoDeviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.EditInfoDeviceWithDeviceId(id, createInfoDeviceDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("FindWithUserName/{name}")]
        public async Task<IActionResult> GetWithName(int? pageIndex, PageSizeEnum pageSizeEnum, string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.FindWithName(name);
            if (response._success)
            {
                var pagesize = (int)pageSizeEnum;
                var resultpage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pagesize);
                if (resultpage._success)
                {
                    return Ok(resultpage);
                }
                return BadRequest(resultpage);
            }
            return BadRequest(response);
        }

        [HttpGet("FindWithOperatingSystem/{operatingSystem}")]
        public async Task<IActionResult> GetWithOperatingSystem(int operatingSystem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.FindWithOperatingSystem(operatingSystem);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("FindWithUserCode/{userCode}")]
        public async Task<IActionResult> GetWithUserCode(string userCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.FindWithUserCode(userCode);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("FindWithUserId/{userId}")]
        public async Task<IActionResult> GetWithUserId(int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.FindWithUserId(userId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("EditDeviceWithListApplicationByUserId/{id}")]
        [Authorize(Roles = "module:infoDevices action:update")]
        public async Task<IActionResult> EditInfoDeviceWithUserId(int id, CreateInfoDeviceDto createInfoDeviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.EditInfoDeviceWithUserId(id, createInfoDeviceDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
