using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Dtos.StaffReviewDtos;
using BE.Data.Dtos.TimeSheetDtos;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;

namespace BE.Services.TimeSheetServices
{
    public interface ITimeSheetDailyServices
    {
        Task<BaseResponse<List<ResponseTimeSheetDailyDto>>> GetAllTimeSheetDaily(int idUser, string idProject);
        Task<BaseResponse<ResponseTimeSheetDailyDto>> GetByIdTimeSheetDaily(int id, string idProject, int idUser);
        Task<BaseResponse<ResponseTimeSheetDailyDto>> GetByIdUserTimeSheetDaily(int idUser, string idProject);
        Task<BaseResponse<TimeSheetDaily>> CreateTimeSheetDaily(CreatedTimeSheetDailyDto createdTimeSheetDailyDto);
        Task<BaseResponse<TimeSheetDaily>> UpdateTimeSheetDaily(int id, CreatedTimeSheetDailyDto updatedTimeSheetDailyDto);
        Task<BaseResponse<TimeSheetDaily>> DeleteTimeSheetDaily(int id, int idUserDeleted);

        Task<BaseResponse<List<TimeSheetDaily>>> CreateMultiTimeSheetDaily(List<CreatedTimeSheetDailyDto> createdTimeSheetDailyDtos);
        Task<BaseResponse<List<TimeSheetDaily>>> UpdateMultiTimeSheetDaily(List<UpdatedMultiTimeSheetDailyDto> updatedTimeSheetDailyDtos);
        Task<BaseResponse<List<TimeSheetDaily>>> DeleteMultiTimeSheetDaily(int idUserDeleted, List<DeleteMultiTimeSheetDailyDto> deleteMultiTimeSheetDailyDtos);
        Task<BaseResponse<List<ResponseTimeSheetDailyDto>>> GetByDateTimeSheetDaily(GetByDateTimeSheetDto getByDateTimeSheetDto);
        Task<BaseResponse<List<TimeSheetDaily>>> DeleteByDateTimeSheetDaily(GetByDateTimeSheetDto getByDateTimeSheetDto);
        Task<BaseResponse<List<ResponseTimeSheetDailyDto>>> SearchByDateOrIdUserTimeSheetDaily(GetByDateTimeSheetDto getByDateTimeSheetDto);
        Task<BaseResponse<ListTimeSheetTotalDto>> ListTimeSheetTotal(int pageSize, int idUser, string idProject);
        Task<BaseResponse<TimeSheetDaily>> checkToDay(string date, int idUser, string idProject);
    }

