using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.PaidServices;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: paids")]
    public class PaidController : Controller
    {
        private readonly IPaidServices _paidServices;
        private readonly IPaginationServices<Paids> _paginationServices;
        private readonly IPaginationServices<PaidResponseDtos> _paginationServicesdto;

        private readonly IWebHostEnvironment _host;

        public PaidController( IPaginationServices<PaidResponseDtos> paginationServicesdto, IPaidServices paidServices, IPaginationServices<Paids> paginationServices, IWebHostEnvironment host)
        {
            _paidServices = paidServices;
            _paginationServices = paginationServices;
            _host = host;
            _paginationServicesdto = paginationServicesdto;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaidAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _paidServices.GetAllPaidAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("GetByIdAsync/{id}")]
        public async Task<IActionResult> GetPaidWithIdAsync(int id)
        {
            var response = await _paidServices.GetPaidWithIdAsync(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetByUserIdAsync")]
        public async Task<IActionResult> GetPaidWithUserIdAsync(int id)
        {
            var response = await _paidServices.GetPaidWithUserIdAsync(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetByIdSampleAsync/{id}")]
        public async Task<IActionResult> GetPaidWithSampleIdAsync(int id, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _paidServices.GetPaidWithSampleIdAsync(id);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        // ========================================================
        [HttpGet("GetAllPaid")]
        public async Task<IActionResult> GetAllPaid(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _paidServices.GetAllPaid();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesdto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetPaidWithId(int id)
        {
            var response = await _paidServices.GetPaidWithId(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("GetByUserId/{id}")]
        public async Task<IActionResult> GetPaidWithUserId(int id, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _paidServices.GetPaidWithUserId(id);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesdto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }
        [HttpGet("GetByIdSample/{id}")]
        public async Task<IActionResult> GetPaidWithSampleId(int id, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _paidServices.GetPaidWithSampleId(id);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesdto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }
        [HttpPost("SearchPaidByDay")]
        public async Task<IActionResult> SearchPaidByDay(int? pageIndex, PageSizeEnum pageSizeEnum, SearchDayPaidDtos searchDayPaidDtos)
        {
            var response = await _paidServices.SearchPaidByDay(searchDayPaidDtos);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServicesdto.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }
        // ========================================================


        [HttpPost]
        [Authorize(Roles = "module:paids action:add")]
        public async Task<IActionResult> CreatePaid([FromForm] CreatePaidDtos createPaidDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var response = await _paidServices.CreatePaid(createPaidDtos, _host.WebRootPath, pathServer);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "module:paids action:update")]
        public async Task<IActionResult> EditPaid(int id, [FromForm] CreatePaidDtos createPaidDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var paid = await _paidServices.EditPaid(id, createPaidDtos, _host.WebRootPath, pathServer);
            if (paid._success)
            {
                return Ok(paid);
            }

            return BadRequest(paid);
        }
       
        [HttpDelete("{id}")]
        [Authorize(Roles = "module:paids action:delete")]
        public async Task<IActionResult> DeletePaid(int id)
        {
            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var response = await _paidServices.DeletePaid(id, _host.WebRootPath, pathServer);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("multi-image/{id}")]
        public async Task<IActionResult> DeleteMultiImgPaid(int id, List<int>? listImg)
        {
            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var response = await _paidServices.DeleteMutilImgPaid(id, listImg, _host.WebRootPath, pathServer);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("SearchPaidByDayAsync")]
        public async Task<IActionResult> SearchPaidByDayAsync(SearchDayPaidDtos searchDayPaidDtos)
        {
            var response = await _paidServices.SearchPaidByDayAsync(searchDayPaidDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        //PUT: accept payment
        [HttpPut("acceptPayment/{idPaid}")]
        [Authorize(Roles = "module:paids action:confirm")]
        public async Task<IActionResult> acceptRegisterPaid(int idPaid, AcceptPaymentPaidDtos acceptPaymentPaidDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var paid = await _paidServices.AccepterPayment(idPaid, acceptPaymentPaidDtos);
            if (paid._success)
            {
                return Ok(paid);
            }

            return BadRequest(paid);
        }

        //PUT: not accept payment
        [HttpPut("NotAcceptPayment/{idPaid}")]
        [Authorize(Roles = "module:paids action:refuse")]
        public async Task<IActionResult> NotAcceptRegisterPaid(int idPaid, AcceptPaymentPaidDtos acceptPaymentPaidDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var paid = await _paidServices.NotAccepterPayment(idPaid, acceptPaymentPaidDtos);
            if (paid._success)
            {
                return Ok(paid);
            }

            return BadRequest(paid);
        }

    }
}
