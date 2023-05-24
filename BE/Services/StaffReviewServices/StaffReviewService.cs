using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.StaffReviewDtos;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;
using static BE.Data.Constand.ConfigEntity;

namespace BE.Services.StaffReviewServices
{
    public interface IStaffReviewService
    {
        Task<BaseResponse<List<StaffReviewDto>>> GetAllStaffReview();
        Task<BaseResponse<StaffReview>> CreateStaffReview(CreateStaffReviewDto staffReviewDto);
        Task<BaseResponse<int>> TimesReview(int idStaff, int idReviewer);
        Task<BaseResponse<List<StaffReviewDto>>> GetReviewByIdStaff(int idStaff, int idReviewer);
        Task<BaseResponse<StaffReviewDto>> GetReviewById(int id);
        Task<BaseResponse<StaffReview>> ConfirmReview(int id, int idAccepted);
        Task<BaseResponse<StaffReview>> RejectReview(int id, RejectReviewDto rejectReviewDto);
        Task<BaseResponse<StaffReview>> EditReview(int id, CreateStaffReviewDto staffReviewDto);
        Task<BaseResponse<List<StaffReviewDto>>> GetAllStaffReviewByOffice(int idReviewTime, int idBranch, int idReviewer);
        Task<BaseResponse<List<StaffReviewDto>>> GetReviewByUserCreated(int idReviewTime, int idBranch, int idReviewer);
        /* Task<BaseResponse<List<StaffReviewDto>>> SearchReviewByStaffName(string name);
         Task<BaseResponse<List<StaffReviewDto>>> SearchByReviewerId(int id);*/
        Task<BaseResponse<List<StaffReviewDto>>> SearchReviewByStaffNameOrReviewerId(SearchReviewByStaffNameOrIDReviewerDto searchReviewByStaffNameOrIDReviewerDto);
        Task<BaseResponse<List<StaffReviewDto>>> GetReviewTicketByStaffIdAndReviewerId(int staffId, int reviewerId);
        Task<BaseResponse<List<StaffReviewDto>>> GetReviewTicketByLeadIdAndReviewerId(int staffId, int reviewerId);
        Task<BaseResponse<StaffReview>> DeleteReview(int id);
        Task<BaseResponse<List<StaffReview>>> DeleteReviewByUser(int idUser);

        Task<BaseResponse<List<StaffReviewDto>>> GetAllStaffReviewByIdReviewTime(int idReviewTime, int idBranch, int idReviewer);
        Task<BaseResponse<StaffReview>> ConfirmUpdateReview(int idReview, int idUserUpdate);
    }
    public class StaffReviewService : IStaffReviewService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StaffReviewService(AppDbContext appcontext, IMapper mapper)
        {
            _context = appcontext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<StaffReviewDto>>> GetAllStaffReview()
        {
            var getTerminate = _context.ReviewResults
                                        .Include(s => s.StaffReview)
                                        .Where(x => x.isTerminate == true && x.StaffReview.IsConfirm == true)
                                        .Select(s => s.StaffReview.staffReview)
                                        .ToList();

            var getAll = await _context.StaffReviews
                                 .Include(x => x.ReviewResult)
                                 .Include(x => x.experiences)
                                 .Include(x=> x.StaffReviewTime)
                                 .Where(x => !getTerminate.Contains(x.staffReview ?? 0) && x.isDeleted == false)
                                 .GroupBy(x => x.staffReview)
                                 .Select(g => g.Count() > 1 ? g.OrderBy(x => x.id).Last() : g.First())
                                 .ToListAsync();


            if (getAll == null)
                return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", null);

            var result = _mapper.Map<List<StaffReviewDto>>(getAll);

            return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", result);
        }
        public async Task<BaseResponse<List<StaffReviewDto>>> GetAllStaffReviewByIdReviewTime(int idReviewTime, int idBranch, int idReviewer)
        {
            var reviewTimes = await _context.StaffReviewTimes.Where(s => s.isDeleted == false && s.staffReviewTime.Equals(idReviewTime) && s.companyBranhId.Equals(idBranch) && s.reviewerId.Equals(idReviewer)).Select(s => s.id).ToListAsync();
            var getTerminate = _context.ReviewResults
                                        .Include(s => s.StaffReview)
                                        .Where(x => x.isTerminate == true && x.StaffReview.IsConfirm == true)
                                        .Select(s => s.StaffReview.staffReview)
                                        .ToList();

            var getAll = await _context.StaffReviews
                                 .Include(x => x.ReviewResult)
                                 .Include(x => x.experiences)
                                 .Include(x => x.StaffReviewTime)
                                 .Where(x => !getTerminate.Contains(x.staffReview ?? 0) && x.isDeleted == false && reviewTimes.Contains(x.IdstaffReviewDate))
                                 .GroupBy(x => x.staffReview)
                                 .Select(g => g.Count() > 1 ? g.OrderBy(x => x.id).Last() : g.First())
                                 .ToListAsync();


            if (getAll == null)
                return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", null);

            var result = _mapper.Map<List<StaffReviewDto>>(getAll);

            return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", result);
        }

