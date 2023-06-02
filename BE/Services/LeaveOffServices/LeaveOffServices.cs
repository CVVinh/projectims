using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Extentions;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static BE.Data.Enum.LeaveOff.Status;

namespace BE.Services.LeaveOffServices
{
    public interface ILeaveOffServices
    {
        Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllAsync();
        Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllAsyncByLead(int id);
        Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllAsyncByPm(int id);
        Task<BaseResponse<LeaveOff>> AddNewLeaveOffAsync(AddNewLeaveOffDto addNewLeaveOffDto);
        Task<BaseResponse<LeaveOff>> EditRegisterLeaveOffAsync(int id, EditRegisterLeaveOffDtos editRegisterLeaveOffDtos);
        Task<BaseResponse<LeaveOff>> AccepterLeaveOffAsync(int idLeaveOff, AccepterLeaveOffDto accepterLeaveOff);
        Task<BaseResponse<LeaveOff>> NotAccepterLeaveOffAsync(int idLeaveOff, NotAcceptLeaveOffDto notAcceptLeaveOffDto);
        Task<BaseResponse<LeaveOff>> GetLeaveOffByIdAsync(int idLeaveOff);
        Task<BaseResponse<LeaveOff>> DeleteLeaveOffByIdAsync(int idLeaveOff);
        Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffByStatus(StatusLO statusLO);
        Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffOfIdUserByStatus(int idLeaveOffUser, StatusLO statusLO);
        Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllLeaveOffByYear(FindByNameStatusDateDtos infoDtos);
        Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllLeaveOffByNameStatusDate(FindByNameStatusDateDtos infoDtos);
        Task<BaseResponse<List<ListLeaveOffInfo>>> getAllLeaveOffInfo();
        Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllLeaveOffByNameStatusDateLead(int id, FindByNameStatusDateDtos infoDtos);
        Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllLeaveOffByNameStatusDatePM(int id, FindByNameStatusDateDtos infoDtos);
        Task<BaseResponse<UserInfoLeaveOffDtos>> GetInfoUserLeaveOff(int idLeaveOffUser, DateTime date);
    }

