using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Dtos.ProjectDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.TokenServices;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;


namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: project")]
    public class ProjectController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IPaginationServices<Projects> _paginationServices;
        private readonly IPaginationServices<ListProjectDtos> _paginationServiceProject;
        private readonly IMapper _mapper;
        public ProjectController(IPaginationServices<ListProjectDtos> paginationServiceProject, AppDbContext context, IPaginationServices<Projects> paginationServices, TokenService tokenService, IMapper mapper)
        {
            _context = context;
            _paginationServices = paginationServices;
            _tokenService = tokenService;
            _mapper = mapper;
            _paginationServiceProject = paginationServiceProject;

        }
        // GET: ProjectController

        // PM, sample
        [HttpGet("getAllProject")]
        public async Task<ActionResult> getAllProject(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var dsProject = (from x in _context.Projects
                                 join k in _context.Users on x.Leader equals k.id
                                 where x.IsDeleted == false
                                 select new ListProjectDtos()
                                 {
                                     id = x.Id,
                                     dateCreated = x.DateCreated,
                                     dateUpdate = x.DateUpdate,
                                     description = x.Description,
                                     endDate = x.EndDate,
                                     isDeleted = x.IsDeleted,
                                     isFinished = x.IsFinished,
                                     isOnGitlab = x.IsOnGitlab,
                                     leader = k.FullName,
                                     idLeader = k.id,
                                     name = x.Name,
                                     projectCode = x.ProjectCode,
                                     subProjectCode = x.SubProjectCode,
                                     projectName = x.ProjectName,
                                     startDate = x.StartDate,
                                     userCreated = x.UserCreated,
                                     userId = x.UserId,
                                     userUpdate = x.UserUpdate,
                                     fullNameUserCreated = _context.Users.Where(s => s.id == x.UserCreated).Select(s => s.FullName).SingleOrDefault(),
                                     fullNameUserUpdate = _context.Users.Where(s => s.id == x.UserUpdate).Select(s => s.FullName).SingleOrDefault(),
                                     fullNameUserId = _context.Users.Where(s => s.id == x.UserId).Select(s => s.FullName).SingleOrDefault(),
                                 }).OrderBy(x => x.isFinished == false && x.isDeleted == false).ToList();
                //dsProject = dsProject.OrderBy(s => s.projectCode).ToList();
                //dsProject = dsProject.GroupBy(x => new { x.projectCode, x.name }).Select(g => g.Count() > 1 ? g.OrderBy(x => x.projectCode).Last() : g.First()).ToList();
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceProject.paginationListTableAsync(dsProject, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            catch (Exception ew)
            {
                return BadRequest(ew);
            }
        }

        [HttpGet("getAllProjectRunning")]
        public ActionResult getAllProjectRunning()
        {
            try
            {
                var dsProject = from x in _context.Projects
                                join k in _context.Users on x.Leader equals k.id
                                where x.IsDeleted == false && x.IsFinished == false
                                select new
                                {
                                    id = x.Id,
                                    dateCreated = x.DateCreated,
                                    dateUpdate = x.DateUpdate,
                                    description = x.Description,
                                    endDate = x.EndDate,
                                    isDeleted = x.IsDeleted,
                                    isFinished = x.IsFinished,
                                    isOnGitlab = x.IsOnGitlab,
                                    leader = k.FullName,
                                    name = x.Name,
                                    projectCode = x.ProjectCode,
                                    subProjectCode = x.SubProjectCode,
                                    projectName = x.ProjectName,
                                    startDate = x.StartDate,
                                    userCreated = x.UserCreated,
                                    userId = x.UserId,
                                    userUpdate = x.UserUpdate
                                };

                return Ok(dsProject);
            }
            catch (Exception ew)
            {
                return BadRequest(ew);
            }
        }

        // Lead
        [HttpGet("getAllProjectByLead/{IdLead}")]
        public async Task<ActionResult> getAllProjectByLead(int IdLead, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var list = (from x in _context.Projects
                            join d in _context.Users on x.Leader equals d.id
                            where d.id == IdLead && x.IsDeleted == false && x.IsFinished == false
                            select new ListProjectDtos()
                            {
                                id = x.Id,
                                dateCreated = x.DateCreated,
                                dateUpdate = x.DateUpdate,
                                description = x.Description,
                                endDate = x.EndDate,
                                isDeleted = x.IsDeleted,
                                isFinished = x.IsFinished,
                                isOnGitlab = x.IsOnGitlab,
                                leader = d.FullName,
                                idLeader = d.id,
                                name = x.Name,
                                projectCode = x.ProjectCode,
                                subProjectCode = x.SubProjectCode,
                                projectName = x.ProjectName,
                                startDate = x.StartDate,
                                userCreated = x.UserCreated,
                                userId = x.UserId,
                                userUpdate = x.UserUpdate,
                                fullNameUserCreated = _context.Users.Where(s => s.id == x.UserCreated).Select(s => s.FullName).SingleOrDefault(),
                                fullNameUserUpdate = _context.Users.Where(s => s.id == x.UserUpdate).Select(s => s.FullName).SingleOrDefault(),
                                fullNameUserId = _context.Users.Where(s => s.id == x.UserId).Select(s => s.FullName).SingleOrDefault(),
                            }).OrderBy(x => x.isFinished == false && x.isDeleted == false).ToList();

                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceProject.paginationListTableAsync(list, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
                //return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // Staff
        [HttpGet("getAllProjectByStaff/{idstaff}")]
        public async Task<ActionResult> getAllProjectByStaff(int idstaff, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var list = (from x in _context.Projects
                            join c in _context.Member_Projects on x.Id equals c.idProject
                            join k in _context.Users on x.Leader equals k.id
                            where c.member == idstaff && x.IsDeleted == false && x.IsFinished == false
                            select new ListProjectDtos()
                            {
                                id = x.Id,
                                dateCreated = x.DateCreated,
                                dateUpdate = x.DateUpdate,
                                description = x.Description,
                                endDate = x.EndDate,
                                isDeleted = x.IsDeleted,
                                isFinished = x.IsFinished,
                                isOnGitlab = x.IsOnGitlab,
                                leader = k.FullName,
                                idLeader = k.id,
                                name = x.Name,
                                projectCode = x.ProjectCode,
                                subProjectCode = x.SubProjectCode,
                                projectName = x.ProjectName,
                                startDate = x.StartDate,
                                userCreated = x.UserCreated,
                                userId = x.UserId,
                                userUpdate = x.UserUpdate,
                                fullNameUserCreated = _context.Users.Where(s => s.id == x.UserCreated).Select(s => s.FullName).SingleOrDefault(),
                                fullNameUserUpdate = _context.Users.Where(s => s.id == x.UserUpdate).Select(s => s.FullName).SingleOrDefault(),
                                fullNameUserId = _context.Users.Where(s => s.id == x.UserId).Select(s => s.FullName).SingleOrDefault(),
                            }).OrderBy(x => x.isFinished == false && x.isDeleted == false).ToList();

                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceProject.paginationListTableAsync(list, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getAllProjectByPM/{idpm}")]
        public async Task<ActionResult> getAllProjectByPM(int idpm, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var list = (from x in _context.Projects
                            join d in _context.Users on x.UserId equals d.id
                            where d.id == idpm && x.IsDeleted == false && x.IsFinished == false
                            select new ListProjectDtos()
                            {
                                id = x.Id,
                                dateCreated = x.DateCreated,
                                dateUpdate = x.DateUpdate,
                                description = x.Description,
                                endDate = x.EndDate,
                                isDeleted = x.IsDeleted,
                                isFinished = x.IsFinished,
                                isOnGitlab = x.IsOnGitlab,
                                leader = d.FullName,
                                idLeader = d.id,
                                name = x.Name,
                                projectCode = x.ProjectCode,
                                subProjectCode = x.SubProjectCode,
                                projectName = x.ProjectName,
                                startDate = x.StartDate,
                                userCreated = x.UserCreated,
                                userId = x.UserId,
                                userUpdate = x.UserUpdate,
                                fullNameUserCreated = _context.Users.Where(s => s.id == x.UserCreated).Select(s => s.FullName).SingleOrDefault(),
                                fullNameUserUpdate = _context.Users.Where(s => s.id == x.UserUpdate).Select(s => s.FullName).SingleOrDefault(),
                                fullNameUserId = _context.Users.Where(s => s.id == x.UserId).Select(s => s.FullName).SingleOrDefault(),
                            }).OrderBy(x => x.isFinished == false && x.isDeleted == false).ToList();

                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceProject.paginationListTableAsync(list, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
                //return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // get lead list in addProject
        [HttpGet("getListLead")]
        public IActionResult getListLead()
        {
            try
            {
                var list = from x in _context.Users
                           join c in _context.UserGroups on x.id equals c.idUser
                           where c.idGroup == 3 && x.isDeleted == 0 && c.isDeleted == false 
                           select new
                           {
                               id = x.id,
                               fullName = x.FullName,
                           };


                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost("addProject")]
        [Authorize(Roles = "module:project action:add")]
        public async Task<IActionResult> Create(AddNewProjectDto project_Model)
        {
            try
            {
                var existingProject = _context.Projects.Where(p => p.SubProjectCode == project_Model.SubProjectCode);
                if (existingProject.Any())
                {
                    return BadRequest("Dự án này đã tồn tại!");
                }
                /*            var p = new Projects
                            {

                                Name = project_Model.Name,
                                ProjectCode = project_Model.ProjectCode,
                                Description = project_Model.Description,
                                StartDate = project_Model.StartDate,
                                EndDate = project_Model.EndDate,
                                IsDeleted = false,
                                IsFinished = false,
                                UserId = project_Model.UserId,
                                Leader = project_Model.Leader,
                                UserCreated = project_Model.UserCreated,
                                DateCreated = DateTime.Now,
                                UserUpdate = project_Model.UserCreated,
                                DateUpdate = DateTime.Now

                            };*/
                var p = _mapper.Map<Projects>(project_Model);
                /*var pro = await _context.Projects.SingleOrDefaultAsync(pr => pr.Name == p.Name);*/
                if (/*pro == null && */p.StartDate < p.EndDate || p.EndDate == null)
                { 
                    if (p.IsOnGitlab)
                    {
                        p.EndDate = null;
                    }
                    _context.Add(p);
                    _context.SaveChanges();
                    return Ok();
                }
                return BadRequest("Wrong data enter");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }


        }
        [HttpPut("DeleteProject/{id}")]
        [Authorize(Roles = "module:project action:delete")]
        public IActionResult DeleteProject(int id, IdUserChangeProjectDto request)
        {
            try
            {
                var pro = _context.Projects.SingleOrDefault(p => p.Id == id);
                if (pro == null)
                {
                    return NotFound("Khong tim thay doi tuong");
                }
                else
                {
                    _context.Projects.Remove(pro);
                    _context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("FinishProject/{id}")]
        [Authorize(Roles = "module:project action:confirm")]
        public IActionResult FinishProject(int id, IdUserChangeProjectDto request)
        {

            try
            {
                var pro = _context.Projects.SingleOrDefault(p => p.Id == id);
                if (pro == null)
                {
                    return NotFound("Khong tim thay doi tuong");
                }
                else
                {
                    if(pro.IsOnGitlab == false)
                    {
                        if (pro.StartDate < pro.EndDate)
                        {
                            pro.IsFinished = true;
                            pro.UserUpdate = request.UserId;
                            pro.DateUpdate = DateTime.UtcNow;
                            _context.SaveChanges();
                            return Ok();
                        }

                        if (pro.StartDate > pro.EndDate && pro.StartDate < DateTime.UtcNow)
                        {
                            pro.IsFinished = true;
                            pro.UserUpdate = request.UserId;
                            pro.DateUpdate = DateTime.UtcNow;
                            pro.EndDate = DateTime.UtcNow;
                            _context.SaveChanges();
                            return Ok();
                        }

                        if (pro.EndDate == null && pro.StartDate < DateTime.UtcNow)
                        {
                            pro.IsFinished = true;
                            pro.UserUpdate = request.UserId;
                            pro.DateUpdate = DateTime.UtcNow;
                            pro.EndDate = DateTime.UtcNow;
                            _context.SaveChanges();
                            return Ok();
                        }

                        if (pro.EndDate == null && pro.StartDate > DateTime.UtcNow)
                        {
                            return BadRequest("Ngày bắt đầu không được nhỏ hơn ngày hiện tại hoặc thêm ngày kết thúc");
                        }


                        if (pro.StartDate > pro.EndDate && pro.StartDate > DateTime.UtcNow)
                        {
                           return BadRequest("Ngày bắt đầu không được nhỏ hơn ngày hiện tại hoặc thêm ngày kết thúc");
                        }

                        return BadRequest("Something went wrong");
                    }
                    else
                    {
                        pro.IsFinished = true;
                        pro.UserUpdate = request.UserId;
                        pro.DateUpdate = DateTime.UtcNow;
                        _context.SaveChanges();
                        return Ok();
                    }
          
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getById/{id}")]
        public IActionResult getById(int id)
        {
            try
            {
                var pro = _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefault(p => p.Id == id);
                if (pro == null)
                {
                    return NotFound();
                }
                return Ok(pro);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("getProByIdDel/{id}")]
        public IActionResult getProByIdDel(int id)
        {
            try
            {
                var pro = _context.Projects.SingleOrDefault(p => p.Id == id);
                if (pro == null)
                {
                    return NotFound();
                }
                return Ok(pro);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("getProjectIsDelete")]
        public IActionResult getListProIsDelete()
        {
            try
            {
                var pro = _context.Projects.Where(p => p.IsDeleted == true);
                if (pro == null)
                {
                    return NotFound("Khong tim thay");
                }
                return Ok(pro);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("getProjectByDayBefore/{day}")]
        public IActionResult getProjectByDayBefore(DateTime day)
        {
            try
            {
                var pro = _context.Projects.Where(p => p.IsDeleted == false).Where(p => p.EndDate < day);
                if (pro == null)
                {
                    return NotFound("Khong tim thay du an");
                }
                else
                {
                    return Ok(pro);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getProjectByName/{name}")]
        public IActionResult getProjectByName(string name)
        {
            try
            {
                var pro = _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefault(p => p.Name == name);
                if (pro == null)
                {
                    return NotFound();
                }
                return Ok(pro);

            }
            catch (Exception ex) { return BadRequest(ex); }
        }
        [HttpGet("getProjectIsNotFinished")]
        public IActionResult getProjectIsNotFinished()
        {
            try
            {
                var pro = _context.Projects.Where(p => p.IsFinished == false);
                if (pro == null)
                {
                    return NotFound();
                }
                return Ok(pro);

            }
            catch (Exception ex) { return BadRequest(ex); }
        }


        [HttpGet("getLengthOfProject/{name}")]
        public IActionResult getLengthOfProject(string name)
        {
            try
            {
                var pro = _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefault(p => p.Name == name);
                if (pro == null)
                {
                    return NotFound();
                }
                else
                {
                    TimeSpan count = (TimeSpan)(pro.EndDate - pro.StartDate);
                    double day = count.TotalDays;
                    return Ok(day);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("getProjectById/{Id}")]
        public async Task<ActionResult<Projects>> getProjectById(int Id)
        {
            var project = await _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefaultAsync(x => x.Id == Id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        [HttpGet]
        [Route("getSubProjectById/{SubCode}")]
        public async Task<ActionResult<Projects>> getSubProjectById(string SubCode)
        {
            var project = await _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefaultAsync(x => x.SubProjectCode == SubCode);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        [HttpPut]
        [Route("updateProject/{id}")]
        [Authorize(Roles = "module:project action:update")]
        public async Task<ActionResult> updateProject(EditProjectDto requests, int id)
        {

            var acc = await _context.Projects.FindAsync(id);
            if (acc == null)
            {
                return NotFound();
            }
            /*acc.Name = requests.Name;
            acc.ProjectCode = requests.ProjectCode;
            acc.Description = requests.Description;
            acc.StartDate = requests.StartDate;
            acc.EndDate = requests.EndDate;
            acc.UserId = requests.UserId;
            acc.Leader = requests.Leader;
            acc.UserUpdate = requests.UserUpdate;
            acc.DateUpdate = DateTime.Now;*/
            var project = _mapper.Map<EditProjectDto, Projects>(requests, acc);
            _context.Update(project);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task<bool> checkToken(HttpContext httpContext)
        {
            string token = httpContext.Request.Headers["Authorization"];
            if (token == null)
                return false;
            token = token.Substring("Bearer ".Length).Trim();
            var response = _tokenService.Decode(token);
            var user = await _context.Users.Where(user => user.id == response.id && user.userCode == response.userCode && user.IdGroup == response.group).SingleOrDefaultAsync();
            if (user == null)
                return false;
            return true;
        }
        [HttpPost("getAll")]
        public async Task<IActionResult> getAll(Paginate p)
        {
            if (p.pageIndex == 0)
            {
                p.pageIndex = 1;
            }

            if (p.pageSizeEnum == 0)
            {
                p.pageSizeEnum = 2;
            }
            var keyWord = await _context.Projects.Where(i => i.IsFinished == false
                                                            && (string.IsNullOrEmpty(p.word)
                                                            || i.Id.ToString().Contains(p.word)
                                                            || i.StartDate.ToString().Contains(p.word)
                                                            || i.EndDate.ToString().Contains(p.word)
                                                            || i.UserId.ToString().Contains(p.word)
                                                            || i.Leader.ToString().Contains(p.word)
                                                            || i.UserCreated.ToString().Contains(p.word)
                                                            || i.DateCreated.ToString().Contains(p.word)
                                                            || i.UserUpdate.ToString().Contains(p.word)
                                                            || i.DateUpdate.ToString().Contains(p.word)
                                                            || i.ProjectCode.Contains(p.word)
                                                            || i.Name.Contains(p.word)
                                                            || i.Description!.Contains(p.word))).Skip((p.pageIndex - 1) * p.pageSizeEnum).Take(p.pageSizeEnum).ToListAsync();

            var listSort = keyWord.OrderBy(i => i.DateCreated).ToList();
            var pageIndex = p.pageIndex;
            var pageSize = p.pageSizeEnum;
            var resultPage = await _paginationServices.paginationListTableAsync(listSort, pageIndex, pageSize);

            if (resultPage._success)
            {
                return Ok(resultPage);
            }
            return BadRequest(resultPage);
        }

        [HttpGet]
        [Route("getProjectByDayAfter")]
        public async Task<ActionResult<Projects>> getProjectByDayAfter(DateTime d)
        {
            var project = await _context.Projects.Where(x => x.EndDate > d && x.IsDeleted == false).ToListAsync();
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }


        [HttpGet]
        [Route("getProjectisFinished")]
        public async Task<ActionResult<IEnumerable<Projects>>> getProjectisFinished()
        {
            var project = await _context.Projects.Where(x => x.IsFinished == true).ToListAsync();
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpGet]
        [Route("getUserByproject/{Id}")]
        public async Task<ActionResult> getUserByproject(int Id)
        {
            var project = await _context.Projects.Where(x => x.IsFinished == false).SingleOrDefaultAsync(x => x.Id == Id);
            if (project == null)
            {
                return NotFound();
            }
            var acc = project.UserId;
            return Ok(acc);
        }
        [HttpGet]
        [Route("exportExcel")]
        [Authorize(Roles = "module:project action:export")]
        public async Task<string> DownloadFile()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");
            // get data from DB
            var _data = await _context.Projects.ToListAsync();
            var columns_name = typeof(Projects).GetProperties()
                        .Select(property => property.Name)
                        .ToArray();
            // table header
            for (int idx = 0; idx < columns_name.Length; idx++)
            {
                var cell = ws.Cell(1, idx + 1);
                cell.Value = columns_name[idx];
            }
            // table data
            ws.Cells("A2").Value = _data;
            // Apply style to excel
            for (int idx = 0; idx < columns_name.Length; idx++)
            {
                var col = ws.Column(idx + 1);
                col.AdjustToContents();
            }
            // border for table
            IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(_data.Count() + 1, columns_name.Length).Address);
            data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            // save file to excel folder
            wb.SaveAs("..\\FE\\Excel\\Projects_Table.xlsx");
            return "Excel\\Projects_Table.xlsx";
        }
        [HttpGet("getProjectOnGitlab")]
        public IActionResult getProjectOnGitlab()
        {
            try
            {
                var pro = _context.Projects.Where(p => p.IsOnGitlab == true && p.IsDeleted == false).ToList();
                if (pro == null)
                {
                    return NotFound();
                }
                return Ok(pro);

            }
            catch (Exception ex) { return BadRequest(ex); }
        }

        [HttpGet("UserInProject/{idproject}")]
        public ActionResult UserInProject(int idproject)
        {
            try
            {
                var listuser = from x in _context.Users
                               join c in _context.Member_Projects on x.id equals c.member
                               join d in _context.Projects on c.idProject equals d.Id
                               where d.Id == idproject && d.IsDeleted == false
                               select new
                               {
                                   x,
                                   name = x.lastName + " " + x.firstName,
                               };

                return Ok(listuser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("GetLeadByProject/{project}")]
        public IActionResult GetLeadByProject(string project)
        {
            try
            {
                var list = (from c in _context.Projects
                           join x in _context.Users on c.Leader equals x.id
                           where c.ProjectCode == project
                           select new
                           {
                               x,
                               x.FullName,
                           }).FirstOrDefault();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("GetProjectByIdLead/{idlead}")]
        public IActionResult GetProjectByIdLead(int idlead)
        {
            try
            {
                var list = _context.Projects.Where(x => x.IsFinished == false && x.IsDeleted == false && x.Leader == idlead).ToList();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetProjectByIdPM/{idpm}")]
        public IActionResult GetProjectByIdPM(int idpm)
        {
            try
            {
                var list = _context.Projects.Where(x => x.IsFinished == false && x.IsDeleted == false && x.UserId == idpm).ToList();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("FillterProjectByName")]
        public async Task<IActionResult> FillterProjectByName(string? name, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var dsProject = _context.Projects
                                .Join(_context.Users, x => x.Leader, k => k.id, (x, k) => new { x, k })
                                .Where(xk => xk.x.IsDeleted == false && xk.x.ProjectName.ToLower().Trim().Contains(name.ToLower().Trim()))
                                .Select(xk => new ListProjectDtos()
                                {
                                    id = xk.x.Id,
                                    dateCreated = xk.x.DateCreated,
                                    dateUpdate = xk.x.DateUpdate,
                                    description = xk.x.Description,
                                    endDate = xk.x.EndDate,
                                    isDeleted = xk.x.IsDeleted,
                                    isFinished = xk.x.IsFinished,
                                    isOnGitlab = xk.x.IsOnGitlab,
                                    leader = xk.k.FullName,
                                    idLeader = xk.k.id,
                                    name = xk.x.Name,
                                    projectCode = xk.x.ProjectCode,
                                    subProjectCode = xk.x.SubProjectCode,
                                    projectName = xk.x.ProjectName,
                                    startDate = xk.x.StartDate,
                                    userCreated = xk.x.UserCreated,
                                    userId = xk.x.UserId,
                                    userUpdate = xk.x.UserUpdate,
                                    fullNameUserCreated = _context.Users.Where(s => s.id == xk.x.UserCreated).Select(s => s.FullName).SingleOrDefault(),
                                    fullNameUserUpdate = _context.Users.Where(s => s.id == xk.x.UserUpdate).Select(s => s.FullName).SingleOrDefault(),
                                    fullNameUserId = _context.Users.Where(s => s.id == xk.x.UserId).Select(s => s.FullName).SingleOrDefault(),
                                }).OrderBy(x => x.isFinished == false && x.isDeleted == false).ToList();

                    var pageSize = (int)pageSizeEnum;
                    var resultPage = await _paginationServiceProject.paginationListTableAsync(dsProject, pageIndex, pageSize);

                    if (resultPage._success)
                    {
                        return Ok(resultPage);
                    }
                    return BadRequest(resultPage);
                }
                return BadRequest("Tên dự án rỗng !!!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("FillterProjectByNameOfLead/{IdLead}")]
        public async Task<ActionResult> FillterProjectByNameOfLead(int IdLead, string? name, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var list = _context.Projects
                          .Join(_context.Users, x => x.Leader, d => d.id, (x, d) => new { x, d })
                          .Where(joinResult => joinResult.d.id == IdLead
                          && joinResult.x.IsDeleted == false
                          && joinResult.x.IsFinished == false && joinResult.x.ProjectName.Trim().ToLower().Contains(name.Trim().ToLower()))
                          .OrderBy(joinResult => joinResult.x.IsFinished == false && joinResult.x.IsDeleted == false)
                          .Select(joinResult => new ListProjectDtos()
                          {
                              id = joinResult.x.Id,
                              dateCreated = joinResult.x.DateCreated,
                              dateUpdate = joinResult.x.DateUpdate,
                              description = joinResult.x.Description,
                              endDate = joinResult.x.EndDate,
                              isDeleted = joinResult.x.IsDeleted,
                              isFinished = joinResult.x.IsFinished,
                              isOnGitlab = joinResult.x.IsOnGitlab,
                              leader = joinResult.d.FullName,
                              idLeader = joinResult.d.id,
                              name = joinResult.x.Name,
                              projectCode = joinResult.x.ProjectCode,
                              subProjectCode = joinResult.x.SubProjectCode,
                              projectName = joinResult.x.ProjectName,
                              startDate = joinResult.x.StartDate,
                              userCreated = joinResult.x.UserCreated,
                              userId = joinResult.x.UserId,
                              userUpdate = joinResult.x.UserUpdate,
                              fullNameUserCreated = _context.Users.Where(s => s.id == joinResult.x.UserCreated).Select(s => s.FullName).SingleOrDefault(),
                              fullNameUserUpdate = _context.Users.Where(s => s.id == joinResult.x.UserUpdate).Select(s => s.FullName).SingleOrDefault(),
                              fullNameUserId = _context.Users.Where(s => s.id == joinResult.x.UserId).Select(s => s.FullName).SingleOrDefault(),
                          }).ToList();

                    var pageSize = (int)pageSizeEnum;
                    var resultPage = await _paginationServiceProject.paginationListTableAsync(list, pageIndex, pageSize);
                    if (resultPage._success)
                    {
                        return Ok(resultPage);
                    }
                    return BadRequest(resultPage);
                }
                else
                {
                    return BadRequest("Tên dự án rỗng !!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getMaxIdProject")]
        public async Task<ActionResult> getMaxIdProject()
        {
            try
            {
                var checkProject = await _context.Projects.ToListAsync();
                if (checkProject.Count() > 0)
                {
                    var listArrayProject = await _context.Projects.Select(x => Convert.ToInt32(x.ProjectCode)).ToListAsync();                  
                    int maxValue = listArrayProject.Max();
                    var listSubProject = await _context.Projects.Where(s=> s.SubProjectCode != "").Select(x => Convert.ToInt32(x.SubProjectCode)).ToListAsync();
                    if (listSubProject.Count() > 0)
                    {
                        int maxSubValue = listSubProject.Max();
                        if (maxSubValue == maxValue && maxValue < 1000)
                        {
                            maxValue = 999;
                        }
                        else if (maxSubValue > maxValue)
                        {
                            maxValue = maxSubValue;
                        }
                        
                    }
                    
                    return Ok(maxValue + 1);
                }
                else
                {
                    return Ok(1000);
                }     
            } 
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
        [HttpGet("checkOnProjectGit/{projectCode}")]
        public IActionResult checkOnProjectGit(string projectCode)
        {
            try
            {
                var checkProject = _context.Projects.Where(x => x.IsDeleted == false && x.ProjectCode.Equals(projectCode)).FirstOrDefault();
                if (checkProject != null)
                {
                    return Ok(checkProject);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // Staff
        [HttpGet("FillterProjectByNameOfStaff/{idstaff}")]
        public async Task<ActionResult> FillterProjectByNameOfStaff(int idstaff, string? name, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var list = _context.Projects
                             .Join(_context.Member_Projects, x => x.Id, c => c.idProject, (x, c) => new { x, c })
                             .Join(_context.Users, joinResult1 => joinResult1.x.Leader, k => k.id, (joinResult1, k) => new { joinResult1, k })
                             .Where(joinResult2 => joinResult2.joinResult1.c.member == idstaff
                             && joinResult2.joinResult1.x.IsDeleted == false
                             && joinResult2.joinResult1.x.IsFinished == false && joinResult2.joinResult1.x.ProjectName.Trim().ToLower().Contains(name.Trim().ToLower()))
                             .OrderBy(joinResult2 => joinResult2.joinResult1.x.IsFinished == false && joinResult2.joinResult1.x.IsDeleted == false)
                             .Select(joinResult2 => new ListProjectDtos()
                             {
                                 id = joinResult2.joinResult1.x.Id,
                                 dateCreated = joinResult2.joinResult1.x.DateCreated,
                                 dateUpdate = joinResult2.joinResult1.x.DateUpdate,
                                 description = joinResult2.joinResult1.x.Description,
                                 endDate = joinResult2.joinResult1.x.EndDate,
                                 isDeleted = joinResult2.joinResult1.x.IsDeleted,
                                 isFinished = joinResult2.joinResult1.x.IsFinished,
                                 isOnGitlab = joinResult2.joinResult1.x.IsOnGitlab,
                                 leader = joinResult2.k.FullName,
                                 idLeader = joinResult2.k.id,
                                 name = joinResult2.joinResult1.x.Name,
                                 projectCode = joinResult2.joinResult1.x.ProjectCode,
                                 subProjectCode = joinResult2.joinResult1.x.SubProjectCode,
                                 projectName = joinResult2.joinResult1.x.ProjectName,
                                 startDate = joinResult2.joinResult1.x.StartDate,
                                 userCreated = joinResult2.joinResult1.x.UserCreated,
                                 userId = joinResult2.joinResult1.x.UserId,
                                 userUpdate = joinResult2.joinResult1.x.UserUpdate,
                                 fullNameUserCreated = _context.Users.Where(s => s.id == joinResult2.joinResult1.x.UserCreated).Select(s => s.FullName).SingleOrDefault(),
                                 fullNameUserUpdate = _context.Users.Where(s => s.id == joinResult2.joinResult1.x.UserUpdate).Select(s => s.FullName).SingleOrDefault(),
                                 fullNameUserId = _context.Users.Where(s => s.id == joinResult2.joinResult1.x.UserId).Select(s => s.FullName).SingleOrDefault(),
                             }).ToList();

                    var pageSize = (int)pageSizeEnum;
                    var resultPage = await _paginationServiceProject.paginationListTableAsync(list, pageIndex, pageSize);
                    if (resultPage._success)
                    {
                        return Ok(resultPage);
                    }
                    return BadRequest(resultPage);
                }
                else
                {
                    return BadRequest("Tên dự án rỗng !!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }

}
