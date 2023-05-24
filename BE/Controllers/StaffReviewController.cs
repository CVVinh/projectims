using BE.Data.Dtos.RulesDTOs;
using BE.Data.Dtos.StaffReviewDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Response;
using BE.Services.PaginationServices;
using BE.Services.StaffReviewServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: reviews")]
    public class StaffReviewController : ControllerBase
    {
        private readonly IStaffReviewService _staffReviewService;
        private readonly IPaginationServices<ReviewResult> _paginationService;
        private readonly IPaginationServices<StaffReviewDto> _paginationServiceDto;

        public StaffReviewController(IStaffReviewService staffReviewService, IPaginationServices<ReviewResult> paginationService, IPaginationServices<StaffReviewDto> paginationServiceDto)
        {
            _staffReviewService = staffReviewService;
            _paginationService = paginationService;
            _paginationServiceDto = paginationServiceDto;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStaffReview(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
          var response = await _staffReviewService.GetAllStaffReview();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);    
        }

        [HttpGet("GetAllStaffReviewByIdReviewTime/{idReviewTime}/{idBranch}/{idReviewer}")]
        public async Task<IActionResult> GetAllStaffReviewByIdReviewTime(int idReviewTime, int idBranch, int idReviewer, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _staffReviewService.GetAllStaffReviewByIdReviewTime(idReviewTime, idBranch, idReviewer);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [Authorize(Roles = "module:reviews action:add")]
        public async Task<IActionResult> CreateStaffReview(CreateStaffReviewDto createStaffReviewDto)
        {
            var response = await _staffReviewService.CreateStaffReview(createStaffReviewDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);    
        }
        [HttpGet("GetTimesOfStaffReview/{idStaff}/{idReviewer}")]
        public async Task<IActionResult> GetTimesOfStaffReview(int idStaff, int idReviewer)
        {
            var response = await _staffReviewService.TimesReview(idStaff, idReviewer);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("GetReviewByIdStaff/{idStaff}/{idReviewer}")]
        public async Task<IActionResult> GetReviewByIdStaff(int idStaff, int idReviewer)
        {
            var response = await _staffReviewService.GetReviewByIdStaff(idStaff, idReviewer);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("GetReviewById/{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var response = await _staffReviewService.GetReviewById(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("ConfirmReview/{id}")]
        [Authorize(Roles = "module:reviews action:confirm")]
        public async Task<IActionResult> ConfirmReview(int id, int idAccepted)
        {
            var response = await _staffReviewService.ConfirmReview(id,idAccepted);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);

        }

        [HttpPut("RejectReview/{id}")]
        [Authorize(Roles = "module:reviews action:refuse")]
        public async Task<IActionResult> RejectReview(int id, RejectReviewDto rejectReviewDto)
        {
            var response = await _staffReviewService.RejectReview(id, rejectReviewDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);

        }
        [HttpPut("EditReview/{id}")]
        [Authorize(Roles = "module:reviews action:update")]
        public async Task<IActionResult> EditReview(int id, CreateStaffReviewDto staffReviewDto)
        {
            var response = await _staffReviewService.EditReview(id, staffReviewDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getAllByOffice/{idReviewTime}/{idBranch}/{idReviewer}")]
        public async Task<IActionResult> getAllByOffice(int idReviewTime, int idBranch, int idReviewer, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _staffReviewService.GetAllStaffReviewByOffice(idReviewTime, idBranch, idReviewer);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getAllByLead/{idReviewTime}/{idBranch}/{idReviewer}")]
        public async Task<IActionResult> getAllByLead(int idReviewTime, int idBranch, int idReviewer, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _staffReviewService.GetReviewByUserCreated(idReviewTime, idBranch, idReviewer);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }
        /* [HttpGet("SearchReviewByStaffName/{fullName}")]
         public async Task<IActionResult> SearchReviewByStaffName(string fullName)
         {
             var response = await _staffReviewService.SearchReviewByStaffName(fullName);
             if (response._success)
             {
                 return Ok(response);
             }
             return BadRequest(response);
         }

         [HttpGet("SearchByReviewerId/{id}")]
         public async Task<IActionResult> SearchByReviewerId(int id)
         {
             var response = await _staffReviewService.SearchByReviewerId(id);
             if (response._success)
             {
                 return Ok(response);
             }
             return BadRequest(response);
         }*/

        [HttpPost("SearchReviewByStaffNameOrReviewerId")]
        public async Task<IActionResult> SearchReviewByStaffNameOrReviewerId(SearchReviewByStaffNameOrIDReviewerDto searchReviewByStaffNameOrIDReviewerDto, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _staffReviewService.SearchReviewByStaffNameOrReviewerId(searchReviewByStaffNameOrIDReviewerDto);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceDto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }
        [HttpGet("GetReviewTicketByStaffIdAndReviewerId/{staffId}/{reviewerId}")]
        public async Task<IActionResult> GetReviewTicketByStaffIdAndReviewerId(int staffId, int reviewerId)
        {
            var response = await _staffReviewService.GetReviewTicketByStaffIdAndReviewerId(staffId,reviewerId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetReviewTicketByLeadIdAndReviewerId/{staffId}/{reviewerId}")]
        public async Task<IActionResult> GetReviewTicketByLeadIdAndReviewerId(int staffId, int reviewerId)
        {
            var response = await _staffReviewService.GetReviewTicketByLeadIdAndReviewerId(staffId, reviewerId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("deleteReview/{id}")]
        [Authorize(Roles = "module:reviews action:delete")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var response = await _staffReviewService.DeleteReview(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("deleteReviewByUser/{idUser}")]
        [Authorize(Roles = "module:reviews action:delete")]
        public async Task<IActionResult> DeleteReviewByUser(int idUser)
        {
            var response = await _staffReviewService.DeleteReviewByUser(idUser);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("ConfirmUpdateReview/{idReview}/{idUserUpdate}")]
        [Authorize(Roles = "module:reviews action:confirm")]
        public async Task<IActionResult> ConfirmUpdateReview(int idReview, int idUserUpdate)
        {
            var response = await _staffReviewService.ConfirmUpdateReview(idReview, idUserUpdate);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);

        }

    }
}
