using BE.Data.Dtos.ModuleDtos;
using BE.Data.Dtos.TimeSheetDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.TimeSheetServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: timeSheetDailys")]
    public class TimeSheetDailyController : ControllerBase
    {
        private readonly ITimeSheetDailyServices _timeSheetDailyServices;
        private readonly IPaginationServices<TimeSheetDaily> _paginationService;
        private readonly IPaginationServices<ResponseTimeSheetDailyDto> _paginationServiceDto;

        public TimeSheetDailyController(ITimeSheetDailyServices timeSheetDailyServices, IPaginationServices<TimeSheetDaily> paginationService, IPaginationServices<ResponseTimeSheetDailyDto> paginationServiceDto)
        {
            _timeSheetDailyServices = timeSheetDailyServices;
            _paginationService = paginationService;
            _paginationServiceDto = paginationServiceDto;
        }

        [HttpGet("GetAllTimeSheetDaily/{idUser}/{idProject}")]
        public async Task<IActionResult> GetAllTimeSheetDaily(string idProject, int idUser, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _timeSheetDailyServices.GetAllTimeSheetDaily(idUser, idProject);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("SearchByDateOrIdUserTimeSheetDaily")]
        public async Task<IActionResult> SearchByDateOrIdUserTimeSheetDaily([FromQuery] GetByDateTimeSheetDto getByDateTimeSheetDto, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _timeSheetDailyServices.SearchByDateOrIdUserTimeSheetDaily(getByDateTimeSheetDto);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("GetByIdTimeSheetDaily/{id}/{idProject}/{idUser}")]
        public async Task<IActionResult> GetByIdTimeSheetDaily([FromRoute] int id, string idProject, int idUser)
        {
            var response = await _timeSheetDailyServices.GetByIdTimeSheetDaily(id, idProject, idUser);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetByIdUserTimeSheetDaily/{idUser}/{idProject}")]
        public async Task<IActionResult> GetByIdUserTimeSheetDaily([FromRoute] int idUser, string idProject)
        {
            var response = await _timeSheetDailyServices.GetByIdUserTimeSheetDaily(idUser, idProject);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetByDateTimeSheetDaily")]
        public async Task<IActionResult> GetByDateTimeSheetDaily(GetByDateTimeSheetDto getByDateTimeSheetDto)
        {
            var response = await _timeSheetDailyServices.GetByDateTimeSheetDaily(getByDateTimeSheetDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("ListTimeSheetTotal/{pageSize}/{idUser}/{idProject}")]
        public async Task<IActionResult> ListTimeSheetTotal(int pageSize, int idUser, string idProject)
        {
            var response = await _timeSheetDailyServices.ListTimeSheetTotal(pageSize, idUser, idProject);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("CreateTimeSheetDaily")]
        public async Task<IActionResult> CreateTimeSheetDaily(CreatedTimeSheetDailyDto createdTimeSheetDailyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _timeSheetDailyServices.CreateTimeSheetDaily(createdTimeSheetDailyDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("checkToDay")]
        public async Task<IActionResult> checkToDay([FromQuery] string date, int idUser, string idProject)
        {
            var response = await _timeSheetDailyServices.checkToDay(date, idUser, idProject);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("CreateMultiTimeSheetDaily")]
        public async Task<IActionResult> CreateMultiTimeSheetDaily(List<CreatedTimeSheetDailyDto> createdTimeSheetDailyDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _timeSheetDailyServices.CreateMultiTimeSheetDaily(createdTimeSheetDailyDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("UpdateTimeSheetDaily/{id}")]
        public async Task<IActionResult> UpdateTimeSheetDaily([FromRoute] int id, CreatedTimeSheetDailyDto updatedTimeSheetDailyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _timeSheetDailyServices.UpdateTimeSheetDaily(id, updatedTimeSheetDailyDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("UpdateMultiTimeSheetDaily")]
        public async Task<IActionResult> UpdateMultiTimeSheetDaily(List<UpdatedMultiTimeSheetDailyDto> updatedTimeSheetDailyDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _timeSheetDailyServices.UpdateMultiTimeSheetDaily(updatedTimeSheetDailyDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("DeleteTimeSheetDaily/{id}/{idUserDeleted}")]
        public async Task<IActionResult> DeleteTimeSheetDaily([FromRoute] int id, int idUserDeleted)
        {
            var response = await _timeSheetDailyServices.DeleteTimeSheetDaily(id, idUserDeleted);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("DeleteMultiTimeSheetDaily/{idUserDeleted}")]
        public async Task<IActionResult> DeleteMultiTimeSheetDaily([FromRoute] int idUserDeleted, List<DeleteMultiTimeSheetDailyDto> deleteMultiTimeSheetDailyDtos)
        {
            var response = await _timeSheetDailyServices.DeleteMultiTimeSheetDaily(idUserDeleted, deleteMultiTimeSheetDailyDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("DeleteByDateTimeSheetDaily")]
        public async Task<IActionResult> DeleteByDateTimeSheetDaily(GetByDateTimeSheetDto getByDateTimeSheetDto)
        {
            var response = await _timeSheetDailyServices.DeleteByDateTimeSheetDaily(getByDateTimeSheetDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
