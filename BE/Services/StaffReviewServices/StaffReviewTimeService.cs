using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.StaffReviewDtos;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System;
using System.Linq;
using System.Collections.Immutable;
using static BE.Data.Constand.ConfigEntity;
using StaffReviewTime = BE.Data.Models.StaffReviewTime;
using DocumentFormat.OpenXml.InkML;

namespace BE.Services.StaffReviewServices
{
    public interface IStaffReviewTimeService
    {
        Task<BaseResponse<List<StaffReviewTime>>> GetAllStaffReviewTime();
        Task<BaseResponse<StaffReviewTime>> CreateNewStaffReviewTime(StaffReviewTimeDto staffReviewTimeDto);
        Task<BaseResponse<StaffReviewTime>> UpdateStaffReviewTime(int id, UpdateStaffViewTimeDto staffReviewTimeDto);
        Task<BaseResponse<StaffReviewTime>> DeleteStaffReviewTime(int id);
        Task<BaseResponse<List<GetAllMultiStaffViewTimeDto>>> SearchReviewerBranchReviewTime(SearchStaffReviewTimeDto searchStaffReviewTimeDto);

        Task<BaseResponse<List<StaffReviewTime>>> CreateMultiNewStaffReviewTime(CreateMultiStaffReviewTimeDto createMultiStaffReviewTimeDto);
        Task<BaseResponse<List<StaffReviewTime>>> UpdateMultiNewStaffReviewTime(int idStaffReviewTime, int idBranch, int idOffice, CreateMultiStaffReviewTimeDto createMultiStaffReviewTimeDto);
        Task<BaseResponse<List<GetAllMultiStaffViewTimeDto>>> GetAllStaffRewviewTimeAsync(int? idReviewer, int? idUserCreated);
        Task<BaseResponse<List<StaffReviewTime>>> GetAllStaffReviewWithBranch(int idBranch);
        Task<BaseResponse<List<GetAllMultiStaffViewTimeDto>>> GetByIdOfficePmOrLeadNewStaffReviewTime(int idStaffReviewTime, int idBranch, int idOffice);
        Task<BaseResponse<List<StaffReviewTime>>> DeleteMultiStaffReviewTime(int idStaffReviewTime, int idBranch, int idUserDeleted);
        Task<BaseResponse<int>> IdHandleStaffReviewTime(int idBranch);
        Task<BaseResponse<List<ResponseBranchNameDto>>> getAllReviewTimeName();
        Task<BaseResponse<List<StaffReviewTime>>> ConfirmStaffReviewTime(ConfirmStaffReviewTimeDto confirmStaffReviewTimeDto);
    }
    public class StaffReviewTimeService: IStaffReviewTimeService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        public StaffReviewTimeService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<BaseResponse<List<StaffReviewTime>>> GetAllStaffReviewTime()
        {
            var success = false;
            var message = "";
            var data = new List<StaffReviewTime>();
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(s => s.isDeleted == false)
                                                                        .OrderByDescending(s => s.dateCreated)
                                                                        .ToListAsync();
                success = true;
                message = "Nhận tất cả dữ liệu thành công!";
                data.AddRange(reviewTime);
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
        }
        public async Task<BaseResponse<StaffReviewTime>> CreateNewStaffReviewTime(StaffReviewTimeDto staffReviewTimeDto)
        {
            var success = false;
            var message = "";
            var data = new StaffReviewTime();
            try
            {
                var reviewTime = _mapper.Map<StaffReviewTime>(staffReviewTimeDto);
                reviewTime.isDeleted = false;
                reviewTime.dateCreated = DateTime.Now;
                await _appDbContext.StaffReviewTimes.AddAsync(reviewTime);
                await _appDbContext.SaveChangesAsync();
                
                success = true;
                message = "Thêm mới đợt đánh giá thành công!";
                return (new BaseResponse<StaffReviewTime>(success, message, reviewTime));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<StaffReviewTime>(success, message, data));
            }
        }
        public async Task<BaseResponse<StaffReviewTime>> UpdateStaffReviewTime(int id, UpdateStaffViewTimeDto staffReviewTimeDto)
        {
            var success = false;
            var message = "";
            var data = new StaffReviewTime();
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if(reviewTime is null)
                {
                    message = "Đợt đánh giá không tồn tại!";
                    return (new BaseResponse<StaffReviewTime>(success, message, data));
                }

                reviewTime.dateUpdated = DateTime.Now;
                var updateReviewTime = _mapper.Map<UpdateStaffViewTimeDto, StaffReviewTime>(staffReviewTimeDto, reviewTime);
                _appDbContext.StaffReviewTimes.Update(updateReviewTime);
                await _appDbContext.SaveChangesAsync();

                success = true;
                message = "Sửa đợt đánh giá thành công!";
                return (new BaseResponse<StaffReviewTime>(success, message, reviewTime));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<StaffReviewTime>(success, message, data));
            }
        }

        public async Task<BaseResponse<StaffReviewTime>> DeleteStaffReviewTime(int id)
        {
            var success = false;
            var message = "";
            var data = new StaffReviewTime();
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (reviewTime is null)
                {
                    message = "Đợt đánh giá không tồn tại !";
                    return (new BaseResponse<StaffReviewTime>(success, message, data));
                }
                reviewTime.isDeleted = true;
                reviewTime.dateDeleted = DateTime.Now;
                _appDbContext.StaffReviewTimes.Update(reviewTime);
                await _appDbContext.SaveChangesAsync();

                success = true;
                message = "Xóa đợt đánh giá thành công!";
                return (new BaseResponse<StaffReviewTime>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xóa đợt đánh giá thất bại! {ex.InnerException}";
                return (new BaseResponse<StaffReviewTime>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<GetAllMultiStaffViewTimeDto>>> SearchReviewerBranchReviewTime(SearchStaffReviewTimeDto searchStaffReviewTimeDto)
        {
            var success = false;
            var message = "";
            var data = new List<GetAllMultiStaffViewTimeDto>();
            try
            {
                var getData = await GetAllStaffRewviewTimeAsync(searchStaffReviewTimeDto.idReviewer, searchStaffReviewTimeDto.idUserCreated);
                if (getData._success)
                {
                    var result = getData._Data;
                    if (searchStaffReviewTimeDto.staffReviewTime != null && searchStaffReviewTimeDto.companyBranhId == null && searchStaffReviewTimeDto.reviewerId == null)
                    {
                        data = result.Where(s => s.staffReviewTime.Equals(searchStaffReviewTimeDto.staffReviewTime)).ToList();
                    }
                    else if (searchStaffReviewTimeDto.staffReviewTime != null && searchStaffReviewTimeDto.companyBranhId != null && searchStaffReviewTimeDto.reviewerId == null)
                    {
                        data = result.Where(s => s.staffReviewTime.Equals(searchStaffReviewTimeDto.staffReviewTime) && s.companyBranhId.Equals(searchStaffReviewTimeDto.companyBranhId)).ToList();
                    }
                    else if (searchStaffReviewTimeDto.staffReviewTime != null && searchStaffReviewTimeDto.companyBranhId != null && searchStaffReviewTimeDto.reviewerId != null)
                    {
                        data = result.Where(s => s.staffReviewTime.Equals(searchStaffReviewTimeDto.staffReviewTime) && s.companyBranhId.Equals(searchStaffReviewTimeDto.companyBranhId) && s.reviewerId.Equals(searchStaffReviewTimeDto.reviewerId)).ToList();
                    }
                    else if (searchStaffReviewTimeDto.staffReviewTime == null && searchStaffReviewTimeDto.companyBranhId != null && searchStaffReviewTimeDto.reviewerId == null)
                    {
                        data = result.Where(s => s.companyBranhId.Equals(searchStaffReviewTimeDto.companyBranhId)).ToList();
                    }
                    else if (searchStaffReviewTimeDto.staffReviewTime == null && searchStaffReviewTimeDto.companyBranhId != null && searchStaffReviewTimeDto.reviewerId != null)
                    {
                        data = result.Where(s => s.companyBranhId.Equals(searchStaffReviewTimeDto.companyBranhId) && s.reviewerId.Equals(searchStaffReviewTimeDto.reviewerId)).ToList();
                    }
                    else if (searchStaffReviewTimeDto.staffReviewTime == null && searchStaffReviewTimeDto.companyBranhId == null && searchStaffReviewTimeDto.reviewerId != null)
                    {
                        data = result.Where(s => s.reviewerId.Equals(searchStaffReviewTimeDto.reviewerId)).ToList();
                    }
                    else
                    {
                        data.AddRange(result);
                    }
                    success = true;
                    message = "Lấy dữ liệu thành công!";
                    return (new BaseResponse<List<GetAllMultiStaffViewTimeDto>>(success, message, data));
                }
                else
                {
                    success = true;
                    message = "Xảy ra lỗi trong quá trình lấy dữ liệu!";
                    return (new BaseResponse<List<GetAllMultiStaffViewTimeDto>>(success, message, data));
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<GetAllMultiStaffViewTimeDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<StaffReviewTime>>> GetAllStaffReviewWithBranch(int idBranch)
        {
            var success = false;
            var message = "";
            var data = new List<StaffReviewTime>();
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(s => s.isDeleted == false && s.companyBranhId == idBranch).ToListAsync();
                success = true;
                message = $"Nhận tất cả dữ liệu của nhánh {idBranch} thành công!";
                data.AddRange(reviewTime);
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<StaffReviewTime>>> CreateMultiNewStaffReviewTime(CreateMultiStaffReviewTimeDto createMultiStaffReviewTimeDto)
        {
            var success = false;
            var message = "";
            var data = new List<StaffReviewTime>();
            try
            {
                foreach (var item in createMultiStaffReviewTimeDto.listStaffReviews)
                {
                    var reviewTime = _mapper.Map<StaffReviewTime>(createMultiStaffReviewTimeDto);
                    reviewTime.isDeleted = false;
                    reviewTime.dateCreated = DateTime.Now;
                    reviewTime.isDeleted = false;
                    reviewTime.staffReviewId = item;
                    await _appDbContext.StaffReviewTimes.AddAsync(reviewTime);
                    data.Add(reviewTime);
                }
                await _appDbContext.SaveChangesAsync();

                foreach(var item in data)
                {
                    var addStaffReview = new CreateStaffReviewDto();
                    addStaffReview.reviewerId = createMultiStaffReviewTimeDto.reviewerId;
                    addStaffReview.staffReview = item.staffReviewId;
                    addStaffReview.dateCreated = DateTime.Now;
                    addStaffReview.IdstaffReviewDate = item.id;
                    addStaffReview.experiences = new List<CreateExperienceDto>{ new CreateExperienceDto() };
                    addStaffReview.ReviewResult = new CreateReviewResultDto();
                    var map = _mapper.Map<StaffReview>(addStaffReview);
                    map.isDeleted = false;
                    _appDbContext.StaffReviews.Add(map);
                }
                await _appDbContext.SaveChangesAsync();
                success = true;
                message = "Thêm mới đợt đánh giá thành công!";
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<StaffReviewTime>>> UpdateMultiNewStaffReviewTime(int idStaffReviewTime, int idBranch, int idOffice, CreateMultiStaffReviewTimeDto createMultiStaffReviewTimeDto)
        {
            var success = false;
            var message = "";
            var data = new List<StaffReviewTime>();
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(s => s.isDeleted == false && s.staffReviewTime.Equals(idStaffReviewTime) && s.userCreated.Equals(idOffice) && s.companyBranhId.Equals(idBranch)).ToListAsync();
                if (reviewTime.Count() > 0)
                {
                    foreach (var item in reviewTime)
                    {
                        _appDbContext.StaffReviewTimes.Remove(item);
                    }
                    await _appDbContext.SaveChangesAsync();
                }
                //foreach (var item in updateMultiStaffViewTimeDto.listStaffReviews)
                //{
                //    foreach (var staff in reviewTime)
                //    {
                //        if (staff.staffReviewId.Equals((int)item))
                //        {
                //            staff.dateUpdated = DateTime.Now;
                //            var updateStaffReviewTime = _mapper.Map<UpdateMultiStaffViewTimeDto, StaffReviewTime>(updateMultiStaffViewTimeDto, staff);
                //            _appDbContext.StaffReviewTimes.Update(updateStaffReviewTime);
                //            data.Add(updateStaffReviewTime);
                //            break;
                //        }
                //    }
                //}

                var updateReviewTime = await CreateMultiNewStaffReviewTime(createMultiStaffReviewTimeDto);
                success = true;
                message = "Sửa đợt đánh giá thành công!";
                return (new BaseResponse<List<StaffReviewTime>>(success, message, updateReviewTime._Data));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
        }

        private List<GetAllMultiStaffViewTimeDto> GetData()
        {
            var getData = (from svt in _appDbContext.StaffReviewTimes
                          join br in _appDbContext.Branchs on svt.companyBranhId equals br.idBranch
                          join rev in _appDbContext.Users on svt.reviewerId equals rev.id
                          join sta in _appDbContext.Users on svt.staffReviewId equals sta.id
                          join usct in _appDbContext.Users on svt.userCreated equals usct.id
                          join sv in _appDbContext.StaffReviews on svt.id equals sv.IdstaffReviewDate
                          where svt.isDeleted == false && br.isDeleted == 0 && sta.isDeleted == 0 && rev.isDeleted == 0
                          select new GetAllMultiStaffViewTimeDto
                          {
                              id = svt.id,
                              staffReviewTime = svt.staffReviewTime,
                              companyBranhId = svt.companyBranhId,
                              branchName = br.branchName,
                              reviewerId = svt.reviewerId,
                              fullNameReviewer = rev.FullName,
                              staffReviewId = svt.staffReviewId,
                              fullNameStaffReview = sta.FullName,
                              dateReview = svt.dateReview != DateTime.MinValue ? svt.dateReview.ToString("MM/dd/yyyy") : null,
                              detail = svt.detail ,
                              userCreated = svt.userCreated,
                              fullNameUserCreated = usct.FullName,
                              isDeleted = svt.isDeleted,
                              isAllReview = sv.IsConfirm,
                              isNotAllReview = sv.IsConfirm,
                              isConfirm = svt.isConfirm,
                          }).OrderBy(x => x.staffReviewTime).ToList();
            return getData;
        }

        private List<GetAllMultiStaffViewTimeDto> HandlerStaffReviewConfirm()
        {
            var getAllReviewTime = GetData().GroupBy(x => new { x.staffReviewTime, x.companyBranhId }).ToList();
            var result = new List<GetAllMultiStaffViewTimeDto>();
            foreach (var item in getAllReviewTime) 
            {
                var groupList = item.ToList();
                var listNameStaffReviewTime = "";
                groupList.ForEach(x => listNameStaffReviewTime += x.fullNameStaffReview + ", ");
                groupList.ForEach(x => x.fullNameStaffReviewList = listNameStaffReviewTime);

                if (item.All(x => x.isNotAllReview == true))
                {
                    groupList.ForEach(x => x.isNotAllReview = true);
                }
                else if (item.All(x => x.isNotAllReview == null))
                {
                    groupList.ForEach(x => x.isNotAllReview = false);
                }
                else if (groupList.Count() > 1)
                {
                    if (item.Any(x => x.isNotAllReview != true))
                    {
                        groupList.ForEach(x => x.isNotAllReview = true);
                    }
                }

                if (item.Any(x => x.isAllReview != true))
                {
                    groupList.ForEach(x => x.isAllReview = false);
                    result.AddRange(groupList);
                }
                else
                {
                    result.AddRange(groupList);
                }
            }
            result = result.GroupBy(x => new { x.staffReviewTime, x.companyBranhId }).Select(g => g.Count() > 1 ? g.OrderBy(x => x.staffReviewTime).Last() : g.First()).ToList();
            return result;
        }

        public async Task<BaseResponse<List<GetAllMultiStaffViewTimeDto>>> GetAllStaffRewviewTimeAsync(int? idReviewer, int? idUserCreated)
        {
            var success = false;
            var message = "";
            var data = new List<GetAllMultiStaffViewTimeDto>();
            try
            {
                //var getAllReviewTime = GetData().GroupBy(x => new { x.staffReviewTime, x.companyBranhId }).Select(g => g.Count() > 1 ? g.OrderBy(x => x.staffReviewTime).Last() : g.First()).ToList();
                var getAllReviewTime = HandlerStaffReviewConfirm();

                if (idReviewer != null)
                {
                    getAllReviewTime = getAllReviewTime.Where(s => s.reviewerId== idReviewer).ToList();
                }
                if (idUserCreated != null)
                {
                    getAllReviewTime = getAllReviewTime.Where(s => s.userCreated == idUserCreated).ToList();
                }
                success = true;
                message = "Nhận tất cả dữ liệu thành công!";
                data.AddRange(getAllReviewTime);
                return (new BaseResponse<List<GetAllMultiStaffViewTimeDto>>(success, message, data));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<List<GetAllMultiStaffViewTimeDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<GetAllMultiStaffViewTimeDto>>> GetByIdOfficePmOrLeadNewStaffReviewTime(int idStaffReviewTime,int idBranch ,int idOffice)
        {
            var success = false;
            var message = "";
            var data = new List<GetAllMultiStaffViewTimeDto>();
            try
            {
                var getAllReviewTime = GetData().Where(s => s.isDeleted == false && s.staffReviewTime.Equals(idStaffReviewTime) && s.userCreated.Equals(idOffice) && s.companyBranhId.Equals(idBranch)).ToList();
               
                success = true;
                message = "Lấy tất cả dữ liệu thành công!";
                return (new BaseResponse<List<GetAllMultiStaffViewTimeDto>>(success, message, getAllReviewTime));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<List<GetAllMultiStaffViewTimeDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<StaffReviewTime>>> DeleteMultiStaffReviewTime(int idStaffReviewTime, int idBranch, int idUserDeleted)
        {
            var success = false;
            var message = "";
            var data = new List<StaffReviewTime>();
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(s => s.isDeleted == false && s.staffReviewTime.Equals(idStaffReviewTime) && s.companyBranhId.Equals(idBranch)).ToListAsync();
                if (reviewTime.Count() > 0)
                {
                    foreach (var item in reviewTime)
                    {
                        item.isDeleted = true;
                        item.dateDeleted = DateTime.Now;
                        item.userDeleted = idUserDeleted;
                        _appDbContext.StaffReviewTimes.Update(item);
                        data.Add(item);
                    }
                    await _appDbContext.SaveChangesAsync();
                }
                success = true;
                message = "Xóa đợt đánh giá thành công!";
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xóa đợt đánh giá thất bại! {ex.InnerException}";
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
        }

        public async Task<BaseResponse<int>> IdHandleStaffReviewTime(int idBranch)
        {
            var success = false;
            var message = "";
            var data = 1;
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(s => s.isDeleted == false && s.companyBranhId.Equals(idBranch)).GroupBy(x => x.staffReviewTime).Select(g => g.Count() > 1 ? g.OrderBy(x => x.staffReviewTime).Last() : g.First()).ToListAsync();

                if (reviewTime.Count() > 0)
                {
                    var getLast = reviewTime.OrderBy(x => x.staffReviewTime).LastOrDefault();
                    data = getLast.staffReviewTime + 1;
                }

                success = true;
                message = "Lấy dữ liệu thành công!";
                return (new BaseResponse<int>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Lấy dữ liệu thất bại! {ex.InnerException}";
                return (new BaseResponse<int>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<ResponseBranchNameDto>>> getAllReviewTimeName()
        {
            var success = false;
            var message = "";
            var data = new List<ResponseBranchNameDto>();
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(x => x.isDeleted == false).GroupBy(x => x.staffReviewTime).Select(g => g.Count() > 1 ? g.OrderBy(x => x.staffReviewTime).Last() : g.First()).ToListAsync();
                var result = reviewTime.Select(s => new ResponseBranchNameDto
                {
                    id = s.staffReviewTime,
                    name = "Đợt " + s.staffReviewTime,

                }).ToList();

                success = true;
                message = "Lấy dữ liệu thành công!";
                return (new BaseResponse<List<ResponseBranchNameDto>>(success, message, result));
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Lấy dữ liệu thất bại! {ex.InnerException}";
                return (new BaseResponse<List<ResponseBranchNameDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<StaffReviewTime>>> ConfirmStaffReviewTime(ConfirmStaffReviewTimeDto confirmStaffReviewTimeDto)
        {
            var success = false;
            var message = "";
            var data = new List<StaffReviewTime>();
            try
            {
                var reviewTime = await _appDbContext.StaffReviewTimes.Where(s => s.isDeleted == false && s.staffReviewTime.Equals(confirmStaffReviewTimeDto.staffReviewTime) && s.companyBranhId.Equals(confirmStaffReviewTimeDto.companyBranhId) && s.reviewerId.Equals(confirmStaffReviewTimeDto.reviewerId)).ToListAsync();
                if (reviewTime.Count() > 0)
                {
                    foreach (var item in reviewTime)
                    {
                        item.isConfirm = true;
                        item.dateDeleted = DateTime.Now;
                        item.userUpdated = confirmStaffReviewTimeDto.userUpdated;
                        _appDbContext.StaffReviewTimes.Update(item);
                        data.Add(item);
                    }
                    await _appDbContext.SaveChangesAsync();
                }
                success = true;
                message = "Xác nhận hoàn thành đợt đánh giá thành công!";
                return (new BaseResponse<List<StaffReviewTime>>(success, message, reviewTime));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<List<StaffReviewTime>>(success, message, data));
            }
        }


    }
}
