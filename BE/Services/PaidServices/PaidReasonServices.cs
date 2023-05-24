using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.CustomerDtos;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Dtos.PaidReasonDtos;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace BE.Services.PaidServices
{
    public interface IPaidReasonsService
    {
        Task<BaseResponse<List<PaidReasons>>> GetAllAsync();
        Task<BaseResponse<PaidReasons>> GetById(int paidReasonsId);
        Task<BaseResponse<PaidReasons>> CreatePaidReasons(CreatePaidReasonDto createPaidReasonDto);
        Task<BaseResponse<PaidReasons>> UpdatePaidReasons(int id, UpdatePaidReasonDto updatePaidReasonDto);
        Task<BaseResponse<PaidReasons>> DeletePaidReasons(int id);
    }

    public class PaidReasonServices : IPaidReasonsService
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;

        public PaidReasonServices(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<PaidReasons>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<PaidReasons>();
            try
            {
                var paidReasons = await _appContext.PaidReasons.Where(s => s.isDeleted == false).OrderBy(s => s.name).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(paidReasons);
                return (new BaseResponse<List<PaidReasons>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<PaidReasons>>(success, message, data));
            }
        }

        public async Task<BaseResponse<PaidReasons>> GetById(int paidReasonsId)
        {
            var success = false;
            var message = "";
            var data = new PaidReasons();
            try
            {
                var paidReasons = await _appContext.PaidReasons.Where(x => x.isDeleted == false && x.id == paidReasonsId).OrderByDescending(x => x.dateCreated).FirstOrDefaultAsync();

                if (paidReasons is null)
                {
                    message = "paidReasonId doesn't exist !";
                    data = null;
                    return new BaseResponse<PaidReasons>(success, message, data);
                }
                success = true;
                data = paidReasons;
                message = "Get data successfully";
                return (new BaseResponse<PaidReasons>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<PaidReasons>(success, message, data));
            }
        }

        public async Task<BaseResponse<PaidReasons>> CreatePaidReasons(CreatePaidReasonDto createPaidReasonDto)
        {
            var success = false;
            var message = "";
            try
            {
                var name = _appContext.PaidReasons.Where(u => u.name == createPaidReasonDto.name);
                if (name.Any())
                {
                    message = "Lý do chi trả đã tồn tại!";
                    return new BaseResponse<PaidReasons>(success, message, new PaidReasons());
                }
                var paidReason = _mapper.Map<PaidReasons>(createPaidReasonDto);
                paidReason.isDeleted = false;
                paidReason.dateCreated = DateTime.UtcNow;
                await _appContext.PaidReasons.AddAsync(paidReason);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Thêm lý do chi trả thành công";
                return new BaseResponse<PaidReasons>(success, message, paidReason);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Thêm lý do chi trả thất bại! {ex.Message}";
                return new BaseResponse<PaidReasons>(success, message, new PaidReasons());
            }
        }

        public async Task<BaseResponse<PaidReasons>> UpdatePaidReasons(int id, UpdatePaidReasonDto updatePaidReasonDto)
        {
            var success = false;
            var message = "";
            try
            {
                var paidReasons = await _appContext.PaidReasons.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (paidReasons is null)
                {
                    message = "Lý do chi trả không tồn tại !";
                    return new BaseResponse<PaidReasons>(success, message, new PaidReasons());
                }
                var paidReasonMapData = _mapper.Map<UpdatePaidReasonDto, PaidReasons>(updatePaidReasonDto, paidReasons);
                paidReasonMapData.dateUpdated = DateTime.UtcNow;
                _appContext.PaidReasons.Update(paidReasonMapData);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Cập nhật lý do chi trả thành công!";
                return new BaseResponse<PaidReasons>(success, message, paidReasonMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Cập nhật lý do chi trả thất bại! {ex.Message}";
                return new BaseResponse<PaidReasons>(success, message, new PaidReasons());
            }
        }

        public async Task<BaseResponse<PaidReasons>> DeletePaidReasons(int id)
        {
            var success = false;
            var message = "";
            try
            {
                var paidReasons = await _appContext.PaidReasons.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (paidReasons is null)
                {
                    message = "Lý do chi trả không tồn tại !";
                    return new BaseResponse<PaidReasons>(success, message, new PaidReasons());
                }

                paidReasons.isDeleted = true;
                paidReasons.dateDeleted = DateTime.Now;
                _appContext.PaidReasons.Update(paidReasons);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Lý do chi trả được xóa thành công!";
                return new BaseResponse<PaidReasons>(success, message, paidReasons);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xóa lý do chi trả thất bại! {ex.InnerException}";
                return new BaseResponse<PaidReasons>(success, message, new PaidReasons());
            }
        }


    }
}