        public async Task<BaseResponse<StaffReview>> CreateStaffReview(CreateStaffReviewDto staffReviewDto)
        {
            var map = _mapper.Map<StaffReview>(staffReviewDto);
            _context.StaffReviews.Add(map);
            _context.SaveChanges();
            return new BaseResponse<StaffReview>(true, "Create Staff Review Ticket Successfully", map);
        }
        public async Task<BaseResponse<int>?> TimesReview(int idStaff, int idReviewer)
        {
            var checkUser = await _context.Users.Where(x => x.id == idStaff).FirstOrDefaultAsync();
            if (checkUser is null)
            {
                return new BaseResponse<int>(false, "Staff is invalid", 0);
            }
            var check = _context.StaffReviews.Count(x => x.staffReview == idStaff && x.reviewerId == idReviewer && x.IsConfirm == true && x.isDeleted == false);

            if (check.Equals(0))
            {
                return new BaseResponse<int>(true, "Staff is haven't review", 0);
            }
            return new BaseResponse<int>(true, $"Find All Result Review of staff {idStaff} Successfully", check);
        }
        public async Task<BaseResponse<List<StaffReviewDto>>> GetReviewByIdStaff(int idStaff, int idReviewer)
        {
            var reviewTimes = await _context.StaffReviewTimes.Where(s => s.isDeleted == false && s.staffReviewId.Equals(idStaff) && s.reviewerId.Equals(idReviewer)).Select(s => s.id).ToListAsync();
            var checkUser = await _context.Users.Where(x => x.id == idStaff).FirstOrDefaultAsync();
            if (checkUser is null)
            {
                return new BaseResponse<List<StaffReviewDto>>(false, "Staff is invalid", null);
            }
            var check = await _context.StaffReviews.Where(x => x.staffReview == idStaff)
                                                    .Include(x => x.ReviewResult)
                                                    .Include(x => x.experiences)
                                                    .Include(x=> x.StaffReviewTime)
                                                    .Where(x => x.IsConfirm == true && x.isDeleted == false && reviewTimes.Contains(x.IdstaffReviewDate))
                                                    .ToListAsync();
            if (check.Count <= 0)
            {
                return new BaseResponse<List<StaffReviewDto>>(true, "Staff is haven't review", null);
            }
            var result = _mapper.Map<List<StaffReviewDto>>(check);
            return new BaseResponse<List<StaffReviewDto>>(true, $"Find All Review of staff {idStaff} Successfully", result);
        }
        public async Task<BaseResponse<StaffReviewDto>> GetReviewById(int id)
        {
            var check = await _context.StaffReviews.Where(x => x.id == id && x.isDeleted == false).Include(x => x.ReviewResult).Include(x => x.experiences).Include(x => x.StaffReviewTime).SingleOrDefaultAsync();
            if (check is null)
            {
                return new BaseResponse<StaffReviewDto>(false, "Review Ticket Is Invalid", null);
            }
            var result = _mapper.Map<StaffReviewDto>(check);
            return new BaseResponse<StaffReviewDto>(true, $"Find Review By {id} Successfully", result);
        }
        public async Task<BaseResponse<StaffReview>> ConfirmReview(int id, int idAccepted)
        {
            var check = await _context.StaffReviews.Where(x => x.id == id && x.isDeleted == false).SingleOrDefaultAsync();
            if (check is null)
            {
                return new BaseResponse<StaffReview>(false, "Review Ticket Is Invalid", new StaffReview());
            }
            check.IsConfirm = true;
            check.dateUpdated = DateTime.Now;
            check.Approver = idAccepted;
            _context.StaffReviews.Update(check);
            await _context.SaveChangesAsync();
            return new BaseResponse<StaffReview>(true, "Confirm Review Successfully", check);
        }
        public async Task<BaseResponse<StaffReview>> RejectReview(int id, RejectReviewDto rejectReviewDto)
        {
            var check = await _context.StaffReviews.Where(x => x.id == id && x.isDeleted == false).SingleOrDefaultAsync();
            if (check is null)
            {
                return new BaseResponse<StaffReview>(false, "Review Ticket Is Invalid", new StaffReview());
            }
            check.IsConfirm = false;
            check.ReasonNotConfirm = rejectReviewDto.reason;
            check.Approver = rejectReviewDto.idRejecters;
            check.dateUpdated = DateTime.Now;
            _context.StaffReviews.Update(check);
            await _context.SaveChangesAsync();
            return new BaseResponse<StaffReview>(true, "Reject Review Successfuly", check);
        }
        public async Task<BaseResponse<StaffReview>> EditReview(int id, CreateStaffReviewDto staffReviewDto)
        {
            var check = await _context.StaffReviews.Include(x => x.ReviewResult).Include(x => x.experiences).Include(x=>x.StaffReviewTime).Where(x => x.id == id && x.isDeleted == false).FirstOrDefaultAsync();
            if (check is null)
            {
                return new BaseResponse<StaffReview>(false, "Review Ticket Is Invalid", null);
            }

            var map = _mapper.Map(staffReviewDto,check);
            _context.StaffReviews.Update(map);
            _context.ReviewResults.Update(map.ReviewResult);
            _context.Experiences.UpdateRange(map.experiences);
            _context.StaffReviewTimes.UpdateRange(map.StaffReviewTime);
            await _context.SaveChangesAsync();
            return new BaseResponse<StaffReview>(true, "Edit Review Successfully", null);
        }

