using BE.Data.Contexts;
using BE.Data.Dtos.OTDtos;
using BE.Data.Enum;
using BE.Data.Enum.OTEnums;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE.Services.TokenServices;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using BE.Services.PaginationServices;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: ots")]
    public class OTsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IPaginationServices<ListOTDtos> _paginationServices;

        public OTsController(IPaginationServices<ListOTDtos> paginationServices, AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
            _paginationServices = paginationServices;
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


        [HttpGet]
        [Route("getByType/{type}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByType(Types type)
        {
            var ots = await _context.OTs.Where(ot => ot.type == type).ToListAsync();
            if (ots == null)
                return NotFound();
            return Ok(ots);
        }


        [HttpGet]
        [Route("getPageTotal")]
        public ActionResult getPage(Types type, int recordNum)
        {
            var count = _context.OTs.Count(ot => ot.type == type);
            var pageCount = Math.Ceiling(count / Convert.ToDecimal(recordNum));
            return Ok((int)pageCount);
        }


        // Pagination
        [HttpPost]
        [Route("getPaginate")]
        public async Task<ActionResult> GetAll(PaginateOT paginate)
        {
            try
            {
                if (paginate.recordNum == 0)
                    paginate.recordNum = 10;
                if (paginate.page == 0)
                    paginate.page = 1;

                var ots = await _context.OTs.Where(ot => ot.type.Equals(paginate.type)).OrderByDescending(ot => ot.dateCreate).Skip((paginate.page - 1) * paginate.recordNum)
                    .Take(paginate.recordNum).ToListAsync();
                if (ots == null)
                    return NotFound();
                if (paginate.recordNum == 0)
                    paginate.recordNum = 10;
                var pageCount = Math.Ceiling(ots.Count() / Convert.ToDecimal(paginate.recordNum));
                var OTs = ots
                    .Skip((paginate.page - 1) * paginate.recordNum)
                    .Take(paginate.recordNum).ToList();
                var response = new OTresponse
                {
                    OTs = OTs,
                    CurrentPage = paginate.page,
                    Pages = (int)pageCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("getOTByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByUser(int userId)
        {
            var ot = await _context.OTs.Where(o => o.user == userId).OrderBy(o => o.status).ToListAsync();
            if (ot == null)
                return NotFound();
            return Ok(ot);
        }


        [HttpGet]
        [Route("getOTByLead/{leadId}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetById(int leadId)
        {
            var ot = await _context.OTs.Where(o => o.leadCreate == leadId).OrderBy(o => o.status).ToListAsync();
            if (ot == null)
                return NotFound();
            return Ok(ot);
        }


        [HttpGet]
        [Route("getOTByID/{id}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByLead(int id)
        {
            var ot = await _context.OTs.Where(o => o.id == id).SingleOrDefaultAsync();
            if (ot == null)
                return NotFound();
            return Ok(ot);
        }


        [HttpGet]
        [Route("getByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByStatus(StatusOT status)
        {
            var OTs = await _context.OTs.Where(o => o.status == status).ToListAsync();
            if (OTs == null)
                return NoContent();
            return Ok(OTs);
        }


        [HttpGet]
        [Route("getOTByProject/{proId}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByProject(int proId)
        {
            var list = from x in _context.OTs
                       join c in _context.Projects on x.idProject equals c.Id
                       join f in _context.Users on x.leadCreate equals f.id
                       join q in _context.Users on x.updateUser equals q.id
                       join d in _context.Users on x.user equals d.id
                       where (x.idProject == proId)
                       select new
                       {
                           x,
                           c.Name,
                           nameLead = f.id == x.leadCreate ? f.FullName : null,
                           nameUser = x.user == d.id ? d.FullName : null,
                           nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                           usercode = d.userCode,
                       };

            if (list == null)
                return NoContent();
            return Ok(list);
        }

        [HttpGet("getOTBySample")]
        public async Task<ActionResult<IEnumerable<OTs>>> getOTBySample(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where x.status == StatusOT.accepted
                        orderby x.status ascending
                        select new ListOTDtos()
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();

            var pageSize = (int)pageSizeEnum;
            var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
            if (resultPage._success)
            {
                return Ok(resultPage);
            }
            return BadRequest(resultPage);
            //if (list == null)
            //    return NoContent();
            //return Ok(list);
        }



        [HttpPost]
        [Route("createOT")]
        [Authorize(Roles = "module:ots action:add")]
        public async Task<IActionResult> Create(OTs OTs)
        {
            var ots = await _context.OTs.Where(ot => ot.Date == OTs.Date && ot.user == OTs.user).SingleOrDefaultAsync();
            if (ots != null && ots.status != StatusOT.deleted)
            {
                TimeOnly startCurrent = TimeOnly.Parse(ots.start);
                TimeOnly endCurrent = TimeOnly.Parse(ots.end);
                TimeOnly startCreate = TimeOnly.Parse(OTs.start);
                TimeOnly endCreate = TimeOnly.Parse(OTs.end);
                if ((startCurrent >= endCreate) || (endCurrent <= startCreate))
                {
                    await _context.OTs.AddAsync(OTs);
                    _context.SaveChanges();
                    return Ok(OTs);
                }
                else return BadRequest();
            }
            await _context.OTs.AddAsync(OTs);
            _context.SaveChanges();
            return Ok(OTs);
        }


        [HttpPut]
        [Route("acceptOT")]
        [Authorize(Roles = "module:ots action:confirm")]
        [Authorize(Roles = "module:ots action:refuse")]
        public async Task<IActionResult> Update(AcceptOTDto dto)
        {
            var ot = await _context.OTs.Where(o => o.id == dto.id).SingleOrDefaultAsync();
            if (ot == null)
                return NoContent();
            if (dto.status == StatusOT.deny)
                ot.note = dto.note;
            ot.dateUpdate = DateTime.UtcNow;
            ot.updateUser = dto.PM;
            ot.status = dto.status;
            _context.SaveChanges();
            return Ok(ot);
        }

        [HttpPut]
        [Route("updateOT/{id}")]
        [Authorize(Roles = "module:ots action:update")]
        public async Task<IActionResult> Update(int id, EditOTDto dto)
        {
            try
            {
                var OTs = await _context.OTs.Where(o => o.id == id).SingleOrDefaultAsync();
                if (OTs == null)
                    return NotFound();
                if (OTs.status == StatusOT.accepted)
                    return BadRequest();
                OTs.Date = dto.Date;
                OTs.start = dto.start;
                OTs.end = dto.end;
                OTs.realTime = dto.realTime;
                OTs.user = dto.user;
                OTs.idProject = dto.idProject;
                OTs.description = dto.description;
                OTs.dateUpdate = DateTime.UtcNow;
                OTs.updateUser = dto.updateUser;
                OTs.status = StatusOT.process;
                OTs.note = "";
                _context.SaveChanges();
                return Ok(OTs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPut]
        [Route("deleteOT")]
        [Authorize(Roles = "module:ots action:delete")]
        public async Task<IActionResult> Accept(int idOT, int PM)
        {
            var ot = await _context.OTs.Where(o => o.id == idOT).SingleOrDefaultAsync();
            if (ot == null)
                return NoContent();
            ot.dateUpdate = DateTime.UtcNow;
            ot.updateUser = PM;
            ot.status = StatusOT.deleted;
            _context.SaveChanges();
            return Ok(ot);
        }


        [HttpGet]
        [Route("getAccept/{type}")]
        public async Task<ActionResult<IEnumerable<OTs>>> getAccept(Types type)
        {
            return await _context.OTs.Where(ot => ot.type == type && ot.status == 0).ToListAsync();
        }


        // For PM
        [HttpGet("getOTsByidPM/{IdPM}")]
        public async Task<ActionResult> GetOTsByIdPM(int IdPM, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where x.status != StatusOT.deleted
                            select new ListOTDtos
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note,
                                pmName = k.FullName
                            }).OrderByDescending(s => s.usercode).ToList();

                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
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


        // For sample
        [HttpGet("GetAllOTs")]
        public async Task<ActionResult> GetAllOTs(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where x.status != StatusOT.deleted
                            orderby x.status ascending
                            select new ListOTDtos()
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                userCreated = c.UserCreated,
                                note = x.note,
                                usercode = d.userCode,
                                pm = c.UserId,
                                pmName = k.FullName
                            }).ToList();
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
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


        // For staff
        [HttpGet("GetAllOTsByStaff/{idstaff}")]
        public async Task<ActionResult> GetAllOTsByStaff(int idstaff, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where (x.user == idstaff && x.status != StatusOT.deleted)
                            orderby x.status ascending
                            select new ListOTDtos
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note,
                                usercode = d.userCode,
                                pm = c.UserId,
                                pmName = k.FullName
                            }).OrderByDescending(s => s.usercode).ToList();
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
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

        [HttpGet("GetAllOTsByStaffAccept/{idstaff}")]
        public async Task<ActionResult> GetAllOTsByStaffAccept(int idstaff, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where (x.user == idstaff && x.status == StatusOT.accepted)
                            orderby x.status ascending
                            select new ListOTDtos
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note,
                                usercode = d.userCode,
                                pm = c.UserId,
                                pmName = k.FullName
                            }).OrderByDescending(s => s.usercode).ToList();
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
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


        // For LEAD
        [HttpGet("GetAllOTsByLead/{IdLead}")]
        public async Task<ActionResult> GetAllOTsByLead(int IdLead, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var list = (from x in _context.OTs 
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where (x.leadCreate == IdLead && x.status != StatusOT.deleted)
                            orderby x.status ascending
                            select new ListOTDtos
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note,
                                usercode = d.userCode,
                                pm = c.UserId,
                                pmName = k.FullName
                            }).ToList();

                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
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


        [HttpGet]
        [Route("getOTByMonth/month={month}&year={year}")]
        public IActionResult GetByMonth(int month, int year)
        {

            var list = from x in _context.OTs
                       join c in _context.Projects on x.idProject equals c.Id
                       join f in _context.Users on x.leadCreate equals f.id
                       join q in _context.Users on x.updateUser equals q.id
                       join d in _context.Users on x.user equals d.id
                       where (x.Date.Month == month && x.Date.Year == year)
                       select new
                       {
                           x,
                           c.Name,
                           nameLead = f.id == x.leadCreate ? f.FullName : null,
                           nameUser = x.user == d.id ? d.FullName : null,
                           nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                           usercode = d.userCode,
                       };



            if (list == null)
                return NoContent();
            return Ok(list);
        }


        [HttpGet("filterByLead/{month}/{year}/{idproject}")]
        public async Task<IActionResult> filterByLead(int? pageIndex, PageSizeEnum pageSizeEnum, int iduser, int? month = 0, int? year = 0, int? idproject = 0)
        {
            List<ListOTDtos> list = new List<ListOTDtos>();
            if (month != 0 && idproject != 0)
            {
                list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.leadCreate == iduser && x.status != StatusOT.deleted)
                            select new ListOTDtos
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note,
                                usercode = d.userCode,
                                pm = c.UserId,
                                pmName = k.FullName
                            }).OrderByDescending(s => s.usercode).ToList();
            }

            if(month == 0 && idproject == 0)
            {
                list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where (x.leadCreate == iduser && x.status != StatusOT.deleted)
                            select new ListOTDtos
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note,
                                usercode = d.userCode,
                                pm = c.UserId,
                                pmName = k.FullName
                            }).OrderByDescending(s => s.usercode).ToList();
            }

            if(month == 0 && idproject != 0)
            {
                list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where (x.idProject == idproject && x.leadCreate == iduser && x.status != StatusOT.deleted)
                            select new ListOTDtos
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note,
                                usercode = d.userCode,
                                pm = c.UserId,
                                pmName = k.FullName
                            }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month != 0 && idproject == 0)
            {
                list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            join k in _context.Users on c.UserId equals k.id
                            where (x.Date.Month == month && x.Date.Year == year && x.leadCreate == iduser && x.status != StatusOT.deleted)
                            select new ListOTDtos
                            {
                                x = x,
                                name = c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note,
                                usercode = d.userCode,
                                pm = c.UserId,
                                pmName = k.FullName
                            }).OrderByDescending(s => s.usercode).ToList();
            }


            var pageSize = (int)pageSizeEnum;
            var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
            if (resultPage._success)
            {
                return Ok(resultPage);
            }
            return BadRequest(resultPage);
        }

        [HttpGet("filterAll/{month}/{year}/{idproject}")]
        public async Task<IActionResult> filterAll(int? pageIndex, PageSizeEnum pageSizeEnum, int iduser, int? month = 0, int? year = 0, int? idproject = 0)
        {
            List<ListOTDtos> list = new List<ListOTDtos>();
            if (month != 0 && idproject != 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.status != StatusOT.deleted)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month == 0 && idproject == 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.status != StatusOT.deleted)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month == 0 && idproject != 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.idProject == idproject && x.status != StatusOT.deleted)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month != 0 && idproject == 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.Date.Month == month && x.Date.Year == year && x.status != StatusOT.deleted)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }


            var pageSize = (int)pageSizeEnum;
            var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
            if (resultPage._success)
            {
                return Ok(resultPage);
            }
            return BadRequest(resultPage);
        }

        [HttpGet("filterByOffice/{month}/{year}/{idproject}")]
        public async Task<IActionResult> filterByOffice(int? pageIndex, PageSizeEnum pageSizeEnum, int iduser, int? month = 0, int? year = 0, int? idproject = 0)
        {
            List<ListOTDtos> list = new List<ListOTDtos>();
            if (month != 0 && idproject != 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.status == StatusOT.accepted)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month == 0 && idproject == 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.status != StatusOT.deleted)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month == 0 && idproject != 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.idProject == idproject && x.status != StatusOT.accepted)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month != 0 && idproject == 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.Date.Month == month && x.Date.Year == year && x.status == StatusOT.accepted)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }


            var pageSize = (int)pageSizeEnum;
            var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
            if (resultPage._success)
            {
                return Ok(resultPage);
            }
            return BadRequest(resultPage);
        }



        [HttpGet("filterByStaff/{month}/{year}/{idproject}")]
        public async Task<IActionResult> filterByStaff(int? pageIndex, PageSizeEnum pageSizeEnum, int iduser, int? month = 0, int? year = 0, int? idproject = 0)
        {
            List<ListOTDtos> list = new List<ListOTDtos>();
            if (month != 0 && idproject != 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.user == iduser)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month == 0 && idproject == 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.user == iduser)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month == 0 && idproject != 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.idProject == idproject && x.user == iduser)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }

            if (month != 0 && idproject == 0)
            {
                list = (from x in _context.OTs
                        join c in _context.Projects on x.idProject equals c.Id
                        join f in _context.Users on x.leadCreate equals f.id
                        join q in _context.Users on x.updateUser equals q.id
                        join d in _context.Users on x.user equals d.id
                        join k in _context.Users on c.UserId equals k.id
                        where (x.Date.Month == month && x.Date.Year == year && x.user == iduser)
                        select new ListOTDtos
                        {
                            x = x,
                            name = c.Name,
                            nameLead = f.id == x.leadCreate ? f.FullName : null,
                            nameUser = x.user == d.id ? d.FullName : null,
                            nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                            dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                            note = x.note,
                            usercode = d.userCode,
                            pm = c.UserId,
                            pmName = k.FullName
                        }).OrderByDescending(s => s.usercode).ToList();
            }


            var pageSize = (int)pageSizeEnum;
            var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
            if (resultPage._success)
            {
                return Ok(resultPage);
            }
            return BadRequest(resultPage);
        }


        [HttpGet("filterByRole/{month}/{year}/{idproject}/{idrole}")]
        public async Task<ActionResult> filterByRole(int? pageIndex, PageSizeEnum pageSizeEnum, int? month = 0, int? year = 0, int? idproject = 0, int? idrole = 0, int? iduser = 0)
        {
            List<ListOTDtos> list = new List<ListOTDtos>();
            if (idrole == 0 || iduser == 0)
            {
                return Ok("Please input Idrole or IdUser");
            }
            else
            {
                // Admin
                if (idrole == 1 || idrole == 5)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.status != StatusOT.deleted)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();

                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }

                    if (idproject == 0 && month != 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.Date.Month == month && x.Date.Year == year && x.status != StatusOT.deleted)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();
                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }

                    if (month == 0 && idproject != 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.idProject == idproject && x.status != StatusOT.deleted)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();

                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }
                }
                //Sample
                if (idrole == 2)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.status == StatusOT.accepted)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();


                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);

                    }

                    if (idproject == 0 && month != 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.Date.Month == month && x.Date.Year == year && x.status == StatusOT.accepted)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();
                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }

                    if (month == 0 && idproject != 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.idProject == idproject && x.status == StatusOT.accepted)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();

                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }
                }
                //Lead
                if (idrole == 3)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.leadCreate == iduser && x.status != StatusOT.deleted)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();


                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);

                    }

                    if (idproject == 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.Date.Month == month && x.Date.Year == year && x.leadCreate == iduser)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();
                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }

                    if (month == 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.idProject == idproject && x.leadCreate == iduser)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();

                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }
                }
                // Staff
                if (idrole == 4)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.user == iduser)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();
                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);

                    }

                    if (idproject == 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.Date.Month == month && x.Date.Year == year && x.user == iduser)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();
                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }

                    if (month == 0)
                    {
                        list = (from x in _context.OTs
                                join c in _context.Projects on x.idProject equals c.Id
                                join f in _context.Users on x.leadCreate equals f.id
                                join q in _context.Users on x.updateUser equals q.id
                                join d in _context.Users on x.user equals d.id
                                join k in _context.Users on c.UserId equals k.id
                                where (x.idProject == idproject && x.leadCreate == iduser)
                                select new ListOTDtos
                                {
                                    x = x,
                                    name = c.Name,
                                    nameLead = f.id == x.leadCreate ? f.FullName : null,
                                    nameUser = x.user == d.id ? d.FullName : null,
                                    nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                    dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                    note = x.note,
                                    usercode = d.userCode,
                                    pm = c.UserId,
                                    pmName = k.FullName
                                }).OrderByDescending(s => s.usercode).ToList();

                        //if (list == null)
                        //    return NoContent();
                        //return Ok(list);
                    }
                }

                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(list, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
        }


        [HttpGet]
        [Route("GetByMonthAndProject/month={month}&year={year}&idProject={idProject}")]
        public async Task<ActionResult> GetByMonthAndProject(int? month = 0, int? year = 0, int? idProject = 0)
        {
            try
            {
                if (idProject == 0)
                {
                    var list = from x in _context.OTs
                               join c in _context.Projects on x.idProject equals c.Id
                               join f in _context.Users on x.leadCreate equals f.id
                               join q in _context.Users on x.updateUser equals q.id
                               join d in _context.Users on x.user equals d.id
                               where (x.Date.Month == month && x.Date.Year == year)
                               select new
                               {
                                   x,
                                   c.Name,
                                   nameLead = f.id == x.leadCreate ? f.FullName : null,
                                   nameUser = x.user == d.id ? d.FullName : null,
                                   nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                   dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                   note = x.note
                               };
                    if (list == null)
                        return NoContent();
                    return Ok(list);

                }
                else if (month == 0)
                {

                    var list = from x in _context.OTs
                               join c in _context.Projects on x.idProject equals c.Id
                               join f in _context.Users on x.leadCreate equals f.id
                               join q in _context.Users on x.updateUser equals q.id
                               join d in _context.Users on x.user equals d.id
                               where (x.idProject == idProject)
                               select new
                               {
                                   x,
                                   c.Name,
                                   nameLead = f.id == x.leadCreate ? f.FullName : null,
                                   nameUser = x.user == d.id ? d.FullName : null,
                                   nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                   dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                   note = x.note
                               };

                    if (list == null)
                        return NoContent();
                    return Ok(list);
                }
                else if (month != 0 && idProject != 0)
                {
                    var list = from x in _context.OTs
                               join c in _context.Projects on x.idProject equals c.Id
                               join f in _context.Users on x.leadCreate equals f.id
                               join q in _context.Users on x.updateUser equals q.id
                               join d in _context.Users on x.user equals d.id
                               where (x.idProject == idProject && x.Date.Month == month && x.Date.Year == year)
                               select new
                               {
                                   x,
                                   c.Name,
                                   nameLead = f.id == x.leadCreate ? f.FullName : null,
                                   nameUser = x.user == d.id ? d.FullName : null,
                                   nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                   dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                   note = x.note
                               };


                    if (list == null)
                        return NoContent();
                    return Ok(list);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost("AddOTs")]
        [Authorize(Roles = "module:ots action:add")]
        public async Task<IActionResult> AddOTs(AddRangeOTs OTs)
        {
            try
            {
                var listadd = new List<OTs>();
                OTs.user.ForEach(ele =>
                {
                    listadd.Add(new OTs()
                    {
                        Date = OTs.Date,
                        start = OTs.start,
                        end = OTs.end,
                        realTime = OTs.realTime,
                        user = ele,
                        idProject = OTs.idProject,
                        description = OTs.description,
                        updateUser = OTs.leadCreate,
                        dateCreate = OTs.dateCreate,
                        dateUpdate = OTs.dateUpdate,
                        status = OTs.status,
                        leadCreate = OTs.leadCreate,
                        note = OTs.note,
                        type = 0,
                    });
                });

                await _context.OTs.AddRangeAsync(listadd);
                _context.SaveChanges();
                return Ok("Success");

            }
            catch (Exception ex)
            {
                return BadRequest("failed");
            }
        }


    }
}