    public class LeaveOffServices : ILeaveOffServices
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;
        public LeaveOffServices(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<LeaveOff>> AccepterLeaveOffAsync(int idLeaveOff, AccepterLeaveOffDto accepterLeaveOff)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == idLeaveOff && lo.status == StatusLO.Waiting)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }

                var leaveOffMap = leaveOff.AccepterLeaveOffExtention(accepterLeaveOff.ReasonAccept, accepterLeaveOff.idAcceptUser, StatusLO.Done);
                _appContext.leaveOffs.Update(leaveOffMap);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Chấp nhận nghỉ phép thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Chấp nhận nghỉ không thành công! {ex.InnerException}";
                data = null;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<LeaveOff>> AddNewLeaveOffAsync(AddNewLeaveOffDto addNewLeaveOffDto)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var date = DateTime.Now;
                var leaveOff = _mapper.Map<LeaveOff>(addNewLeaveOffDto);
                if ((leaveOff.startTime <= date && leaveOff.endTime <= date) ||
                            (leaveOff.startTime.Date <= date.Date && leaveOff.endTime <= date))
                //if ((leaveOff.endTime - leaveOff.startTime).TotalHours > 24 && leaveOff.endTime <= date)s
                {
                    leaveOff.reasons += " (Nghỉ đột xuất)";
                    leaveOff.ReasonAccept = "Duyệt bù do nghỉ đột xuất.";
                    leaveOff.status = StatusLO.Done;
                    leaveOff.idAcceptUser = 38;
                }
                await _appContext.leaveOffs.AddAsync(leaveOff.addNewLeaveOffExtention());
                var getUser = await _appContext.Users.Where(x => x.id.Equals(leaveOff.idLeaveUser)).FirstOrDefaultAsync();

                if (getUser != null)
                {
                    Notification noti = new Notification()
                    {
                        requestUser = leaveOff.idLeaveUser,
                        message = leaveOff.reasons,
                        title = $"Nghỉ phép từ nhân viên '{getUser.FullName}'",
                        isWatched = false,
                        userCreated = leaveOff.idLeaveUser,
                        link = "/leaveOffs/acceptregisterlists",
                        dateCreated = DateTime.Now,
                    };
                    _appContext.Notifications.Add(noti);
                }
                else
                {
                    Notification noti = new Notification()
                    {
                        requestUser = leaveOff.idLeaveUser,
                        message = leaveOff.reasons,
                        title = $"Nghỉ phép từ nhân viên 'Ẩn danh'",
                        isWatched = false,
                        userCreated = leaveOff.idLeaveUser,
                        link = "/leaveOffs/acceptregisterlists",
                        dateCreated = DateTime.Now,
                    };
                    _appContext.Notifications.Add(noti);
                }

                await _appContext.SaveChangesAsync();

                success = true;
                message = "Thêm ngày nghỉ mới thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Thêm ngày nghỉ mới không thành công! {ex.Message}";
                data = null;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<LeaveOff>> DeleteLeaveOffByIdAsync(int idLeaveOff)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == idLeaveOff)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    success = false;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }

                _appContext.leaveOffs.Remove(leaveOff);
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Xóa nghỉ phép thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Xóa nghỉ phép không thành công! {ex.InnerException}";
                data = null;
                success = false;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<LeaveOff>> EditRegisterLeaveOffAsync(int id, EditRegisterLeaveOffDtos editRegisterLeaveOffDtos)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == id && lo.status == StatusLO.Waiting)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    success = false;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }
                var leaveOffMap = editRegisterLeaveOffDtos.editRegisterLeaveOffExtention(leaveOff);
                _appContext.leaveOffs.Update(leaveOffMap);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Chỉnh sửa nghỉ phép thành công";
                data = leaveOffMap;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Không thể chỉnh sửa nghỉ phép! {ex.Message}";
                data = null;
                success = false;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<ListLeaveOffInfo>();
            try
            {
                var list = await _appContext.leaveOffs
                .Join(_appContext.Users, x => x.idLeaveUser, c => c.id, (x, c) => new { Leave = x, LeaveUser = c })
                .OrderByDescending(x => x.Leave.createTime)
                .Select(x => new ListLeaveOffInfo
                {
                    id = x.Leave.id,
                    idLeaveUser = x.Leave.idLeaveUser,
                    idAcceptUser = x.Leave.idAcceptUser,
                    startTime = x.Leave.startTime,
                    endTime = x.Leave.endTime,
                    reasons = x.Leave.reasons,
                    status = x.Leave.status,
                    createTime = x.Leave.createTime,
                    acceptTime = x.Leave.acceptTime,
                    ReasonNotAccept = x.Leave.ReasonNotAccept,
                    idCompanyBranh = x.Leave.idCompanyBranh,
                    ReasonAccept = x.Leave.ReasonAccept,
                    name = x.LeaveUser.FullName,
                    userCode = x.LeaveUser.userCode,
                    flagOnDay = x.Leave.flagOnDay
                }).ToListAsync();

                var now = DateTime.Now;
                var update = list.AsEnumerable()
                    .Where(s => s.status == StatusLO.Waiting).Select(s => s.id).ToList();

                var findLeavOff = await _appContext.leaveOffs.Where(s => update.Contains(s.id)).ToListAsync();

                var map = findLeavOff.Where(s => s.status == StatusLO.Waiting && s.endTime.Date < now.Date
                                || (s.endTime.Date == now.Date
                                && s.endTime.Hour < now.Hour)
                                || (s.endTime.Date == now.Date
                                && s.endTime.Hour == now.Hour
                                && s.endTime.Minute < now.Minute)).ToList();

                map.ForEach(res =>
                {
                    res.ReasonAccept = "Quá hạn duyệt";
                    res.status = StatusLO.Cancel;
                });

                await _appContext.SaveChangesAsync();

                data = list;
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
        }
        public async Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffByStatus(StatusLO statusLO)
        {
            var success = false;
            var message = "";
            var data = new List<LeaveOff>();
            try
            {
                var listLeaveOff = await _appContext.leaveOffs.Where(lo => lo.status == statusLO)
                                                              .OrderByDescending(t => t.createTime)
                                                              .ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                data.AddRange(listLeaveOff);
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffOfIdUserByStatus(int idLeaveOffUser, StatusLO statusLO)
        {
            var success = false;
            var message = "";
            var data = new List<LeaveOff>();
            try
            {
                if ((int)statusLO == 0)
                {
                    var listAllLeaveOff = await _appContext.leaveOffs.Where(lo => lo.idLeaveUser == idLeaveOffUser)
                                                              .OrderByDescending(t => t.createTime)
                                                              .ToListAsync();

                    //var date = DateTime.Now;
                    //var update = listAllLeaveOff.AsEnumerable()
                    //    .Where(s => (int)s.status != 3 && (int)s.status != 2 && s.startTime < date && s.endTime < date)
                    //    .Select(s => s.id).ToList();

                    //var map = await _appContext.leaveOffs.Where(s => update.Contains(s.id)).ToListAsync();

                    //map.ForEach(res =>
                    //{
                    //    res.ReasonNotAccept = "Quá hạn duyệt";
                    //    res.status = StatusLO.Cancel;
                    //});

                    //await _appContext.SaveChangesAsync();

                    success = true;
                    message = "Lấy tất cả dữ liệu thành công";
                    data.AddRange(listAllLeaveOff);
                    return (new BaseResponse<List<LeaveOff>>(success, message, data));
                }

                var listLeaveOff = await _appContext.leaveOffs.Where(lo => lo.status == statusLO && lo.idLeaveUser == idLeaveOffUser)
                                                              .OrderByDescending(t => t.createTime)
                                                              .ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                data.AddRange(listLeaveOff);
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
        }

        public async Task<BaseResponse<LeaveOff>> GetLeaveOffByIdAsync(int idLeaveOff)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == idLeaveOff)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    success = false;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }

                success = true;
                message = "Lấy thẻ nghĩ phép thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Lấy thẻ nghĩ phép không thành công! {ex.Message}";
                data = null;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<LeaveOff>> NotAccepterLeaveOffAsync(int idLeaveOff, NotAcceptLeaveOffDto notAcceptLeaveOffDto)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == idLeaveOff && lo.status == StatusLO.Waiting)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    success = false;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }

                _appContext.leaveOffs.Update(leaveOff.NotAcceptLeaveOffExtention(notAcceptLeaveOffDto, StatusLO.Cancel));
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Không chấp nhận nghỉ phép thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Không chấp nhận nghỉ phép không thành công! {ex.InnerException}";
                data = null;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllLeaveOffByNameStatusDate(FindByNameStatusDateDtos infoDtos)
        {
            var success = false;
            var message = "";
            var data = new List<ListLeaveOffInfo>();

            if (infoDtos.fullName == null && infoDtos.status == null && infoDtos.date == null)
            {
                success = false;
                message = "Nghỉ phép không thành công! Thông số đầu vào không được cung cấp.";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }

            try
            {
                var query = _appContext.leaveOffs.AsQueryable();

                if (String.IsNullOrEmpty(infoDtos.fullName) == false)
                {
                    var checkName = _appContext.Users
                        .Where(q => (q.firstName + " " + q.lastName).ToLower().Trim().Contains(infoDtos.fullName.Trim().ToLower()))
                        .ToList();
                    var idList = checkName.Select(item => item.id).ToList();
                    query = query.Where(x => idList.Contains(x.idLeaveUser));
                }

                if (infoDtos.status.Count > 0)
                {
                    var statusList = infoDtos.status.ToList();
                    query = query.Where(x => statusList.Contains((int)x.status));
                }

                if (infoDtos.date != null)
                {
                    query = query.Where(x => (x.startTime.Month <= infoDtos.date.Value.Month && x.startTime.Year == infoDtos.date.Value.Year)
                        && (x.endTime.Month >= infoDtos.date.Value.Month && x.endTime.Year == infoDtos.date.Value.Year));
                }

                var list = await query
                   .Join(_appContext.Users, x => x.idLeaveUser, c => c.id, (x, c) => new { Leave = x, LeaveUser = c })
                   .OrderBy(x => x.Leave.createTime)
                   .Select(x => new ListLeaveOffInfo
                   {
                       id = x.Leave.id,
                       idLeaveUser = x.Leave.idLeaveUser,
                       idAcceptUser = x.Leave.idAcceptUser,
                       startTime = x.Leave.startTime,
                       endTime = x.Leave.endTime,
                       reasons = x.Leave.reasons,
                       status = x.Leave.status,
                       createTime = x.Leave.createTime,
                       acceptTime = x.Leave.acceptTime,
                       ReasonNotAccept = x.Leave.ReasonNotAccept,
                       idCompanyBranh = x.Leave.idCompanyBranh,
                       ReasonAccept = x.Leave.ReasonAccept,
                       name = x.LeaveUser.FullName,
                       userCode = x.LeaveUser.userCode,
                       flagOnDay = x.Leave.flagOnDay
                   }).ToListAsync();
                data = list;
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Nghỉ phép không thành công! Đã xảy ra lỗi: " + ex.Message;
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllLeaveOffByYear(FindByNameStatusDateDtos infoDtos)
        {
            var success = false;
            var message = "";
            var data = new List<ListLeaveOffInfo>();

            if (infoDtos.fullName == null && infoDtos.status == null && infoDtos.date == null)
            {
                success = false;
                message = "Nghỉ phép không thành công! Thông số đầu vào không được cung cấp.";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }

            try
            {
                var query = _appContext.leaveOffs.Where(s => s.status != StatusLO.Waiting).AsQueryable();

                if (String.IsNullOrEmpty(infoDtos.fullName) == false)
                {
                    var checkName = _appContext.Users
                        .Where(q => (q.firstName + " " + q.lastName).ToLower().Trim().Contains(infoDtos.fullName.Trim().ToLower()))
                        .ToList();
                    var idList = checkName.Select(item => item.id).ToList();
                    query = query.Where(x => idList.Contains(x.idLeaveUser));
                }

                if (infoDtos.status.Count > 0)
                {
                    var statusList = infoDtos.status.ToList();
                    query = query.Where(x => statusList.Contains((int)x.status));
                }

                if (infoDtos.date != null)
                {
                    query = query.Where(t => t.startTime.Year <= infoDtos.date.Value.Year && t.endTime.Year >= infoDtos.date.Value.Year);
                }

                if (infoDtos.dateMonth != null)
                {
                    query = query.Where(s =>
                    (s.startTime.Month <= infoDtos.dateMonth.Value.Month && s.startTime.Year == infoDtos.dateMonth.Value.Year)
                    && (s.endTime.Month >= infoDtos.dateMonth.Value.Month && s.endTime.Year == infoDtos.dateMonth.Value.Year));
                }

                data = await query
                .Join(_appContext.Users, x => x.idLeaveUser, c => c.id, (x, c) => new { Leave = x, LeaveUser = c })
                .Join(_appContext.Users, x => x.Leave.idAcceptUser, d => d.id, (x, d) => new { x.Leave, x.LeaveUser, AcceptUser = d })
                .OrderBy(x => x.Leave.acceptTime)
                .Select(x => new ListLeaveOffInfo
                {
                    id = x.Leave.id,
                    idLeaveUser = x.Leave.idLeaveUser,
                    idAcceptUser = x.Leave.idAcceptUser,
                    startTime = x.Leave.startTime,
                    endTime = x.Leave.endTime,
                    reasons = x.Leave.reasons,
                    status = x.Leave.status,
                    createTime = x.Leave.createTime,
                    acceptTime = x.Leave.acceptTime,
                    ReasonNotAccept = x.Leave.ReasonNotAccept,
                    idCompanyBranh = x.Leave.idCompanyBranh,
                    ReasonAccept = x.Leave.ReasonAccept,
                    name = x.LeaveUser.FullName,
                    acceptBy = x.AcceptUser.FullName,
                    userCode = x.LeaveUser.userCode,
                    flagOnDay = x.Leave.flagOnDay
                }).ToListAsync();

                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Không thể nghỉ phép! Đã xảy ra lỗi: " + ex.Message;
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<ListLeaveOffInfo>>> getAllLeaveOffInfo()
        {
            var success = false;
            var message = "";
            var data = new List<ListLeaveOffInfo>();
            try
            {
                var list = _appContext.leaveOffs
                  .Join(_appContext.Users, x => x.idLeaveUser, c => c.id, (x, c) => new { Leave = x, LeaveUser = c })
                  .Join(_appContext.Users, x => x.Leave.idAcceptUser, d => d.id, (x, d) => new { x.Leave, x.LeaveUser, AcceptUser = d })
                  .Where(x => x.Leave.status != StatusLO.Waiting)
                  .OrderBy(x => x.Leave.status)
                  .Select(x => new ListLeaveOffInfo
                  {
                      id = x.Leave.id,
                      idLeaveUser = x.Leave.idLeaveUser,
                      idAcceptUser = x.Leave.idAcceptUser,
                      startTime = x.Leave.startTime,
                      endTime = x.Leave.endTime,
                      reasons = x.Leave.reasons,
                      status = x.Leave.status,
                      createTime = x.Leave.createTime,
                      acceptTime = x.Leave.acceptTime,
                      ReasonNotAccept = x.Leave.ReasonNotAccept,
                      idCompanyBranh = x.Leave.idCompanyBranh,
                      ReasonAccept = x.Leave.ReasonAccept,
                      name = x.LeaveUser.FullName,
                      acceptBy = x.AcceptUser.FullName,
                      userCode = x.LeaveUser.userCode,
                      flagOnDay = x.Leave.flagOnDay
                  }).ToList();
                data = list;
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
            catch (Exception)
            {
                success = false;
                message = "Lấy tất cả dữ liệu không thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllAsyncByLead(int id)
        {
            var success = false;
            var message = "";
            var data = new List<ListLeaveOffInfo>();
            try
            {
                var getProjectPMCreated = await _appContext.Projects.Where(p => p.Leader.Equals(id) && p.IsDeleted == false).Select(p => p.UserCreated).Distinct().ToListAsync();
                var list = (from x in _appContext.leaveOffs
                            join e in _appContext.Users on x.idLeaveUser equals e.id
                            join d in _appContext.Member_Projects on e.id equals d.member
                            join c in _appContext.Projects on d.idProject equals c.Id
                            where (c.Leader == id || x.idAcceptUser == id) && x.idLeaveUser != id && (!getProjectPMCreated.Contains(d.member))
                            select new ListLeaveOffInfo
                            {
                                id = x.id,
                                idLeaveUser = e.id,
                                idAcceptUser = x.idAcceptUser,
                                startTime = x.startTime,
                                endTime = x.endTime,
                                reasons = x.reasons,
                                status = x.status,
                                createTime = x.createTime,
                                acceptTime = x.acceptTime,
                                ReasonNotAccept = x.ReasonNotAccept,
                                idCompanyBranh = x.idCompanyBranh,
                                ReasonAccept = x.ReasonAccept,
                                name = e.FullName,
                                userCode = e.userCode,
                                flagOnDay = x.flagOnDay,
                            }).OrderBy(x => x.createTime).Distinct().ToListAsync();

                var now = DateTime.Now;
                var update = (await list).AsEnumerable()
                    .Where(s => s.status == StatusLO.Waiting).Select(s => s.id).ToList();

                var findLeavOff = await _appContext.leaveOffs.Where(s => update.Contains(s.id)).ToListAsync();

                var map = findLeavOff.Where(s => s.status == StatusLO.Waiting && s.endTime.Date < now.Date
                                || (s.endTime.Date == now.Date
                                && s.endTime.Hour < now.Hour)
                                || (s.endTime.Date == now.Date
                                && s.endTime.Hour == now.Hour
                                && s.endTime.Minute < now.Minute)).ToList();

                map.ForEach(res =>
                {
                    res.ReasonAccept = "Quá hạn duyệt";
                    res.status = StatusLO.Cancel;
                });

                await _appContext.SaveChangesAsync();

                data = await list;
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllAsyncByPm(int id)
        {
            var success = false;
            var message = "";
            var data = new List<ListLeaveOffInfo>();
            try
            {
                var list = await (from x in _appContext.leaveOffs
                            join c in _appContext.Projects on x.idLeaveUser equals c.Leader
                            join e in _appContext.Users on c.Leader equals e.id
                            where c.UserCreated == id || x.idAcceptUser == id
                            select new ListLeaveOffInfo
                            {
                                id = x.id,
                                idLeaveUser = e.id,
                                idAcceptUser = x.idAcceptUser,
                                startTime = x.startTime,
                                endTime = x.endTime,
                                reasons = x.reasons,
                                status = x.status,
                                createTime = x.createTime,
                                acceptTime = x.acceptTime,
                                ReasonNotAccept = x.ReasonNotAccept,
                                idCompanyBranh = x.idCompanyBranh,
                                ReasonAccept = x.ReasonAccept,
                                name = e.FullName,
                                userCode = e.userCode,
                                flagOnDay = x.flagOnDay,
                            }).Distinct().ToListAsync();

                var listLeaveOffUserCurrent = await (from x in _appContext.leaveOffs
                            join e in _appContext.Users on x.idLeaveUser equals e.id
                            where x.idLeaveUser == id || x.idAcceptUser == id
                            select new ListLeaveOffInfo
                            {
                                id = x.id,
                                idLeaveUser = e.id,
                                idAcceptUser = x.idAcceptUser,
                                startTime = x.startTime,
                                endTime = x.endTime,
                                reasons = x.reasons,
                                status = x.status,
                                createTime = x.createTime,
                                acceptTime = x.acceptTime,
                                ReasonNotAccept = x.ReasonNotAccept,
                                idCompanyBranh = x.idCompanyBranh,
                                ReasonAccept = x.ReasonAccept,
                                name = e.FullName,
                                userCode = e.userCode,
                                flagOnDay = x.flagOnDay,
                            }).ToListAsync();

                list.AddRange(listLeaveOffUserCurrent);
                var now = DateTime.Now;
                var update = list.AsEnumerable()
                    .Where(s => s.status == StatusLO.Waiting).Select(s => s.id).ToList();

                var findLeavOff = await _appContext.leaveOffs.Where(s => update.Contains(s.id)).ToListAsync();

                var map = findLeavOff.Where(s => s.status == StatusLO.Waiting && s.endTime.Date < now.Date
                                || (s.endTime.Date == now.Date
                                && s.endTime.Hour < now.Hour)
                                || (s.endTime.Date == now.Date
                                && s.endTime.Hour == now.Hour
                                && s.endTime.Minute < now.Minute)).ToList();

                map.ForEach(res =>
                {
                    res.ReasonAccept = "Quá hạn duyệt";
                    res.status = StatusLO.Cancel;
                });

                await _appContext.SaveChangesAsync();

                data.AddRange(list.OrderBy(x => x.createTime));
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllLeaveOffByNameStatusDateLead(int id, FindByNameStatusDateDtos infoDtos)
        {
            var success = false;
            var message = "";
            var data = new List<ListLeaveOffInfo>();

            if (infoDtos.fullName == null && infoDtos.status == null && infoDtos.date == null)
            {
                success = false;
                message = "Nghỉ phép không thành công! Thông số đầu vào không được cung cấp.";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }

            try
            {
                var getProjectPMCreated = await _appContext.Projects.Where(p => p.Leader.Equals(id) && p.IsDeleted == false).Select(p => p.UserCreated).Distinct().ToListAsync();
                var query = await (from x in _appContext.leaveOffs
                            join e in _appContext.Users on x.idLeaveUser equals e.id
                            join d in _appContext.Member_Projects on e.id equals d.member
                            join c in _appContext.Projects on d.idProject equals c.Id
                            where (c.Leader == id || x.idAcceptUser == id) && x.idLeaveUser != id && (!getProjectPMCreated.Contains(d.member))
                            select new ListLeaveOffInfo
                            {
                                id = x.id,
                                idLeaveUser = e.id,
                                idAcceptUser = x.idAcceptUser,
                                startTime = x.startTime,
                                endTime = x.endTime,
                                reasons = x.reasons,
                                status = x.status,
                                createTime = x.createTime,
                                acceptTime = x.acceptTime,
                                ReasonNotAccept = x.ReasonNotAccept,
                                idCompanyBranh = x.idCompanyBranh,
                                ReasonAccept = x.ReasonAccept,
                                name = e.FullName,
                                userCode = e.userCode,
                                flagOnDay = x.flagOnDay,
                            }).OrderBy(x => x.createTime).Distinct().ToListAsync();
                
                if (String.IsNullOrEmpty(infoDtos.fullName) == false)
                {
                    var checkName = _appContext.Users
                        .Where(q => (q.firstName + " " + q.lastName).ToLower().Trim().Contains(infoDtos.fullName.Trim().ToLower()))
                        .ToList();
                    var idList = checkName.Select(item => item.id).ToList();
                    query = query.Where(x => idList.Contains(x.idLeaveUser)).ToList();
                }

                if (infoDtos.status.Count > 0)
                {
                    var statusList = infoDtos.status.ToList();
                    query = query.Where(x => statusList.Contains((int)x.status)).ToList();
                }

                if (infoDtos.date != null)
                {
                    query = query.Where(x => (x.startTime.Month <= infoDtos.date.Value.Month && x.startTime.Year == infoDtos.date.Value.Year)
                        && (x.endTime.Month >= infoDtos.date.Value.Month && x.endTime.Year == infoDtos.date.Value.Year)).ToList();
                }

                data.AddRange(query);
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Nghỉ phép không thành công! Đã xảy ra lỗi: " + ex.Message;
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
        }
        public async Task<BaseResponse<List<ListLeaveOffInfo>>> GetAllLeaveOffByNameStatusDatePM(int id, FindByNameStatusDateDtos infoDtos)
        {
            var success = false;
            var message = "";
            var data = new List<ListLeaveOffInfo>();

            if (infoDtos.fullName == null && infoDtos.status == null && infoDtos.date == null)
            {
                success = false;
                message = "Nghỉ phép không thành công! Thông số đầu vào không được cung cấp.";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }

            try
            {
                var query = await  (from x in _appContext.leaveOffs
                             join c in _appContext.Projects on x.idLeaveUser equals c.Leader
                             join e in _appContext.Users on c.Leader equals e.id
                             where c.UserCreated == id || x.idAcceptUser == id
                             select new ListLeaveOffInfo
                             {
                                 id = x.id,
                                 idLeaveUser = e.id,
                                 idAcceptUser = x.idAcceptUser,
                                 startTime = x.startTime,
                                 endTime = x.endTime,
                                 reasons = x.reasons,
                                 status = x.status,
                                 createTime = x.createTime,
                                 acceptTime = x.acceptTime,
                                 ReasonNotAccept = x.ReasonNotAccept,
                                 idCompanyBranh = x.idCompanyBranh,
                                 ReasonAccept = x.ReasonAccept,
                                 name = e.FullName,
                                 userCode = e.userCode,
                                 flagOnDay = x.flagOnDay,
                             }).OrderBy(x => x.createTime).Distinct().ToListAsync();

                var listLeaveOffUserCurrent = await (from x in _appContext.leaveOffs
                                                     join e in _appContext.Users on x.idLeaveUser equals e.id
                                                     where x.idLeaveUser == id || x.idAcceptUser == id
                                                     select new ListLeaveOffInfo
                                                     {
                                                         id = x.id,
                                                         idLeaveUser = e.id,
                                                         idAcceptUser = x.idAcceptUser,
                                                         startTime = x.startTime,
                                                         endTime = x.endTime,
                                                         reasons = x.reasons,
                                                         status = x.status,
                                                         createTime = x.createTime,
                                                         acceptTime = x.acceptTime,
                                                         ReasonNotAccept = x.ReasonNotAccept,
                                                         idCompanyBranh = x.idCompanyBranh,
                                                         ReasonAccept = x.ReasonAccept,
                                                         name = e.FullName,
                                                         userCode = e.userCode,
                                                         flagOnDay = x.flagOnDay,
                                                     }).ToListAsync();
                query.AddRange(listLeaveOffUserCurrent);
                if (String.IsNullOrEmpty(infoDtos.fullName) == false)
                {
                    var checkName = _appContext.Users
                        .Where(q => (q.firstName + " " + q.lastName).ToLower().Trim().Contains(infoDtos.fullName.Trim().ToLower()))
                        .ToList();
                    var idList = checkName.Select(item => item.id).ToList();
                    query = query.Where(x => idList.Contains(x.idLeaveUser)).ToList();
                }

                if (infoDtos.status.Count > 0)
                {
                    var statusList = infoDtos.status.ToList();
                    query = query.Where(x => statusList.Contains((int)x.status)).ToList();
                }

                if (infoDtos.date != null)
                {
                    query = query.Where(x => (x.startTime.Month <= infoDtos.date.Value.Month && x.startTime.Year == infoDtos.date.Value.Year)
                        && (x.endTime.Month >= infoDtos.date.Value.Month && x.endTime.Year == infoDtos.date.Value.Year)).ToList();
                }

                data.AddRange(query);
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Nghỉ phép không thành công! Đã xảy ra lỗi: " + ex.Message;
                return (new BaseResponse<List<ListLeaveOffInfo>>(success, message, data));
            }
        }

        public async Task<BaseResponse<UserInfoLeaveOffDtos>> GetInfoUserLeaveOff(int idLeaveOffUser, DateTime date)
        {
            var success = false;
            var message = "";
            var data = new UserInfoLeaveOffDtos();
            try
            {
                var map = await _appContext.leaveOffs.Where(s => s.idLeaveUser == idLeaveOffUser && s.status != StatusLO.Waiting && s.createTime.Year == date.Year)
                                                           .OrderByDescending(s => s.createTime)
                                                           .ToListAsync();
                data.CurrentUser = idLeaveOffUser;
                data.AllLeaveOffNavigations = map;
                data.LeaveOffNavigations = map.Where(s => s.status == StatusLO.Done).ToList();
                data.NumberOfLeaveAccept = map.Where(s => s.status == StatusLO.Done).ToList().Count();
                data.NumberOfLeaveNoAccept = map.Where(s => s.status == StatusLO.Cancel).ToList().Count();

                success = true;
                message = $"Lấy thông tin của nhân viên nghỉ phép thành công ";
                return (new BaseResponse<UserInfoLeaveOffDtos>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Lấy thông tin của nhân viên nghỉ phép không thành công {ex.Message}";
                return (new BaseResponse<UserInfoLeaveOffDtos>(success, message, data));
            }

        }
    }
}