        public async Task<BaseResponse<List<StaffReviewDto>>> GetAllStaffReviewByOffice(int idReviewTime, int idBranch, int idReviewer)
        {
            var reviewTimes = await _context.StaffReviewTimes.Where(s => s.isDeleted == false && s.staffReviewTime.Equals(idReviewTime) && s.companyBranhId.Equals(idBranch) && s.reviewerId.Equals(idReviewer)).Select(s => s.id).ToListAsync();
            var getTerminate = _context.ReviewResults
                                        .Include(s => s.StaffReview)
                                        .Where(x => x.isTerminate == true && x.StaffReview.IsConfirm == true)
                                        .Select(s => s.StaffReview.staffReview)
                                        .ToList();

            var getAll = await _context.StaffReviews
                                 .Include(x => x.ReviewResult)
                                 .Include(x => x.experiences)
                                 .Include(x=>x.StaffReviewTime)
                                 .Where(x => !getTerminate.Contains(x.staffReview ?? 0) && x.IsConfirm == true && x.isDeleted == false && reviewTimes.Contains(x.IdstaffReviewDate))
                                 .GroupBy(x => x.staffReview)
                                 .Select(g => g.Count() > 1 ? g.OrderBy(x => x.id).Last() : g.First())
                                 .ToListAsync();


            if (getAll == null)
                return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", null);

            var result = _mapper.Map<List<StaffReviewDto>>(getAll);

            return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review by Office Successfully", result);
        }

        public async Task<BaseResponse<List<StaffReviewDto>>> GetReviewByUserCreated(int idReviewTime, int idBranch, int idReviewer)
        {
            var reviewTimes = await _context.StaffReviewTimes.Where(s => s.isDeleted == false && s.staffReviewTime.Equals(idReviewTime) && s.companyBranhId.Equals(idBranch) && s.reviewerId.Equals(idReviewer)).Select(s => s.id).ToListAsync();

            var getAll = await _context.StaffReviews
                .Where(x => x.isDeleted == false && reviewTimes.Contains(x.IdstaffReviewDate))
                .Include(x => x.ReviewResult)
                .Include(x => x.experiences).Include(x=> x.StaffReviewTime)
                .GroupBy(x => x.staffReview)
                .Select(g => g.Count() > 1 ? g.OrderBy(x => x.id).Last() : g.First())
                .ToListAsync();
            var result = _mapper.Map<List<StaffReviewDto>>(getAll);
            if (getAll.Count == 0) return new BaseResponse<List<StaffReviewDto>>(true, "User not created any review ticket", result);
            else return new BaseResponse<List<StaffReviewDto>>(true, "Get all ticket review by User created successfully", result);
        }