    public class TimeSheetDailyServices : ITimeSheetDailyServices
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public TimeSheetDailyServices(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        private List<ResponseTimeSheetDailyDto> getData(string idProject, int idUser)
        {
            var groupTimeSheet = _appDbContext.TimeSheetDailys.AsEnumerable().Where(s => s.isDeleted == false && s.idProject.Equals(idProject) && s.idUser.Equals(idUser)).GroupBy(s => s.dateInputTimeSheet).ToList();
            var results = new List<ResponseTimeSheetDailyDto>();
            foreach (var item in groupTimeSheet)
            {
                var resultGroups = item.ToList();
                var totalSum = item.Sum(s => s.sum);
                var resultCalculation = (from tsd in resultGroups
                                         join us in _appDbContext.Users on tsd.idUser equals us.id
                                        where tsd.isDeleted == false && us.isDeleted == 0
                                        orderby tsd.dateInputTimeSheet
                                        select new ResponseTimeSheetDailyDto
                                        {
                                            id = tsd.id,
                                            idUser = tsd.idUser,
                                            idProject = tsd.idProject,
                                            fullName = us.FullName,
                                            dateInputTimeSheet = tsd.dateInputTimeSheet,
                                            date = tsd.dateInputTimeSheet.ToString("dd/MM/yyyy"),
                                            taskContent = tsd.taskContent,
                                            layout = tsd.layout == 0 ? null : tsd.layout,
                                            spec = tsd.spec == 0 ? null : tsd.spec,
                                            api = tsd.api == 0 ? null : tsd.api,
                                            web = tsd.web == 0 ? null : tsd.web,
                                            utc = tsd.utc == 0 ? null : tsd.utc,
                                            ute = tsd.ute == 0 ? null : tsd.ute,
                                            integration = tsd.integration == 0 ? null : tsd.integration,
                                            deployment = tsd.deployment == 0 ? null : tsd.deployment,
                                            fixbug = tsd.fixbug == 0 ? null : tsd.fixbug,
                                            support = tsd.support == 0 ? null : tsd.support,
                                            others = tsd.others == 0 ? null : tsd.others,
                                            sum = totalSum,
                                            isConfirm = tsd.isConfirm,
                                            isAction = DateTime.Now.ToString("dd/MM/yyyy") == tsd.dateInputTimeSheet.ToString("dd/MM/yyyy")? true: false,
                                        }).ToList();
                results.AddRange(resultCalculation);
            }
            return results.OrderBy(s => s.date).ToList();
        }
        public async Task<BaseResponse<List<ResponseTimeSheetDailyDto>>> GetAllTimeSheetDaily(int idUser, string idProject)
        {
            var success = false;
            var message = "";
            var data = new List<ResponseTimeSheetDailyDto>();
            try
            {
                var timeSheetDailies = getData(idProject, idUser);
                success = true;
                message = "Nhận tất cả dữ liệu thành công!";
                return (new BaseResponse<List<ResponseTimeSheetDailyDto>>(success, message, timeSheetDailies));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<List<ResponseTimeSheetDailyDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<ListTimeSheetTotalDto>> ListTimeSheetTotal(int pageSize, int idUser, string idProject)
        {
            var success = false;
            var message = "";
            var data = new ListTimeSheetTotalDto();
            try
            {
                var groupTimeSheet = await _appDbContext.TimeSheetDailys.Where(s => s.isDeleted == false && s.idUser.Equals(idUser) && s.idProject.Equals(idProject)).Take(pageSize).ToListAsync();
                foreach (var item in groupTimeSheet)
                {
                    data.layoutTotal += item.layout;
                    data.specTotal += item.spec;
                    data.apiTotal += item.api;
                    data.webTotal += item.web;
                    data.utcTotal += item.utc;
                    data.uteTotal += item.ute;
                    data.integrationTotal += item.integration;
                    data.deploymentTotal += item.deployment;
                    data.fixbugTotal += item.fixbug;
                    data.supportTotal += item.support;
                    data.othersTotal += item.others;
                    data.sumTotal += item.sum;
                }
                success = true;
                message = "Tính tổng TimeSheet thành công!";
                return (new BaseResponse<ListTimeSheetTotalDto>(success, message, data));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return (new BaseResponse<ListTimeSheetTotalDto>(success, message, data));
            }
        }

        public async Task<BaseResponse<ResponseTimeSheetDailyDto>> GetByIdTimeSheetDaily(int id, string idProject, int idUser)
        {
            var success = false;
            var message = "";
            var data = new ResponseTimeSheetDailyDto();
            try
            {
                var timeSheetDailies = getData(idProject, idUser);
                var timeSheetDaily = timeSheetDailies.Where(s => s.id.Equals(id)).FirstOrDefault();
                if(timeSheetDaily == null)
                {
                    message = "Timesheet không tồn tại!";
                    return (new BaseResponse<ResponseTimeSheetDailyDto>(success, message, data));
                }
                success = true;
                message = "Tìm kiếm timesheet với id thành công!";
                return (new BaseResponse<ResponseTimeSheetDailyDto>(success, message, timeSheetDaily));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<ResponseTimeSheetDailyDto>(success, message, data));
            }
        }

        public async Task<BaseResponse<ResponseTimeSheetDailyDto>> GetByIdUserTimeSheetDaily(int idUser, string idProject)
        {
            var success = false;
            var message = "";
            var data = new ResponseTimeSheetDailyDto();
            try
            {
                var timeSheetDailies = getData(idProject, idUser);
                var timeSheetDaily = timeSheetDailies.Where(s => s.idUser.Equals(idUser)).FirstOrDefault();
                if (timeSheetDaily == null)
                {
                    message = "Timesheet không tồn tại!";
                    return (new BaseResponse<ResponseTimeSheetDailyDto>(success, message, data));
                }
                success = true;
                message = "Tìm kiếm timesheet với idUser thành công!";
                return (new BaseResponse<ResponseTimeSheetDailyDto>(success, message, timeSheetDaily));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<ResponseTimeSheetDailyDto>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<ResponseTimeSheetDailyDto>>> GetByDateTimeSheetDaily(GetByDateTimeSheetDto getByDateTimeSheetDto)
        {
            var success = false;
            var message = "";
            var data = new List<ResponseTimeSheetDailyDto>();
            try
            {
                var timeSheetDailies = getData(getByDateTimeSheetDto.idProject, (int)getByDateTimeSheetDto.idUser);
                var timeSheetDaily = timeSheetDailies.Where(s => s.idUser.Equals(getByDateTimeSheetDto.idUser) && s.date.Equals(getByDateTimeSheetDto.date)).ToList();
                
                success = true;
                message = "Tìm kiếm timesheet với thời gian làm việc thành công!";
                return (new BaseResponse<List<ResponseTimeSheetDailyDto>>(success, message, timeSheetDaily));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<ResponseTimeSheetDailyDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<TimeSheetDaily>> checkToDay(string date, int idUser, string idProject)
        {
            var success = false;
            var message = "";
            var data = new TimeSheetDaily();
            try
            {
                var timeSheetDaily = _appDbContext.TimeSheetDailys.AsEnumerable().Where(s => s.isDeleted == false && s.idUser.Equals(idUser) && s.idProject.Equals(idProject) && s.dateInputTimeSheet.ToString("dd/MM/yyyy").Equals(date)).FirstOrDefault();
                if(timeSheetDaily == null)
                {
                    success = false;
                    message = "Tìm kiếm timesheet với thời gian làm việc KHÔNG thành công!";
                    return (new BaseResponse<TimeSheetDaily>(success, message, data));
                }
                success = true;
                message = "Tìm kiếm timesheet với thời gian làm việc thành công!";
                return (new BaseResponse<TimeSheetDaily>(success, message, timeSheetDaily));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<TimeSheetDaily>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<ResponseTimeSheetDailyDto>>> SearchByDateOrIdUserTimeSheetDaily(GetByDateTimeSheetDto getByDateTimeSheetDto)
        {
            var success = false;
            var message = "";
            var data = new List<ResponseTimeSheetDailyDto>();
            try
            {
                var timeSheetDailies = new List<ResponseTimeSheetDailyDto>();
                if (getByDateTimeSheetDto.idUser != null)
                {
                    timeSheetDailies = getData(getByDateTimeSheetDto.idProject, (int)getByDateTimeSheetDto.idUser);
                    data = timeSheetDailies.Where(s => s.idUser.Equals(getByDateTimeSheetDto.idUser) && s.dateInputTimeSheet.ToString("MM/yyyy").Equals(getByDateTimeSheetDto.date)).ToList();
                }
                else if (getByDateTimeSheetDto.idUserCurrent != null)
                {
                    timeSheetDailies = getData(getByDateTimeSheetDto.idProject, (int)getByDateTimeSheetDto.idUserCurrent);
                    data = timeSheetDailies.Where(s => s.idUser.Equals(getByDateTimeSheetDto.idUserCurrent) && s.dateInputTimeSheet.ToString("MM/yyyy").Equals(getByDateTimeSheetDto.date)).ToList();
                }
                success = true;
                message = "Tìm kiếm timesheet với thời gian làm việc thành công!";
                return (new BaseResponse<List<ResponseTimeSheetDailyDto>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<ResponseTimeSheetDailyDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<TimeSheetDaily>> CreateTimeSheetDaily(CreatedTimeSheetDailyDto createdTimeSheetDailyDto)
        {
            var success = false;
            var message = "";
            try
            {
                var timeSheetDaily = _mapper.Map<TimeSheetDaily>(createdTimeSheetDailyDto);
                timeSheetDaily.dateCreated = DateTime.Now;
                await _appDbContext.TimeSheetDailys.AddAsync(timeSheetDaily);
                await _appDbContext.SaveChangesAsync();

                success = true;
                message = "Thêm việc làm thành công!";
                return new BaseResponse<TimeSheetDaily>(success, message, timeSheetDaily);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Thêm việc làm KHÔNG thành công! {ex.Message}";
                return new BaseResponse<TimeSheetDaily>(success, message, new TimeSheetDaily());
            }
        }

        public async Task<BaseResponse<List<TimeSheetDaily>>> CreateMultiTimeSheetDaily(List<CreatedTimeSheetDailyDto> createdTimeSheetDailyDtos)
        {
            var success = false;
            var message = "";
            var data = new List<TimeSheetDaily>();
            try
            {
                foreach(var item in createdTimeSheetDailyDtos)
                {
                    var timeSheetDaily = _mapper.Map<TimeSheetDaily>(item);
                    timeSheetDaily.dateCreated = DateTime.Now;
                    timeSheetDaily.dateInputTimeSheet = DateTime.ParseExact(item.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    await _appDbContext.TimeSheetDailys.AddAsync(timeSheetDaily);

                    data.Add(timeSheetDaily);
                }
                await _appDbContext.SaveChangesAsync();
                success = true;
                message = "Thêm danh sách việc làm thành công!";
                return new BaseResponse<List<TimeSheetDaily>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Thêm danh sách việc làm KHÔNG thành công! {ex.Message}";
                return new BaseResponse<List<TimeSheetDaily>>(success, message, data);
            }
        }

        public async Task<BaseResponse<TimeSheetDaily>> UpdateTimeSheetDaily(int id, CreatedTimeSheetDailyDto updatedTimeSheetDailyDto)
        {
            var success = false;
            var message = "";
            var data = new TimeSheetDaily();
            try
            {
                var timeSheetDaily = await _appDbContext.TimeSheetDailys.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (timeSheetDaily is null)
                {
                    message = "Timesheet không tồn tại!";
                    return new BaseResponse<TimeSheetDaily>(success, message, data);
                }
                var timeSheetDailyMapData = _mapper.Map<CreatedTimeSheetDailyDto, TimeSheetDaily>(updatedTimeSheetDailyDto, timeSheetDaily);
                timeSheetDaily.dateUpdated = DateTime.Now;
                timeSheetDaily.userUpdated = updatedTimeSheetDailyDto.userCreated;
                _appDbContext.TimeSheetDailys.Update(timeSheetDailyMapData);
                await _appDbContext.SaveChangesAsync();

                success = true;
                message = "Cập nhật việc làm thành công!";
                return new BaseResponse<TimeSheetDaily>(success, message, timeSheetDailyMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Cập nhật việc làm KHÔNG thành công! {ex.Message}";
                return new BaseResponse<TimeSheetDaily>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<TimeSheetDaily>>> UpdateMultiTimeSheetDaily(List<UpdatedMultiTimeSheetDailyDto> updatedTimeSheetDailyDtos)
        {
            var success = false;
            var message = "";
            var data = new List<TimeSheetDaily>();
            try
            {
                foreach (var item in updatedTimeSheetDailyDtos)
                {
                    var timeSheetDaily = await _appDbContext.TimeSheetDailys.Where(s => s.isDeleted == false && s.id.Equals(item.id)).FirstOrDefaultAsync();

                    if (timeSheetDaily != null)
                    {
                        var timeSheetDailyMapData = _mapper.Map<UpdatedMultiTimeSheetDailyDto, TimeSheetDaily>(item, timeSheetDaily);
                        timeSheetDaily.dateUpdated = DateTime.Now;
                        timeSheetDaily.dateInputTimeSheet = DateTime.ParseExact(item.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        _appDbContext.TimeSheetDailys.Update(timeSheetDailyMapData);

                        data.Add(timeSheetDaily);
                    }
                }
                await _appDbContext.SaveChangesAsync();

                var getIdUpdate = data.Select(x => x.id).ToList();
                var idUserDeleted = data[0].userUpdated;
                var dateInputTimeSheet = data[0].dateInputTimeSheet.ToString("dd/MM/yyyy");
                var getOriginData = new GetByDateTimeSheetDto
                {
                    idUser = idUserDeleted,
                    date = dateInputTimeSheet,
                    idProject = data[0].idProject,
                };
                var originData = await GetByDateTimeSheetDaily(getOriginData);
                if(originData._success)
                {
                    var rowDeletes = originData._Data.Where(s => !getIdUpdate.Contains(s.id)).Select(x => new DeleteMultiTimeSheetDailyDto
                    {
                        id = x.id,
                    }).ToList();
                    await DeleteMultiTimeSheetDaily((int)idUserDeleted, rowDeletes);
                }

                success = true;
                message = "Cập nhật danh sách việc làm thành công!";
                return new BaseResponse<List<TimeSheetDaily>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Cập nhật danh sách việc làm KHÔNG thành công! {ex.Message}";
                return new BaseResponse<List<TimeSheetDaily>>(success, message, data);
            }
        }

        public async Task<BaseResponse<TimeSheetDaily>> DeleteTimeSheetDaily(int id, int idUserDeleted)
        {
            var success = false;
            var message = "";
            var data = new TimeSheetDaily();
            try
            {
                var timeSheetDaily = await _appDbContext.TimeSheetDailys.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (timeSheetDaily is null)
                {
                    message = "Timesheet không tồn tại!";
                    return new BaseResponse<TimeSheetDaily>(success, message, data);
                }

                timeSheetDaily.isDeleted = true;
                timeSheetDaily.dateDeleted = DateTime.Now;
                timeSheetDaily.userDeleted = idUserDeleted;
                _appDbContext.TimeSheetDailys.Update(timeSheetDaily);
                await _appDbContext.SaveChangesAsync();

                success = true;
                message = "Xoá việc làm thành công!";
                return new BaseResponse<TimeSheetDaily>(success, message, timeSheetDaily);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xoá việc làm KHÔNG thành công! {ex.InnerException}";
                return new BaseResponse<TimeSheetDaily>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<TimeSheetDaily>>> DeleteByDateTimeSheetDaily(GetByDateTimeSheetDto getByDateTimeSheetDto)
        {
            var success = false;
            var message = "";
            var data = new List<TimeSheetDaily>();
            try
            {
                var timeSheetDailies = _appDbContext.TimeSheetDailys.AsEnumerable().Where(s => s.isDeleted == false && s.idUser.Equals(getByDateTimeSheetDto.idUser) && s.dateInputTimeSheet.ToString("dd/MM/yyyy").Equals(getByDateTimeSheetDto.date)).ToList();
                if (timeSheetDailies.Count() > 0)
                {
                   foreach(var item in timeSheetDailies)
                   {
                        item.isDeleted = true;
                        item.dateDeleted = DateTime.Now;
                        item.userDeleted = getByDateTimeSheetDto.idUserCurrent;
                        _appDbContext.TimeSheetDailys.Update(item);
                        data.Add(item);
                    }
                }
                await _appDbContext.SaveChangesAsync();

                success = true;
                message = "Xoá việc làm thành công!";
                return new BaseResponse<List<TimeSheetDaily>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xoá việc làm KHÔNG thành công! {ex.InnerException}";
                return new BaseResponse<List<TimeSheetDaily>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<TimeSheetDaily>>> DeleteMultiTimeSheetDaily(int idUserDeleted, List<DeleteMultiTimeSheetDailyDto> deleteMultiTimeSheetDailyDtos)
        {
            var success = false;
            var message = "";
            var data = new List<TimeSheetDaily>();
            try
            {
                foreach (var item in deleteMultiTimeSheetDailyDtos)
                {
                    var timeSheetDaily = await _appDbContext.TimeSheetDailys.Where(s => s.isDeleted == false && s.id.Equals(item.id)).FirstOrDefaultAsync();

                    if (timeSheetDaily != null)
                    {
                        timeSheetDaily.isDeleted = true;
                        timeSheetDaily.dateDeleted = DateTime.Now;
                        timeSheetDaily.userDeleted = idUserDeleted;
                        _appDbContext.TimeSheetDailys.Update(timeSheetDaily);

                        data.Add(timeSheetDaily);
                    }
                }
                await _appDbContext.SaveChangesAsync();
                success = true;
                message = "Xoá danh sách việc làm thành công!";
                return new BaseResponse<List<TimeSheetDaily>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xoá danh sách việc làm KHÔNG thành công! {ex.Message}";
                return new BaseResponse<List<TimeSheetDaily>>(success, message, data);
            }
        }

    }
}
