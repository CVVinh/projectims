using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.TaskDto;
using BE.Data.Dtos.TaskDtos;
using BE.Data.Enum;
using BE.Data.Enums.TaskEnums;
using BE.Data.Models;
using BE.Services.MemberProjectServices;
using BE.Services.PaginationServices;
using BE.Services.TaskServices;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BE.Data.Enum.LeaveOff.Status;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize(Roles = "permission_group: True module: tasks")]
    public class TasksController : Controller
    {
        #region Property
        private readonly AppDbContext _appContext;
        private readonly ILogger<TasksController> _logger;
        private readonly IMapper _mapper;
        private readonly ITasksServices _tasksServices;
        private readonly IMemberProjectServices _memberProjectServices;
        private readonly IPaginationServices<GetAllTaskDto> _paginationServices;
        #endregion

        #region Constructor
        public TasksController(AppDbContext appContext,
            ILogger<TasksController> logger,
            IMapper mapper,
            ITasksServices tasksServices,
            IPaginationServices<GetAllTaskDto> paginationServices,
            IMemberProjectServices memberProjectServices)
        {
            _appContext = appContext;
            _memberProjectServices = memberProjectServices;
            _logger = logger;
            _mapper = mapper;
            _tasksServices = tasksServices;
            _paginationServices = paginationServices;
        }
        #endregion

        //GET: get all tasks
        [HttpGet("getAllTasks")]
        public async Task<IActionResult> getAll(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var results = await _appContext.tasks.Where(s => s.isDeleted == false).ToListAsync();
                var pageSize = (int)pageSizeEnum;
                var sortResults = results.OrderByDescending(t => t.createTaskDate);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //GET: get all tasks by id task
        [HttpGet("getAllTasksById/{id}")]
        public async Task<IActionResult> getAllTasksById(int id)
        {
            try
            {
                var results = await _tasksServices.getTaskByIdAsync(id);

                if (results._success)
                {
                    return Ok(results);
                }

                return BadRequest(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        [HttpGet("getAllTasksByIdStaff/{idstaff}/{idproject}/{pageIndex}/{pageSizeEnum}")]
        public async Task<IActionResult> getAllTasksByIdStaff(int? pageIndex, PageSizeEnum pageSizeEnum, int idstaff, int idproject)
        {
            try
            {
                var results = await _appContext.tasks.Where(s => s.assignee == idstaff && s.idProject == idproject).ToListAsync();
                var pageSize = (int)pageSizeEnum;
                var sortResults = results.OrderByDescending(t => t.dateCreated);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //POST: add new task
        [HttpPost("addNewTask")]
        [Authorize(Roles = "module:tasks action:add")]
        public async Task<IActionResult> addNewTask(AddNewTaskDto addNewTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newTask = await _tasksServices.addNewTaskAsync(addNewTaskDto);
                if (newTask._success)
                {
                    return Ok(newTask);
                }

                return BadRequest(newTask);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //GET: get all tasks by assignee in project
        [HttpGet("getAllTaskByAssigneeInProject/{idAssignee}/{idProject}")]
        public async Task<ActionResult<List<Tasks>>> getAllTaskByidAssigneeInProject(int idAssignee, int idProject, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var results = await _tasksServices.GetTasksByIdUserAtProjectAsync(idAssignee, idProject);
                if (results._success == false)
                {
                    return BadRequest(results);
                }

                var pageSize = (int)pageSizeEnum;
                var sortResults = results._Data.OrderByDescending(t => t.createTaskDate);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //GET: get all tasks by date 
        [HttpGet("getAllTaskByDate/{time}")]
        public async Task<ActionResult<List<Tasks>>> getAllTaskByDate(DateTime time, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var results = await _appContext.tasks.Where(t => t.startTaskDate <= time && t.endTaskDate >= time).ToListAsync();
                if (!results.Any())
                {
                    return NotFound("Don't found task in date!!!");
                }

                var pageSize = (int)pageSizeEnum;
                var sortResults = results.OrderByDescending(t => t.createTaskDate);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //GET: get all tasks that expired
        [HttpGet("getAllTaskExpired")]
        public async Task<ActionResult<List<Tasks>>> getAllTaskExpired(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var results = await _appContext.tasks.Where(t => t.endTaskDate < DateTime.Now).ToListAsync();
                if (!results.Any())
                {
                    return NotFound("Don't found tasks expired!!!");
                }

                var pageSize = (int)pageSizeEnum;
                var sortResults = results.OrderByDescending(t => t.createTaskDate);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //GET: get all tasks that deleted
        [HttpGet("getAllTasksDeleted")]
        public async Task<ActionResult<List<Tasks>>> GetAllTasksDeletedAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var results = await _appContext.tasks.Where(t => t.status == Status.Deleted).ToListAsync();
                if (!results.Any())
                {
                    return NotFound("Don't found tasks deleted!!!");
                }

                var pageSize = (int)pageSizeEnum;
                var sortResults = results.OrderByDescending(t => t.createTaskDate);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //PUT: edit task by id
        [HttpPut("editTaskById/{idTask}")]
        [Authorize(Roles = "module:tasks action:update")]
        public async Task<ActionResult> editTaskById([FromRoute] int idTask, UpdateTaskDto updateTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var update = await _tasksServices.UpdateTaskAsync(idTask, updateTaskDto);
                if (update._success)
                {
                    return Ok(update);
                }
                else
                {
                    return BadRequest(update);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //PUT: Delete task by id
        [HttpPut("deletedTask/{idTask}")]
        [Authorize(Roles = "module:tasks action:delete")]
        public async Task<ActionResult> deletedTaskById([FromRoute] int idTask, DeleteTaskDto userDelete)
        {
            try
            {
                var result = await _appContext.tasks.SingleOrDefaultAsync(h => h.idTask == idTask);
                if (result is null)
                {
                    return BadRequest("task not found!!!");
                }

                var isDeleted = await _tasksServices.deleteTaskByIdAsync(result.idTask, userDelete);
                if (isDeleted == true)
                {
                    return Ok("Deleted task success");
                }

                return BadRequest("Delete Faild");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }


        //GET: Get all tasks by id of project
        [HttpGet("getAllTaskByIdProject/{idProject}")]
        public async Task<ActionResult<List<Tasks>>> getAllTaskByIdProject(int idProject, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var results = await _appContext.tasks.Where(t => t.idProject == idProject && t.isDeleted == false).ToListAsync();
                if (!results.ToList().Any())
                {
                    return NotFound("Không tìm thấy công việc nào trong dự án!!!");
                }

                var pageSize = (int)pageSizeEnum;
                var sortResults = results.OrderByDescending(t => t.createTaskDate);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //GET: Get all tasks by status
        [HttpGet("getTasksByStatus/{status}")]
        public async Task<ActionResult<List<Tasks>>> getTasksByStatus(Status status, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var results = await _appContext.tasks.Where(t => t.status == status).ToListAsync();
                var pageSize = (int)pageSizeEnum;
                var sortResults = results.OrderByDescending(t => t.createTaskDate);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //GET: Get number of days that completed task
        [HttpGet("getDaysCompleted/{id}")]
        public async Task<ActionResult> getDaysCompleted(int id)
        {
            try
            {
                var task = await _appContext.tasks.SingleOrDefaultAsync(h => h.idTask == id);
                if (task is null)
                {
                    return NotFound("Don't found task !");
                }

                // test startDate and endDate      
                if (task.startTaskDate is null || task.endTaskDate is null)
                {
                    return BadRequest("this task hasn't starting date or ending date !");
                }

                System.TimeSpan result = (TimeSpan)(task.endTaskDate - task.startTaskDate);
                int day = result.Days;
                return Ok(day);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        //GET: Get all tasks that stilled deadline
        [HttpGet("getTasksStillDealine")]
        public async Task<ActionResult<List<Tasks>>> getTasksStillDealine(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var results = await _appContext.tasks.Where(h => DateTime.Now <= h.endTaskDate && h.status != Status.Deleted).ToListAsync();
                var pageSize = (int)pageSizeEnum;
                var sortResults = results.OrderByDescending(t => t.createTaskDate);
                var resultMap = _mapper.Map<List<GetAllTaskDto>>(sortResults.ToList());
                var resultPage = await _paginationServices.paginationListTableAsync(resultMap, pageIndex, pageSize);

                if (resultPage._success)
                {
                    return Ok(resultPage);
                }

                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }
        }

        [HttpGet("getTaskById/{id}")]
        public async Task<ActionResult<Tasks>> GetTaskById(int id)
        {
            if (_appContext.tasks == null)
            {
                return NotFound();
            }
            var tasks = await _appContext.tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            return Ok(tasks);
        }

        [HttpPost("mutilateAddTask/{idProject}")]
        [Authorize(Roles = "module:tasks action:add")]
        public async Task<IActionResult> MutilateAddTask([FromRoute] int idProject, List<AddNewTaskDto> addNewTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newMutilateTask = await _tasksServices.MutilateAddTask(idProject, addNewTaskDto);
                if (newMutilateTask._success)
                {
                    return Ok(newMutilateTask);
                }

                return BadRequest(newMutilateTask);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("SearchTasksByName/{idProject}")]
        public async Task<IActionResult> SearchTasksByName(int idProject, string? name, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _tasksServices.SearchTasksByName(idProject, name);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("SearchTasksOfStaff/{idproject}/{idstaff}")]
        public async Task<IActionResult> SearchTasksOfStaff(int idProject, int idstaff, string? name, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _tasksServices.SearchTasksByNameStaff(idstaff, idProject, name);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

    }
}