        public async Task<BaseResponse<List<StaffReviewDto>>> SearchReviewByStaffNameOrReviewerId(SearchReviewByStaffNameOrIDReviewerDto searchReviewByStaffNameOrIDReviewerDto)
        {
            var query = _context.StaffReviews
                .Include(x => x.ReviewResult)
                .Include(x => x.experiences)
                .Include(x => x.StaffReviewTime)
                .Where(x => x.StaffReviewTime.staffReviewTime == searchReviewByStaffNameOrIDReviewerDto.staffReviewTime && x.StaffReviewTime.companyBranhId == searchReviewByStaffNameOrIDReviewerDto.companyBranhId).AsQueryable();
               
            if (searchReviewByStaffNameOrIDReviewerDto.id != 0)
            {
                var check = await _context.Users.Where(x => x.id == searchReviewByStaffNameOrIDReviewerDto.id).FirstOrDefaultAsync();
                if (check is null)
                    return new BaseResponse<List<StaffReviewDto>>(false, "User is invalid", null);

                query = query.Where(x => x.staffReview == searchReviewByStaffNameOrIDReviewerDto.id);
            }

            var getTerminate = _context.ReviewResults
                .Include(s => s.StaffReview)
                .Where(x => x.isTerminate == true && x.StaffReview.IsConfirm == true )
                .Select(s => s.StaffReview.staffReview);

            var getAll = await query
                .Where(x => !getTerminate.Contains(x.staffReview ?? 0) && x.isDeleted == false)
                .GroupBy(x => x.staffReview)
                .Select(g => g.Count() > 1 ? g.OrderBy(x => x.id).Last() : g.First())
                .ToListAsync();

            var getUserGroup = await _context.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(searchReviewByStaffNameOrIDReviewerDto.idUserCurrent)).Select(s => s.idGroup).ToListAsync();

            if (getUserGroup.Contains(1) || getUserGroup.Contains(5)) ;
            else if (getUserGroup.Contains(3))
            {
                getAll = getAll.Where(s => s.reviewerId.Equals(searchReviewByStaffNameOrIDReviewerDto.idUserCurrent)).ToList();
            }
            else if (getUserGroup.Contains(2))
            {
                getAll = getAll.Where(s => s.IsConfirm== true).ToList();
            }

            if (getAll == null)
            return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", null);

            var result = _mapper.Map<List<StaffReviewDto>>(getAll);

            return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", result);
        }
    
        public async Task<BaseResponse<List<StaffReviewDto>>> GetReviewTicketByStaffIdAndReviewerId(int staffId, int reviewerId)
        {
            var checkStaff = await _context.Users.Where(x => x.id == staffId).FirstOrDefaultAsync();
            if(checkStaff is null) return new BaseResponse<List<StaffReviewDto>>(false, "Staff is invalid", null);

            var checkReviewer = await _context.Users.Where(x => x.id == reviewerId).FirstOrDefaultAsync();
            if (checkReviewer is null) return new BaseResponse<List<StaffReviewDto>>(false, "Reviewer is invalid", null);

            var getTerminate = _context.ReviewResults
                                               .Include(s => s.StaffReview)
                                               .Where(x => x.isTerminate == true && x.StaffReview.IsConfirm == true)
                                               .Select(s => s.StaffReview.staffReview)
                                               .ToList();

            var getAll = await _context.StaffReviews
                                        .Include(x => x.ReviewResult)
                                        .Include(x => x.experiences)
                                        .Include(x=> x.StaffReviewTime)
                                        .Where(x => !getTerminate.Contains(x.staffReview ?? 0) && x.reviewerId == reviewerId && x.staffReview == staffId && x.isDeleted == false)
                                        .GroupBy(x => x.staffReview)
                                        .Select(g => g.Count() > 1 ? g.OrderBy(x => x.id).Last() : g.First())
                                        .ToListAsync();

            if (getAll == null)
                return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", null);

            var result = _mapper.Map<List<StaffReviewDto>>(getAll);

            return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", result);
        }

