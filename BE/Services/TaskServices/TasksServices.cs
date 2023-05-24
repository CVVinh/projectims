using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.TaskDto;
using BE.Data.Dtos.TaskDtos;
using BE.Data.Enum;
using BE.Data.Enums.TaskEnums;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using BE.Services.MemberProjectServices;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.TaskServices
{
    public interface ITasksServices
    {
        Task<bool> deleteTaskByIdAsync(int Id, DeleteTaskDto deleteTask);
        Task<BaseResponse<Tasks>> getTaskByIdAsync(int Id);
        Task<BaseResponse<Tasks>> addNewTaskAsync(AddNewTaskDto addNewTaskDto);
        Task<BaseResponse<ICollection<Tasks>>> GetTasksByIdUserAtProjectAsync(int idUser, int idProject);
        Task<BaseResponse<List<Tasks>>> MutilateAddTask(int ipProject, List<AddNewTaskDto> addNewTaskDto);
        Task<BaseResponse<Tasks>> UpdateTaskAsync(int idTask, UpdateTaskDto addNewTaskDto);
        Task<BaseResponse<List<GetAllTaskDto>>> SearchTasksByName(int idProject, string? name);
        Task<BaseResponse<List<GetAllTaskDto>>> SearchTasksByNameStaff(int idStaff, int idProject, string? name);

    }

    public class TasksServices : ITasksServices
    {
        private readonly AppDbContext _appContext;
        private readonly IMemberProjectServices _memberProjectServices;
        private readonly IMapper _mapper;
        public TasksServices(AppDbContext appContext, IMemberProjectServices memberProjectServices, IMapper mapper)
        {
            _appContext = appContext;
            _memberProjectServices = memberProjectServices;
            _mapper = mapper;
        }
        public async Task<BaseResponse<List<Tasks>>> MutilateAddTask(int ipProject, List<AddNewTaskDto> addNewTaskDto)
        {
            var success = false;
            var message = "";
            List<Tasks> resultMap = new List<Tasks>();
            try
            {
                var projectDb = await _appContext.Projects.FindAsync(ipProject);
                if (projectDb == null)
                {
                    success = false;
                    message = "Không thể tìm thấy dự án";
                    return new BaseResponse<List<Tasks>>(success, message, resultMap);
                }
                if (addNewTaskDto.Count() > 0)
                {
                    foreach (var item in addNewTaskDto)
                    {
                        var tasksToAdd = _mapper.Map<Tasks>(item);

                        if (item.isOnGitLab)
                        {
                            tasksToAdd.status = null;
                        }
                        else
                        {
                            tasksToAdd.status = Status.Open;
                        }
                        resultMap.Add(tasksToAdd);
                    }
                    await _appContext.tasks.AddRangeAsync(resultMap);
                }
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Thêm nhiều công việc mới thành công";
                return new BaseResponse<List<Tasks>>(success, message, resultMap);
            }
            catch
            {
                message = "Thêm nhiều công việc mới không thành công!";
                success = false;
                return new BaseResponse<List<Tasks>>(success, message, resultMap);
            }
        }

        public async Task<BaseResponse<Tasks>> addNewTaskAsync(AddNewTaskDto addNewTaskDto)
        {
            var success = false;
            var message = "";
            Tasks resultMap = new Tasks();
            try
            {
                var projectDb = await _appContext.Projects.FindAsync(addNewTaskDto.idProject);
                if (projectDb == null)
                {
                    success = false;
                    message = "Không thể tìm thấy dự án";
                    return new BaseResponse<Tasks>(success, message, resultMap);
                }

                resultMap = _mapper.Map<Tasks>(addNewTaskDto);
                resultMap.taskCode = CommonHelper.RandomCode();
                resultMap.status = addNewTaskDto.isOnGitLab ? null : addNewTaskDto.status;

                await _appContext.tasks.AddAsync(resultMap);
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Thêm nhiệm vụ mới thành công";
                return new BaseResponse<Tasks>(success, message, resultMap);
            }
            catch
            {
                message = "Thêm nhiệm vụ mới không thành công!";
                success = false;
                return new BaseResponse<Tasks>(success, message, resultMap);
            }
        }

        public async Task<BaseResponse<Tasks>> UpdateTaskAsync(int idTask, UpdateTaskDto updateTaskDto)
        {
            var success = false;
            var message = "";
            Tasks resultMap = new Tasks();
            try
            {
                var taskDb = await _appContext.tasks.Where(s => s.iid_issue == idTask && s.status != Status.Deleted && s.isDeleted == false).FirstOrDefaultAsync();
                if (taskDb == null)
                {
                    success = false;
                    message = "Không thể tìm thấy Công việc !";
                    return new BaseResponse<Tasks>(success, message, resultMap);
                }
                resultMap = _mapper.Map<UpdateTaskDto, Tasks>(updateTaskDto, taskDb);
                if (updateTaskDto.time_estimate != null)
                {
                    resultMap.estimate_DateCreated = DateTime.Now;
                }
                _appContext.tasks.Update(resultMap);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Cập nhật công việc thành công";
                return new BaseResponse<Tasks>(success, message, resultMap);
            }
            catch
            {
                message = "Cập nhật công việc không công!";
                success = false;
                return new BaseResponse<Tasks>(success, message, resultMap);
            }
        }

        public async Task<bool> deleteTaskByIdAsync(int Id, DeleteTaskDto deleteTask)
        {
            try
            {
                var result = await _appContext.tasks.SingleOrDefaultAsync(h => h.idTask == Id &&
                                                                          h.status != Status.Deleted);
                if (result is null)
                {
                    return false;
                }

                result.status = Status.Deleted;
                result.isDeleted = true;
                result.dateDeleted = DateTime.Now;
                result.userDeleted = deleteTask.userDelete;
                await _appContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<BaseResponse<Tasks>> getTaskByIdAsync(int Id)
        {
            var success = false;
            var message = "";
            var data = new Tasks();
            try
            {
                var checkIdTask = await _appContext.tasks.Where(t => t.idTask == Id && t.status != Status.Deleted).FirstOrDefaultAsync();
                if (checkIdTask is null)
                {
                    message = "Id task does not exist";
                    return new BaseResponse<Tasks>(success, message, data);
                }

                success = true;
                message = "Getting task by id task sucessfully";
                data = checkIdTask;
                return new BaseResponse<Tasks>(success, message, data);
            }
            catch
            {
                message = "Don't connect database !";
                data = null;
                return new BaseResponse<Tasks>(success, message, data);
            }
        }

        public async Task<BaseResponse<ICollection<Tasks>>> GetTasksByIdUserAtProjectAsync(int idUser, int idProject)
        {
            var success = false;
            var message = "";
            var data = new List<Tasks>();
            try
            {
                // test idUser existed in project
                var memberProject = await _memberProjectServices.GetMemberByIdUserAtProjectAsync(idUser, idProject);
                if (!memberProject._success)
                {
                    message = memberProject._Message;
                    return new BaseResponse<ICollection<Tasks>>(success, message, data);
                }

                // getting tasks of idUser in project
                var tasks = await _appContext.tasks.Where(t => t.assignee == memberProject._Data.Id &&
                                                                    t.status != Status.Deleted && t.isDeleted == false).ToListAsync();
                if (!tasks.Any())
                {
                    message = "This user doesn't have task !";
                    return new BaseResponse<ICollection<Tasks>>(success, message, data);
                }

                data = tasks.ToList();
                success = true;
                message = "Getting tasks of idUser in project successfully";
                return new BaseResponse<ICollection<Tasks>>(success, message, data);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return new BaseResponse<ICollection<Tasks>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<GetAllTaskDto>>> SearchTasksByName(int idProject, string? name)
        {
            var success = false;
            var message = "";
            var data = new List<GetAllTaskDto>();
            try
            {
                var findProject = await _appContext.Projects.Where(s => s.Id.Equals(idProject)).FirstOrDefaultAsync();
                if (findProject == null)
                {
                    success = false;
                    message = "Không tìm thấy dự án nào";
                    return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    var results = await _appContext.tasks
                        .Where(s => s.idProject == findProject.Id && s.isDeleted == false && s.taskName.ToLower().Trim().Contains(name.Trim().ToLower()))
                        .OrderByDescending(s => s.createTaskDate).ToListAsync();
                    if (!results.ToList().Any())
                    {
                        success = false;
                        message = "Dự án không có công việc nào";
                        return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
                    }
                    var resultMap = _mapper.Map<List<GetAllTaskDto>>(results);

                    data = resultMap;
                    success = true;
                    message = "Tìm kiếm thành công !!";
                    return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
                }

                success = false;
                message = "Chưa nhập tìm kiếm";
                return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<GetAllTaskDto>>> SearchTasksByNameStaff(int idStaff, int idProject, string? name)
        {
            var success = false;
            var message = "";
            var data = new List<GetAllTaskDto>();
            try
            {
                var findProject = await _appContext.Projects.Where(s => s.Id.Equals(idProject)).FirstOrDefaultAsync();
                if (findProject == null)
                {
                    success = false;
                    message = "Không tìm thấy dự án nào";
                    return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    var results = await _appContext.tasks
                        .Where(s => s.assignee == idStaff && s.idProject == idProject && s.taskName.ToLower().Trim().Contains(name.Trim().ToLower()))
                        .OrderByDescending(s => s.createTaskDate).ToListAsync();

                    var resultMap = _mapper.Map<List<GetAllTaskDto>>(results);

                    data = resultMap;
                    success = true;
                    message = "Tìm kiếm thành công !!";
                    return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
                }
                success = false;
                message = "Tìm kiếm không thành công !!";
                return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return new BaseResponse<List<GetAllTaskDto>>(success, message, data);
            }
        }
    }
}

