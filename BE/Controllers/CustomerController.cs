using BE.Data.Dtos.CustomerDtos;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.Customers;
using BE.Services.PaginationServices;
using BE.Services.PaidServices;
using BE.shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPaginationServices<Customer> _paginationService;

        public CustomerController(ICustomerService customerService, IPaginationServices<Customer> paginationServices)
        {
            _customerService = customerService;
            _paginationService = paginationServices;
        }

        [HttpGet("getAllCustomer")]
        public async Task<IActionResult> GetAllPaid(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _customerService.GetAllAsync();
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

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _customerService.GetById(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("createCustomer")]
        [Authorize(Roles = "module:customers action:add")]
        public async Task<IActionResult> CreateCustomer(CustomerDto customerDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _customerService.CreateCustomer(customerDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("updateCustomer/{id}")]
        [Authorize(Roles = "module:customers action:update")]
        public async Task<IActionResult> UpdateCustomer([FromRoute] int id, UpdateCustomerDto customerDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _customerService.UpdateCustomer(id, customerDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("deleteCustomer/{id}")]
        [Authorize(Roles = "module:customers action:delete")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            var response = await _customerService.DeleteCustomer(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