        public async Task<BaseResponse<List<StaffReviewDto>>> GetReviewTicketByLeadIdAndReviewerId(int staffId, int reviewerId)
        {
            var checkStaff = await _context.Users.Where(x => x.id == staffId).FirstOrDefaultAsync();
            if (checkStaff is null) return new BaseResponse<List<StaffReviewDto>>(false, "Staff is invalid", null);

            var checkReviewer = await _context.Users.Where(x => x.id == reviewerId).FirstOrDefaultAsync();
            if (checkReviewer is null) return new BaseResponse<List<StaffReviewDto>>(false, "Reviewer is invalid", null);

            //var getTerminate = _context.ReviewResults
            //                                   .Include(s => s.StaffReview)
            //                                   .Where(x => x.isTerminate == true)
            //                                   .Select(s => s.StaffReview.staffReview)
            //                                   .ToList();

            var getAll = await _context.StaffReviews
                                        .Include(x => x.ReviewResult)
                                        .Include(x => x.experiences)
                                        .Include(x=> x.StaffReviewTime)
                                        .Where(x => x.reviewerId == reviewerId && x.staffReview == staffId && x.isDeleted == false)
                                        .ToListAsync();

            if (getAll == null)
                return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Lead Review Successfully", null);

            var result = _mapper.Map<List<StaffReviewDto>>(getAll);

            return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Lead Review Successfully", result);
        }

        public async Task<BaseResponse<StaffReview>> DeleteReview(int id)
        {
            var success = false;
            var message = "";
            var data = new StaffReview();
            try
            {
                var review = await _context.StaffReviews.Where(s => s.id.Equals(id) && s.isDeleted == false).FirstOrDefaultAsync();
                if (review is null)
                {
                    success = false;
                    message = "Review doesn't exist!";
                    return new BaseResponse<StaffReview>(success, message, null);
                }
                review.dateDeleted = DateTime.Now;
                review.isDeleted = true;
                _context.StaffReviews.Update(review);

                var reviewTime = await _context.StaffReviewTimes.Where(x => x.id.Equals(review.IdstaffReviewDate)).FirstOrDefaultAsync();
                if (reviewTime != null)
                {
                    reviewTime.isDeleted = true;
                    reviewTime.dateDeleted = DateTime.Now;
                    _context.StaffReviewTimes.Update(reviewTime);
                }

                await _context.SaveChangesAsync();
                success = true;
                message = "Deleting Review successfully";
                return new BaseResponse<StaffReview>(success, message, review);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Review failed! {ex.InnerException}";
                return new BaseResponse<StaffReview>(success, message, null);
            }
        }
        public async Task<BaseResponse<List<StaffReview>>> DeleteReviewByUser(int idUser)
        {
            var success = false;
            var message = "";
            var data = new StaffReview();
            try
            {
                var reviews = await _context.StaffReviews.Where(s => s.staffReview.Equals(idUser) && s.isDeleted == false).ToListAsync();
                if (reviews is null)
                {
                    success = false;
                    message = "Review doesn't exist!";
                    return new BaseResponse<List<StaffReview>> (success, message, null);
                }
                foreach(var review in reviews)
                {
                    review.dateDeleted = DateTime.Now;
                    review.isDeleted = true;
                    _context.StaffReviews.Update(review);

                    var reviewTime = await _context.StaffReviewTimes.Where(x => x.id.Equals(review.IdstaffReviewDate)).FirstOrDefaultAsync();
                    if (reviewTime != null)
                    {
                        reviewTime.isDeleted = true;
                        reviewTime.dateDeleted = DateTime.Now;
                        _context.StaffReviewTimes.Update(reviewTime);
                    }
                }               
                await _context.SaveChangesAsync();
                success = true;
                message = "Deleting Review successfully";
                return new BaseResponse<List<StaffReview>> (success, message, reviews);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Review failed! {ex.InnerException}";
                return new BaseResponse<List<StaffReview>> (success, message, null);
            }
        }

        public async Task<BaseResponse<StaffReview>> ConfirmUpdateReview(int idReview, int idUserUpdate)
        {
            var check = await _context.StaffReviews.Where(x => x.id == idReview && x.isDeleted == false).SingleOrDefaultAsync();
            if (check is null)
            {
                return new BaseResponse<StaffReview>(false, "Review Ticket Is Invalid", new StaffReview());
            }
            check.IsConfirm = null;
            check.userUpdated = idUserUpdate;
            check.dateUpdated = DateTime.Now;
            _context.StaffReviews.Update(check);
            await _context.SaveChangesAsync();
            return new BaseResponse<StaffReview>(true, "Reject Review Successfuly", check);
        }

    }
}

