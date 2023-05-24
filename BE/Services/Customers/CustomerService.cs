using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.CustomerDtos;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BE.Services.Customers
{

    public interface ICustomerService
    {
        Task<BaseResponse<List<Customer>>> GetAllAsync();
        Task<BaseResponse<Customer>> GetById(int customerId);
        Task<BaseResponse<Customer>> CreateCustomer(CustomerDto customerDtos);
        Task<BaseResponse<Customer>> UpdateCustomer(int id, UpdateCustomerDto customerDto);
        Task<BaseResponse<Customer>> DeleteCustomer(int id);
    }

    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;

        public CustomerService(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Customer>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Customer>();
            try
            {
                var customers = await _appContext.Customers.Where(s => s.isDeleted == false).OrderBy(s => s.lastName).ThenBy(s => s.firstName).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(customers);
                return (new BaseResponse<List<Customer>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Customer>>(success, message, data));
            }
        }

        public async Task<BaseResponse<Customer>> GetById(int customerId)
        {
            var success = false;
            var message = "";
            var data = new Customer();
            try
            {
                var customers = await _appContext.Customers.Where(x => x.isDeleted == false &&  x.id == customerId).OrderByDescending(x => x.dateCreated).FirstOrDefaultAsync();

                if (customers is null)
                {
                    message = "customerId doesn't exist !";
                    data = null;
                    return new BaseResponse<Customer>(success, message, data);
                }
                success = true;
                data = customers;
                message = "Get data successfully";
                return (new BaseResponse<Customer>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<Customer>(success, message, data));
            }
        }
        public async Task<BaseResponse<Customer>> CreateCustomer(CustomerDto customerDtos)
        {
            var success = false;
            var message = "";
            try
            {
                if(customerDtos.identityCard != null && customerDtos.identityCard.Length != 0)
                {
                    var isNumber = customerDtos.identityCard.All(char.IsDigit);
                    if (!isNumber)
                    {
                        message = "Chứng minh nhân dân không phải là số!";
                        return new BaseResponse<Customer>(success, message, new Customer());
                    }
                    if (customerDtos.identityCard.Length != 9 && customerDtos.identityCard.Length != 12)
                    {
                        message = "Độ dài của số CMND phải là 9 hoặc 12.";
                        return new BaseResponse<Customer>(success, message, new Customer());
                    }
                    //check is exist
                    var identityExist = _appContext.Customers.Where(u => u.identityCard == customerDtos.identityCard);
                    if (identityExist.Any())
                    {
                        message = "Chứng minh nhân dân đã tồn tại!";
                        return new BaseResponse<Customer>(success, message, new Customer());
                    }
                }               
                var phoneExist = _appContext.Customers.Where(u => u.phoneNumber == customerDtos.phoneNumber);
                if (customerDtos.phoneNumber != null)
                {
                    if (phoneExist.Any() && customerDtos.phoneNumber.Length != 0)
                    {
                        message = "Số điện thoại đã tồn tại!";
                        return new BaseResponse<Customer>(success, message, new Customer());
                    }
                }
                if(customerDtos.email != null && customerDtos.email.Length != 0)
                {
                    var emailExist = _appContext.Customers.Where(u => u.email == customerDtos.email);
                    if (emailExist.Any())
                    {
                        message = "Email đã tồn tại!";
                        return new BaseResponse<Customer>(success, message, new Customer());
                    }
                }                       
                var customer = _mapper.Map<Customer>(customerDtos);
                customer.isDeleted = false;
                await _appContext.Customers.AddAsync(customer);
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Thêm mới khách hàng thành công!";
                return new BaseResponse<Customer>(success, message, customer);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Lỗi thêm mới khách hàng! {ex.Message}";
                return new BaseResponse<Customer>(success, message, new Customer());
            }
        }

        public async Task<BaseResponse<Customer>> UpdateCustomer(int id, UpdateCustomerDto customerDtos)
        {
            var success = false;
            var message = "";
            try
            {
                var customer = await _appContext.Customers.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                //check is exist
                if (customer is null)
                {
                    message = "ID không tồn tại!";
                    return new BaseResponse<Customer>(success, message, new Customer());
                }               

                customer.dateUpdated = DateTime.Now;
                var customerMapData = _mapper.Map<UpdateCustomerDto, Customer>(customerDtos, customer);
                _appContext.Customers.Update(customerMapData);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Cập nhật thành công!";
                return new BaseResponse<Customer>(success, message, customerMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Cập nhật thất bại! {ex.Message}";
                return new BaseResponse<Customer>(success, message, new Customer());
            }
        }

        public async Task<BaseResponse<Customer>> DeleteCustomer(int id)
        {
            var success = false;
            var message = "";
            try
            {
                var customer = await _appContext.Customers.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (customer is null)
                {
                    message = "Khách hàng không tồn tại !";
                    return new BaseResponse<Customer>(success, message, new Customer());
                }
                
                customer.isDeleted = true;
                customer.dateDeleted = DateTime.Now;
                _appContext.Customers.Update(customer);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Xóa khách hàng thành công!";
                return new BaseResponse<Customer>(success, message, customer);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xóa khách hàng thất bại! {ex.InnerException}";
                return new BaseResponse<Customer>(success, message, new Customer());
            }
        }
    }
}
