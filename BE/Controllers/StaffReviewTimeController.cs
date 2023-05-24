using BE.Data.Dtos.StaffReviewDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.StaffReviewServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: reviewsTime")]
    public class StaffReviewTimeController : ControllerBase
    {
        private readonly IStaffReviewTimeService _staffReviewTimeService;
        private readonly IPaginationServices<StaffReviewTime> _paginationService;
        private readonly IPaginationServices<GetAllMultiStaffViewTimeDto> _paginationServiceDto;
        public StaffReviewTimeController(IStaffReviewTimeService staffReviewTimeService, IPaginationServices<StaffReviewTime> paginationServices, IPaginationServices<GetAllMultiStaffViewTimeDto> paginationServiceDto)
        {
            _staffReviewTimeService = staffReviewTimeService;
            _paginationService = paginationServices;
            _paginationServiceDto = paginationServiceDto;
        }

        [HttpGet("getAllStaffReviewTime")]
        public async Task<IActionResult> GetAllStaffReviewTime(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _staffReviewTimeService.GetAllStaffReviewTime();
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

        [HttpPost("createNewStaffReviewTime")]
        [Authorize(Roles = "module:reviewsTime action:add")]
        public async Task<IActionResult> CreateNewStaffReviewTime(StaffReviewTimeDto staffReviewTimeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _staffReviewTimeService.CreateNewStaffReviewTime(staffReviewTimeDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateStaffReviewTime/{id}")]
        [Authorize(Roles = "module:reviewsTime action:update")]
        public async Task<IActionResult> UpdateStaffReviewTime(int id, UpdateStaffViewTimeDto staffReviewTimeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _staffReviewTimeService.UpdateStaffReviewTime(id, staffReviewTimeDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("deleteStaffReviewTime/{id}")]
        [Authorize(Roles = "module:reviewsTime action:delete")]
        public async Task<IActionResult> DeleteStaffReviewTime(int id)
        {
            var response = await _staffReviewTimeService.DeleteStaffReviewTime(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("SearchReviewerBranchReviewTime")]
        public async Task<IActionResult> SearchReviewerBranchReviewTime([FromQuery]SearchStaffReviewTimeDto searchStaffReviewTimeDto, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _staffReviewTimeService.SearchReviewerBranchReviewTime(searchStaffReviewTimeDto);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getAllStaffReviewWithBranch")]
        public async Task<IActionResult> GetAllStaffReviewWithBranch(int idBranch)
        {
            var response = await _staffReviewTimeService.GetAllStaffReviewWithBranch(idBranch);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("CreateMultiNewStaffReviewTime")]
        [Authorize(Roles = "module:reviewsTime action:add")]
        public async Task<IActionResult> CreateMultiNewStaffReviewTime(CreateMultiStaffReviewTimeDto createMultiStaffReviewTimeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _staffReviewTimeService.CreateMultiNewStaffReviewTime(createMultiStaffReviewTimeDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("UpdateMultiNewStaffReviewTime/{idStaffReviewTime}/{idBranch}/{idOffice}")]
        [Authorize(Roles = "module:reviewsTime action:update")]
        public async Task<IActionResult> UpdateMultiNewStaffReviewTime(int idStaffReviewTime, int idBranch, int idOffice, CreateMultiStaffReviewTimeDto createMultiStaffReviewTimeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _staffReviewTimeService.UpdateMultiNewStaffReviewTime(idStaffReviewTime, idBranch, idOffice, createMultiStaffReviewTimeDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetAllStaffRewviewTimeAsync")]
        public async Task<IActionResult> GetAllStaffRewviewTimeAsync(int? idReviewer, int? idUserCreated, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _staffReviewTimeService.GetAllStaffRewviewTimeAsync(idReviewer, idUserCreated);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("GetByIdOfficePmOrLeadNewStaffReviewTime/{idStaffReviewTime}/{idBranch}/{idOffice}")]
        public async Task<IActionResult> GetByIdOfficePmOrLeadNewStaffReviewTime(int idStaffReviewTime, int idBranch, int idOffice)
        {
            var response = await _staffReviewTimeService.GetByIdOfficePmOrLeadNewStaffReviewTime(idStaffReviewTime, idBranch, idOffice);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("DeleteMultiStaffReviewTime/{idStaffReviewTime}/{idBranch}/{idUserDeleted}")]
        [Authorize(Roles = "module:reviewsTime action:delete")]
        public async Task<IActionResult> DeleteMultiStaffReviewTime(int idStaffReviewTime, int idBranch, int idUserDeleted)
        {
            var response = await _staffReviewTimeService.DeleteMultiStaffReviewTime(idStaffReviewTime, idBranch, idUserDeleted);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("IdHandleStaffReviewTime/{idBranch}")]
        public async Task<IActionResult> IdHandleStaffReviewTime(int idBranch)
        {
            var response = await _staffReviewTimeService.IdHandleStaffReviewTime(idBranch);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getAllReviewTimeName")]
        public async Task<IActionResult> getAllReviewTimeName()
        {
            var response = await _staffReviewTimeService.getAllReviewTimeName();
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("ConfirmStaffReviewTime")]
        [Authorize(Roles = "module:reviewsTime action:confirm")]
        public async Task<IActionResult> ConfirmStaffReviewTime(ConfirmStaffReviewTimeDto confirmStaffReviewTimeDto)
        {
            var response = await _staffReviewTimeService.ConfirmStaffReviewTime(confirmStaffReviewTimeDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
