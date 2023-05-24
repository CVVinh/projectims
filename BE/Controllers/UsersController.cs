using BE.Data.Contexts;
using BE.Data.Dtos.UsersDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using BE.Services.TokenServices;
using BE.Data.Dtos.UserDtos;
using BE.Services.MailServices;
using BE.Services.MemberProjectServices;
using BE.Data.Dtos.MailDtos;
using BE.Response;
using BE.Services.PaginationServices;
using ClosedXML.Excel;
using AutoMapper;
using BE.Helpers;
using System.IO;
using MimeKit.Encodings;
using BE.Services.PermissionUserMenuServices;
using BE.Services.UserServices;
using BE.Data.Enum;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Spreadsheet;
using Users = BE.Data.Models.Users;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.InkML;
using System.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly MailService _mailService;

        private readonly IMemberProjectServices _memberProjectServices;
        private readonly IPaginationServices<Users> _paginationServices;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _host;
        private readonly IPermissionUserMenuServices _permissionUserMenuServices;
        private readonly IUserGroupServices _userGroupServices;
        private readonly IPaginationServices<ListUserDtos> _paginationServiceList;
        //public static Users user = new Users();


        public UsersController(IPaginationServices<ListUserDtos> paginationServiceList, IWebHostEnvironment host, AppDbContext context, TokenService tokenService, IMemberProjectServices memberProjectServices, MailService mailService, IPaginationServices<Users> paginationServices, IMapper mapper, IPermissionUserMenuServices permissionUserMenuServices, IUserGroupServices userGroupServices)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _memberProjectServices = memberProjectServices ?? throw new ArgumentNullException(nameof(memberProjectServices));
            _paginationServices = paginationServices;
            _mapper = mapper;
            _host = host;
            _permissionUserMenuServices = permissionUserMenuServices;
            _userGroupServices = userGroupServices;
            _paginationServiceList = paginationServiceList;
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendMail(MailDto mailDto)
        {
            try
            {
                await _mailService.SendMail(mailDto);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /*[HttpGet("getAllWithPaging")]
        //[Authorize(Roles = "Director,HR, Admin")]
        [Authorize(Roles = "permission_group: True module: remotes")]
        [Authorize(Roles = "controller: project action: click add: 1 update: 1 delete: 0 export: 1")]
        public async Task<ActionResult<Users>> GetAllWithPaging(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var users = await _context.Users.OrderByDescending(u => u.dateCreated).ToListAsync();
                if (users == null)
                    return NotFound();
                var result = users.ToPagedList(pageIndex, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }*/

        [HttpGet("getAll")]
        public async Task<ActionResult<Users>> GetAll(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var users = await (from u in _context.Users
                                   from r in _context.Groups.Where(a => a.Id == u.IdGroup).DefaultIfEmpty()
                                   select new ListUserDtos()
                                   {
                                       id = u.id,
                                       userCode = u.userCode,
                                       userCreated = u.userCreated, // => userCode
                                       dateCreated = u.dateCreated,
                                       userModified = u.userModified, // => userCode
                                       dateModified = u.dateModified,
                                       firstName = u.firstName,
                                       lastName = u.lastName,
                                       phoneNumber = u.phoneNumber,
                                       dOB = u.dOB,
                                       gender = u.gender,
                                       address = u.address,
                                       university = u.university,
                                       yearGraduated = u.yearGraduated,
                                       email = u.email,
                                       skype = u.skype,
                                       workStatus = u.workStatus,
                                       dateStartWork = u.dateStartWork,
                                       dateLeave = u.dateLeave,
                                       maritalStatus = u.maritalStatus,
                                       reasonResignation = u.reasonResignation,
                                       identitycard = u.identitycard,
                                       idGroup = r.NameGroup,
                                       isDeleted = u.isDeleted,
                                       fullName = u.FullName,
                                       idBranch = u.idBranch,
                                       nameBranch = _context.Branchs.Where(s => s.isDeleted == 0 && s.idBranch.Equals(u.idBranch)).Select(s => s.branchName).SingleOrDefault(),
                                       rank = u.rank,
                                       idUserGitLab = u.idUserGitLab,
                                       tokenGitLab = u.tokenGitLab,
                                       fullNameCreate = _context.Users.Where(s => s.id == u.userCreated).Select(s => s.FullName).SingleOrDefault(),
                                       fullNameModifier = _context.Users.Where(s => s.id == u.userModified).Select(s => s.FullName).SingleOrDefault()
                                   }).OrderBy(a => a.userCode).Where(s => s.isDeleted != 1).ToListAsync();

                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceList.paginationListTableAsync(users, pageIndex, pageSize);
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

        [HttpGet("getAllUsersByIdProject/{idProject}")]
        public async Task<IActionResult> getAllUsersByIdProject(int idProject, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var success = false;
            var messgae = "";
            var data = new List<Users>();
            try
            {
                var member = await _memberProjectServices.GetMembersByIdProjectAsync(idProject);
                if (member._success)
                {
                    var userIds = member._Data.Select(m => m.member);

                    data = await _context.Users
                        .Where(u => userIds.Contains(u.id))
                        .ToListAsync();

                    var pageSize = (int)pageSizeEnum;
                    var resultPage = await _paginationServices.paginationListTableAsync(data, pageIndex, pageSize);
                    if (resultPage._success)
                    {
                        return Ok(resultPage);
                    }
                    else
                    {
                        success = false;
                        messgae = resultPage._Message;
                        return Ok(new BaseResponse<List<Users>>(success, messgae, data));
                    }
                }
                messgae = member._Message;
                data = null;
                return BadRequest(new BaseResponse<List<Users>>(success, messgae, data));
            }
            catch (Exception ex)
            {
                messgae = ex.Message;
                data = null;
                return StatusCode(500, new BaseResponse<List<Users>>(success, messgae, data));
            }
        }

        [HttpGet("getUsersOutsideProject/{idProject}")]
        public async Task<IActionResult> getAllUsersOutsideProject(int idProject)
        {
            var success = false;
            var messgae = "";
            var data = new List<Users>();
            try
            {
                //var listAllUsers = await _context.Users.Where(u => (u.workStatus == 1 &&
                //(u.IdGroup == 4))).ToListAsync();

                var listAllUsers = from us in _context.Users
                                   join ug in _context.UserGroups on us.id equals ug.idUser
                                   where us.workStatus == 1 && ug.idGroup == 4
                                   orderby us.firstName ascending
                                   select us;

                List<Users> listUserProject = new List<Users>();
                var member = await _memberProjectServices.GetMembersByIdProjectAsync(idProject);

                if (member._success)
                {
                    var memberIdUser = member._Data.Select(x => x.member).ToList();
                    var getUserHaveNotProject = listAllUsers.Where(x => !memberIdUser.Contains(x.id)).ToList();
                    data.AddRange(getUserHaveNotProject);
                    success = true;
                    messgae = "Lấy tất cả user thành công";
                    return Ok(new BaseResponse<List<Users>>(success, messgae, data));
                }
                messgae = member._Message;
                data = null;
                return BadRequest(new BaseResponse<List<Users>>(success, messgae, data));
            }
            catch (Exception ex)
            {
                messgae = ex.Message;
                data = null;
                return StatusCode(500, new BaseResponse<List<Users>>(success, messgae, data));
            }
        }

        [HttpGet("getAllUsersExcepPmLeadProject/{idProject}")]
        public async Task<IActionResult> getAllUsersExcepPmLeadProject(int idProject)
        {
            var success = false;
            var messgae = "";
            var data = new List<Users>();
            try
            {
                var getProject = await _context.Projects.Where(s => s.IsDeleted == false && s.Id == idProject).FirstOrDefaultAsync();
                if (getProject == null)
                {
                    return Ok(new BaseResponse<List<Users>>(success, messgae, data));
                }
                var listAllUsers = await _context.Users.Where(s => s.isDeleted == 0 && (s.id!=getProject.UserCreated && s.id!=getProject.Leader)).OrderBy(x => x.firstName).ToArrayAsync();

                data.AddRange(listAllUsers);
                messgae = "Lấy tất cả user thành công";
                return Ok(new BaseResponse<List<Users>>(success, messgae, data));
            }
            catch (Exception ex)
            {
                messgae = ex.Message;
                data = null;
                return StatusCode(500, new BaseResponse<List<Users>>(success, messgae, data));
            }
        }

        [HttpGet("getUserById/{id}")]
        public async Task<ActionResult<Users>> GetUsersById(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.FindAsync(id);
            var mapper = _mapper.Map<UserDto>(users);
            if (users == null)
            {
                return NotFound();
            }
            return Ok(mapper);
        }

        [HttpGet("getUserByUserCode/{usercode}")]
        public async Task<ActionResult<Users>> GetUsersByUserCode(string usercode)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.userCode.ToLower() == usercode.ToLower()).SingleOrDefaultAsync();
            var mapper = _mapper.Map<UserDto>(users);
            if (users == null)
                return NotFound();
            //var data = new GetAllDto(users);
            return Ok(mapper);
        }

        [HttpGet("getByGender/{gender}")]
        public async Task<ActionResult<Users>> GetUsersByGender(int gender)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.gender == gender).OrderBy(u => u.userCode).ToListAsync();
            var data = new List<GetAllDto>();
            foreach (var user in users)
            {
                data.Add(new GetAllDto(user));
            }
            return Ok(data);
        }

        [HttpGet("getByWorkStatus/{workStatus}")]
        public async Task<ActionResult<Users>> GetUsersByWorkStatus(int workStatus)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.workStatus == workStatus).OrderBy(u => u.userCode).ToListAsync();
            var data = new List<GetAllDto>();
            foreach (var user in users)
            {
                data.Add(new GetAllDto(user));
            }
            return Ok(data);
        }

        [HttpGet("getBydOB/{dOB}")]
        public async Task<ActionResult<Users>> GetUsersByDOB(DateTime dOB)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.dOB == dOB).OrderBy(u => u.userCode).ToListAsync();
            var data = new List<GetAllDto>();
            foreach (var user in users)
            {
                data.Add(new GetAllDto(user));
            }
            return Ok(data);
        }

   
        [HttpGet("userDropdown")]
        public async Task<ActionResult<IEnumerable<DropdownUserDto>>> GetUsersDropdown()
        {
            var users = await _context.Users.OrderBy(u => u.userCode).ToListAsync();
            var userDropdown = new List<DropdownUserDto>();
            foreach (var user in users)
            {
                userDropdown.Add(new DropdownUserDto(user));
            }
            return Ok(userDropdown);
        }

        [HttpGet("getInfo")]
        public async Task<ActionResult<UserInfoDto>> GetInfo()
        {
            var users = await _context.Users.OrderBy(u => u.userCode).ToListAsync();
            var usersInfo = new List<UserInfoDto>();
            if (users == null)
            {
                return NotFound();
            }
            foreach (var user in users)
            {
                usersInfo.Add(new UserInfoDto(user));
            }
            return Ok(usersInfo);
        }

        [HttpPut("deleteUser/{id}")]
        [Authorize(Roles = "module:users action:delete")]
        public async Task<IActionResult> DeleteUser(int id, DeleteUserDto dto)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            var permissionUserMenus = await _context.Permission_Use_Menus.Where(s => s.IdUser.Equals(id)).ToListAsync();

            foreach (var item in permissionUserMenus)
            {
                _context.Permission_Use_Menus.Remove(item);
            }

            var userGroups = await _context.UserGroups.OrderByDescending(s => s.dateCreated).Where(s => s.isDeleted == false && s.idUser.Equals(id)).ToListAsync();
            foreach (var item in userGroups)
            {
                _context.UserGroups.Remove(item);
            }

            users.isDeleted = 1;
            users.userModified = dto.userModified;
            users.dateModified = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok("Delete success...");
        }

        [HttpGet("getGroupByUser/{id}")]
        public async Task<IActionResult> GetRoleUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User not exist...");
            }
            return Ok(user.IdGroup);
        }

        [HttpPost("addUser")]
        [Authorize(Roles = "module:users action:add")]
        public async Task<ActionResult> CreateUser(AddUserDto request)
        {
            try
            {
                var userexist = _context.Users.Where(u => u.userCode == request.userCode);
                if (userexist.Any())
                {
                    return BadRequest("Mã người dùng đang tồn tại");
                }

                var isNumber = request.identitycard.All(char.IsDigit);
                if (!isNumber)
                    return BadRequest("Chứng minh nhân dân không phải là số");
                if (request.identitycard.Length != 9 && request.identitycard.Length != 12)
                    return BadRequest("Độ dài của số CMND phải là 9 hoặc 12.");

                //check is exist
                var identityExist = _context.Users.Where(u => u.identitycard == request.identitycard);
                if (identityExist.Any())
                    return BadRequest("Số nhận dạng tồn tại");
                var phoneExist = _context.Users.Where(u => u.phoneNumber == request.phoneNumber);
                if (request.phoneNumber != null)
                {
                    if (phoneExist.Any() && request.phoneNumber.Length != 0)
                        return BadRequest("Số điện thoại tồn tại");
                }

                var emailExist = _context.Users.Where(u => u.email == request.email);
                if (emailExist.Any())
                    return BadRequest("Email tồn tại");
                var user = new Users
                {
                    userCode = request.userCode,
                    userPassword = request.userPassword == null ?
                    BCrypt.Net.BCrypt.HashPassword(request.userCode) :
                    BCrypt.Net.BCrypt.HashPassword(request.userPassword), // if password is null -> password is usercode
                    userCreated = request.userCreated,
                    dateCreated = request.dateCreated,
                    firstName = request.firstName,
                    lastName = request.lastName,
                    phoneNumber = request.phoneNumber,
                    dOB = request.dOB,
                    gender = request.gender,
                    address = request.address,
                    university = request.university,
                    yearGraduated = request.yearGraduated,
                    email = request.email,
                    emailPassword = request.emailPassword == null ? null : BCrypt.Net.BCrypt.HashPassword(request.emailPassword),
                    skype = request.skype,
                    skypePassword = request.skypePassword == null ? null : BCrypt.Net.BCrypt.HashPassword(request.skypePassword),
                    workStatus = 1,
                    dateStartWork = request.dateStartWork,
                    maritalStatus = request.maritalStatus,
                    reasonResignation = request.reasonResignation,
                    identitycard = request.identitycard,
                    isDeleted = 0,
                    refreshToken = null,
                    idBranch = request.idBranch,
                    rank = request.rank,
                    idUserGitLab = request.idUserGitLab,
                    tokenGitLab = request.tokenGitLab,
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("updateUser/{id}")]
        [Authorize(Roles = "module:users action:update")]
        public async Task<ActionResult<Users>> UpdateUser(int id, EditUserDto user_dto)
        {
            var mess = "";
            try
            {
                var user = await _context.Users.FindAsync(id);
                var editor = await _context.Users.Where(u => u.id == user_dto.userModified).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound("Số nhận dạng tồn tại");
                }
                //check is exist
                var identityExist = _context.Users.Where(u => u.identitycard == user_dto.identitycard && u.id != id);
                if (identityExist.Any())
                    return BadRequest("CMND đã tồn tại");
                var phoneExist = _context.Users.Where(u => u.phoneNumber == user_dto.phoneNumber && u.id != id && u.phoneNumber != null);
                if (phoneExist.Any())
                    return BadRequest("Số điện thoại đã tồn tại");
                var emailExist = _context.Users.Where(u => u.email == user_dto.email && u.id != id);
                if (emailExist.Any())
                    return BadRequest("Email tồn tại");

                if (user_dto.userModified != null)
                {
                    user.dateModified = user_dto.dateModified == null ?  // if value is null, will get value in database
                        DateTime.Now : user_dto.dateModified;
                    user.firstName = user_dto.firstName == null ?
                        user.firstName : user_dto.firstName;
                    user.lastName = user_dto.lastName == null ?
                        user.lastName : user_dto.lastName;
                    user.phoneNumber = user_dto.phoneNumber;
                    user.dOB = user_dto.dOB;
                    user.gender = user_dto.gender == 0 ?
                        user.gender : user_dto.gender;
                    user.address = user_dto.address;
                    user.university = user_dto.university;
                    user.yearGraduated = user_dto.yearGraduated;
                    user.email = user_dto.email;
                    user.skype = user_dto.skype;
                    user.maritalStatus = user_dto.maritalStatus == null ?
                        user.maritalStatus : user_dto.maritalStatus;
                    user.reasonResignation = user_dto.reasonResignation;
                    user.userModified = user_dto.userModified;
                    user.identitycard = user_dto.identitycard;
                    user.workStatus = user_dto.workStatus == 0 ?
                                user.workStatus : user_dto.workStatus;
                    user.dateLeave = user_dto.dateLeave;
                    user.dateStartWork = user_dto.dateStartWork;
                    user.idBranch = user_dto.idBranch;
                    user.rank = user_dto.rank;
                    user.idUserGitLab = user_dto.idUserGitLab;
                    user.tokenGitLab = user_dto.tokenGitLab;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok(new ApiResponseDto
                    {
                        Success = true,
                        Message = mess == "" ? "Cập nhật thành công!" : mess, // if mess is empty => return Update success
                    });
                }
                return BadRequest("Không lấy được thông tin của editor !");
            }
            catch
            {
                return BadRequest("Lỗi không rõ !");
            }
        }

        [HttpPut("changePassword/{userCode}")]
        [Authorize(Roles = "module:users action:update")]
        public async Task<IActionResult> ChangePassword(string userCode, [FromBody] ChangePassDto resource)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.FirstOrDefaultAsync(d => d.userCode.Equals(userCode));
            if (users == null)
            {
                return NotFound("Người dùng không tồn tại !");
            }
            if (users.workStatus != 1)
            {
                return BadRequest("Tài khoản của bạn đã bị khóa !");
            }
            if (resource.newPassword.Equals(resource.oldPassword))
            {
                return BadRequest("Mật khẩu mới không thể giống mật khẩu cũ!");
            }
            if (BCrypt.Net.BCrypt.Verify(resource.oldPassword, users.userPassword))
            {
                users.userPassword = BCrypt.Net.BCrypt.HashPassword(resource.newPassword);
                await _context.SaveChangesAsync();
                return Ok("Đổi mật khẩu thành công!");
            }
            return BadRequest("Mật khẩu hiện tại không chính xác!");
        }
        
        [HttpPost]
        [Route("ResetPass")]
        [Authorize(Roles = "module:users action:update")]
        public async Task<IActionResult> GetNewPass([FromBody] string request)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.Where(u => u.userCode == request).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound("Tài khoản không tồn tại !");
            }
            if (user.workStatus != 1)
            {
                return BadRequest("Người dùng đã bị khóa!");
            }
            Guid id = Guid.NewGuid();
            string newPass = id.ToString().Substring(0, 8);
            user.userPassword = BCrypt.Net.BCrypt.HashPassword(newPass);
            var maildto = new MailDto();
            maildto.To = user.email;
            maildto.Subject = "Đặt lại mật khẩu";
            maildto.Body = "Đây là mật khẩu mới: " + newPass;
            if (await _mailService.SendMail(maildto))
            {
                _context.SaveChanges();
                return Ok(new ApiResponseDto
                {
                    Success = true,
                    Message = "Vui lòng kiểm tra email của bạn !"
                });
            }
            else
            {
                return Ok(new ApiResponseDto
                {
                    Success = false,
                    Message = "Không thể đặt lại mật khẩu!"
                });
            }
        }
        [HttpGet("Pagination")]
        public async Task<ActionResult> GetUsers(int page)
        {
            if (_context.Users == null)
                return NotFound();

            var pageResults = 3f;
            var pageCount = Math.Ceiling(_context.Users.Count() / pageResults);

            var Users = await _context.Users
                .Skip((page - 1) * (int)pageResults)
                .Take((int)pageResults)
                .ToListAsync();

            var response = new UserResponse
            {
                Users = Users,
                CurrentPage = page,
                Pages = (int)pageCount
            };

            return Ok(response);
        }

        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] string Username)
        {
            var user = _context.Users.SingleOrDefault(u => u.userCode.ToLower() == Username.ToLower());
            if (user == null)
            {
                return BadRequest("Người dùng không tồn tại...");
            }
            else
            {
                user.refreshToken = null;
                user.refreshTokenExpiryTime = DateTime.MinValue;
                _context.SaveChanges();
                return Ok(new ApiResponseDto
                {
                    Success = true,
                });
            }
        }
        [HttpGet]
        [Route("exportExcel")]
        [Authorize(Roles = "module:users action:export")]
        public async Task<string> DownloadFile()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");
            // get data from DB
            var _data = await _context.Users.ToListAsync();
            var columns_name = typeof(Users).GetProperties()
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
            wb.SaveAs("..\\FE\\Excel\\Users_Table.xlsx");
            return "Excel\\Users_Table.xlsx";
        }
        [HttpGet("GetNameOfUserById/{id}")]
        public async Task<IActionResult> getNameOfUserById(int id)
        {
            var user = await _context.Users.Where(x => x.id == id).SingleOrDefaultAsync();
            var map = _mapper.Map<UserWithNameDto>(user);
            return Ok(map);
        }

        [HttpPut("updateProfileUser/{id}")]
        public async Task<ActionResult<Users>> UpdateProfileUser([FromRoute] int id, [FromForm] UpdateProfileDto user_dto)
        {
            var mess = "";
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _context.Users.FindAsync(id);
                var editor = await _context.Users.Where(u => u.id == user_dto.userModified).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound("Số nhận dạng tồn tại");
                }
                //check is exist
                var identityExist = _context.Users.Where(u => u.identitycard == user_dto.identitycard && u.id != id);
                if (identityExist.Any())
                    return BadRequest("CMND đã tồn tại");
                var phoneExist = _context.Users.Where(u => u.phoneNumber == user_dto.phoneNumber && u.id != id && u.phoneNumber != null);
                if (phoneExist.Any())
                    return BadRequest("Số điện thoại đã tồn tại");
                var emailExist = _context.Users.Where(u => u.email == user_dto.email && u.id != id);
                if (emailExist.Any())
                    return BadRequest("Email tồn tại");

                if (user_dto.userModified != null)
                {
                    var userMap = _mapper.Map<UpdateProfileDto, Users>(user_dto, user);
                    if (user_dto.formFileAvatar != null)
                    {
                        //var localServer = $"{Request.Scheme}://{Request.Host}";
                        //var upload = _host.WebRootPath;
                        //var fileName = userMap.avatarLink?.Replace($"{localServer}/UploadAvatar/", "");
                        //string filePath = System.IO.Path.Combine(upload, "UploadAvatar", fileName ?? "");
                        //if (System.IO.File.Exists(filePath))
                        //{
                        //    System.IO.File.Delete(filePath);
                        //}
                        //userMap.avatarLink = localServer + FilesHelper.UploadFileAndReturnPath(user_dto.formFileAvatar, upload, "/UploadAvatar/");

                        var ms = new MemoryStream();
                        await user_dto.formFileAvatar.CopyToAsync(ms);

                        var base64String = Convert.ToBase64String(ms.ToArray());
                        var imgSrc = $"data:{user_dto.formFileAvatar.ContentType};base64,{base64String}";
                        userMap.avatarLink = imgSrc;
                        userMap.avatarName = user_dto.formFileAvatar.FileName;
                        userMap.avatarType = user_dto.formFileAvatar.ContentType;
                        userMap.avatarCode = ms.ToArray();
                    }
                    _context.Users.Update(userMap);
                    await _context.SaveChangesAsync();
                    return Ok(new ApiResponseDto
                    {
                        Success = true,
                        Message = mess == "" ? "Cập nhật thành công!" : mess,
                    });
                }
                return BadRequest("Không lấy được thông tin của editor !");
            }
            catch
            {
                return BadRequest("Lỗi không rõ !");
            }
        }

        [HttpGet("GetUserInRolePM")]
        public async Task<IActionResult> GetUserInRolePM()
        {
            var listUser = await _context.Users.Where(x => x.IdGroup == 5).ToListAsync();
            var map = _mapper.ProjectTo<UserWithNameDto>(listUser.AsQueryable());
            return Ok(map);
        }
        [HttpGet("GetUserInRoleStaff")]
        public async Task<IActionResult> GetUserInRoleStaff()
        {
            var listStaff = await _context.Users.Where(x => x.IdGroup == 4).ToListAsync();
            var map = _mapper.ProjectTo<UserWithNameDto>(listStaff.AsQueryable());
            return Ok(map);
        }
        [HttpGet("GetUserInRoleLead")]
        public async Task<IActionResult> GetUserInRoleLead()
        {
            var listUser = await _context.Users.Where(x => x.IdGroup == 3).ToListAsync();
            var map = _mapper.ProjectTo<UserWithNameDto>(listUser.AsQueryable());
            return Ok(map);
        }
        [HttpGet("SearchUserByNameOrUserCode")]
        public async Task<IActionResult> SearchUsersByNameOrUserCode([FromQuery] SearchUserDto searchUserDto, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var users = _context.Users
                             .GroupJoin(_context.Groups, u => u.IdGroup, r => r.Id, (u, r) => new { u, r })
                             .SelectMany(x => x.r.DefaultIfEmpty(), (x, r) => new ListUserDtos
                             {
                                 id = x.u.id,
                                 userCode = x.u.userCode,
                                 userCreated = x.u.userCreated, // => userCode
                                 dateCreated = x.u.dateCreated,
                                 userModified = x.u.userModified, // => userCode
                                 dateModified = x.u.dateModified,
                                 firstName = x.u.firstName,
                                 lastName = x.u.lastName,
                                 phoneNumber = x.u.phoneNumber,
                                 dOB = x.u.dOB,
                                 gender = x.u.gender,
                                 address = x.u.address,
                                 university = x.u.university,
                                 yearGraduated = x.u.yearGraduated,
                                 email = x.u.email,
                                 skype = x.u.skype,
                                 workStatus = x.u.workStatus,
                                 dateStartWork = x.u.dateStartWork,
                                 dateLeave = x.u.dateLeave,
                                 maritalStatus = x.u.maritalStatus,
                                 reasonResignation = x.u.reasonResignation,
                                 identitycard = x.u.identitycard,
                                 idGroup = r != null ? r.NameGroup : null,
                                 isDeleted = x.u.isDeleted,

                                 idBranch = x.u.idBranch,
                                 nameBranch = _context.Branchs.Where(s => s.isDeleted == 0 && s.idBranch.Equals(x.u.idBranch)).Select(s => s.branchName).SingleOrDefault(),
                                 rank = x.u.rank,
                                 idUserGitLab = x.u.idUserGitLab,
                                 tokenGitLab = x.u.tokenGitLab,

                                 fullNameCreate = _context.Users.Where(s => s.id == x.u.userCreated).Select(s => s.FullName).SingleOrDefault() ?? "",
                                 fullNameModifier = _context.Users.Where(s => s.id == x.u.userModified).Select(s => s.FullName).SingleOrDefault() ?? "",
                                 fullName = $"{x.u.firstName} {x.u.lastName}"
                             }).Where(s => s.isDeleted != 1 && s.rank != null)
                             .ToList();

            var result = new List<ListUserDtos>();
            if(searchUserDto.name != null && searchUserDto.idBranch == null && searchUserDto.rank == null)
            {
                result = users.Where(s => s.fullName.Trim().ToLower().Contains(searchUserDto.name.ToLower().Trim())
                             || s.userCode.ToLower().Trim().Contains(searchUserDto.name.ToLower().Trim())).OrderByDescending(a => a.userCode).ToList();
            }
            else if (searchUserDto.name != null && searchUserDto.idBranch != null && searchUserDto.rank == null)
            {
                result = (from us in users
                         join br in _context.Branchs on us.idBranch equals br.idBranch
                         where (us.fullName.Trim().ToLower().Contains(searchUserDto.name.ToLower().Trim()) || us.userCode.ToLower().Trim().Contains(searchUserDto.name.ToLower().Trim())) && br.idBranch == searchUserDto.idBranch
                         select us).ToList();
            }
            else if (searchUserDto.name != null && searchUserDto.idBranch == null && searchUserDto.rank != null)
            {
                result = users.Where(us => us.fullName.Trim().ToLower().Contains(searchUserDto.name.ToLower().Trim()) || us.userCode.ToLower().Trim().Contains(searchUserDto.name.ToLower().Trim()) && us.rank.Trim().ToLower().Contains(searchUserDto.rank.Trim().ToLower())).ToList();
            }
            else if (searchUserDto.name != null && searchUserDto.idBranch != null && searchUserDto.rank != null)
            {
                result = (from us in users
                            join br in _context.Branchs on us.idBranch equals br.idBranch
                            where (us.fullName.Trim().ToLower().Contains(searchUserDto.name.ToLower().Trim()) || us.userCode.ToLower().Trim().Contains(searchUserDto.name.ToLower().Trim())) && br.idBranch.Equals(searchUserDto.idBranch) && us.rank.Trim().ToLower().Contains(searchUserDto.rank.Trim().ToLower())
                          select us).ToList();
            }
            else if (searchUserDto.name == null && searchUserDto.idBranch != null && searchUserDto.rank == null)
            {
                result =(from us in users
                            join br in _context.Branchs on us.idBranch equals br.idBranch
                            where br.idBranch.Equals(searchUserDto.idBranch)
                            select us).ToList();
            }
            else if (searchUserDto.name == null && searchUserDto.idBranch != null && searchUserDto.rank != null)
            {
                result = (from us in users
                            join br in _context.Branchs on us.idBranch equals br.idBranch
                            where br.idBranch.Equals(searchUserDto.idBranch) && us.rank.Trim().ToLower().Contains(searchUserDto.rank.Trim().ToLower())
                          select us).ToList();
            }
            else if (searchUserDto.name == null && searchUserDto.idBranch == null && searchUserDto.rank != null)
            {
                result = users.Where(us => us.rank.Trim().ToLower().Contains(searchUserDto.rank.Trim().ToLower())).ToList();
            }
            else
            {
                result = users;
            }

            var pageSize = (int)pageSizeEnum;
            var resultPage = await _paginationServiceList.paginationListTableAsync(result, pageIndex, pageSize);

            if (resultPage._success)
            {
                return Ok(resultPage);
            }
            return BadRequest(resultPage);

        }
        [HttpGet("SearchMemberByName/{idProject}")]
        public async Task<IActionResult> SearchMemberByName(int idProject, string? name, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var success = false;
            var messgae = "";
            var data = new List<Users>();
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var member = await _memberProjectServices.GetMembersByIdProjectAsync(idProject);
                    if (member._success)
                    {
                        var userIds = member._Data.Select(m => m.member);

                        data = await _context.Users
                            .Where(u => userIds.Contains(u.id))
                            .ToListAsync();

                        var map = data.Where(s => s.FullName.Trim().ToLower().Contains(name.Trim().ToLower())).ToList();

                        var pageSize = (int)pageSizeEnum;
                        var resultPage = await _paginationServices.paginationListTableAsync(map, pageIndex, pageSize);
                        if (resultPage._success)
                        {
                            return Ok(resultPage);
                        }
                        else
                        {
                            success = false;
                            messgae = resultPage._Message;
                            return Ok(new BaseResponse<List<Users>>(success, messgae, data));
                        }
                    }
                    messgae = member._Message;
                    data = null;
                    return BadRequest(new BaseResponse<List<Users>>(success, messgae, data));
                }
                else
                {
                    return BadRequest("Name Không được rỗng !!!");
                }
            }
            catch (Exception ex)
            {
                messgae = ex.Message;
                data = null;
                return StatusCode(500, new BaseResponse<List<Users>>(success, messgae, data));
            }
        }

        [HttpGet("getAllUserByStaff")]
        public async Task<ActionResult<Users>> getAllUserByStaff(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var users = await (from u in _context.Users
                                   from r in _context.UserGroups.Where(a => a.idUser == u.id && a.idGroup == 4)
                                   select new ListUserDtos()
                                   {
                                       id = u.id,
                                       userCode = u.userCode,
                                       userCreated = u.userCreated, // => userCode
                                       dateCreated = u.dateCreated,
                                       userModified = u.userModified, // => userCode
                                       dateModified = u.dateModified,
                                       firstName = u.firstName,
                                       lastName = u.lastName,
                                       phoneNumber = u.phoneNumber,
                                       dOB = u.dOB,
                                       gender = u.gender,
                                       address = u.address,
                                       university = u.university,
                                       yearGraduated = u.yearGraduated,
                                       email = u.email,
                                       skype = u.skype,
                                       workStatus = u.workStatus,
                                       dateStartWork = u.dateStartWork,
                                       dateLeave = u.dateLeave,
                                       maritalStatus = u.maritalStatus,
                                       reasonResignation = u.reasonResignation,
                                       identitycard = u.identitycard,
                                       isDeleted = u.isDeleted,
                                       idBranch = u.idBranch,
                                       nameBranch = _context.Branchs.Where(s => s.isDeleted == 0 && s.idBranch.Equals(u.idBranch)).Select(s => s.branchName).SingleOrDefault(),
                                       rank = u.rank,
                                       idUserGitLab = u.idUserGitLab,
                                       tokenGitLab = u.tokenGitLab,
                                       fullName = u.FullName,
                                       fullNameCreate = _context.Users.Where(s => s.id == u.userCreated).Select(s => s.FullName).SingleOrDefault(),
                                       fullNameModifier = _context.Users.Where(s => s.id == u.userModified).Select(s => s.FullName).SingleOrDefault()
                                   }).OrderBy(a => a.userCode).Where(s => s.isDeleted != 1).ToListAsync();

                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServiceList.paginationListTableAsync(users, pageIndex, pageSize);
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

        //[Authorize(Roles = "permission_group: True module: users")]
        [HttpGet("getAllRank")]
        public async Task<IActionResult> getAllRank()
        {
            var user = await _context.Users.Where(u => u.isDeleted == 0 && u.rank != null).Select(u => u.rank).Distinct().OrderBy(u => u).ToListAsync();
            var objectRank = user.Select(u => new
            {
                nameRank = u,
            }).ToList();
            return Ok(objectRank);
        }

        [HttpGet("getAllUsersByIdProjectByIdUser/{idProject}/{idUser}")]
        public async Task<IActionResult> getAllUsersByIdProjectByIdUser(int idProject, int idUser, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var success = false;
            var messgae = "";
            var data = new List<Users>();
            try
            {
                var checkIdProject = await _context.Projects.Where(p => p.Id == idProject && p.IsDeleted == false).FirstOrDefaultAsync();
                var checkIdUser = await _context.Users.Where(p => p.id.Equals(idUser) && p.isDeleted.Equals(0)).FirstOrDefaultAsync();
                if(checkIdProject != null && checkIdUser != null)
                {
                    var memberProject = await _context.Member_Projects.Where(mp => mp.idProject == idProject && mp.isDeleted == false).Select(s => s.member).ToListAsync();

                    data = await _context.Users
                        .Where(u => memberProject.Contains(u.id))
                        .ToListAsync();
                    data.Add(checkIdUser);
                    var userGroup = await _context.UserGroups.Where(u => u.idUser.Equals(idUser) && u.isDeleted==false).ToArrayAsync();
                    if (userGroup.Count() > 0)
                    {
                        var getUserPM = from us in _context.Users
                                        join pj in _context.Projects on us.id equals pj.UserCreated
                                        where pj.Id == idProject
                                        select us;
                        var getUserLd = from us in _context.Users
                                        join pj in _context.Projects on us.id equals pj.Leader
                                        where pj.Id == idProject
                                        select us;
                        var checkCancel = true;
                        foreach(var item in userGroup )
                        {
                            if ((item.idGroup.Equals(1) || item.idGroup.Equals(2)) && checkCancel==true)
                            {
                                data.AddRange(getUserPM);
                                data.AddRange(getUserLd);
                                checkCancel = false;
                            }
                            else if (item.idGroup.Equals(5))
                            {
                                data.AddRange(getUserLd);
                            }
                        }
                    }
                    var pageSize = (int)pageSizeEnum;
                    var resultPage = await _paginationServices.paginationListTableAsync(data, pageIndex, pageSize);
                    if (resultPage._success)
                    {
                        return Ok(resultPage);
                    }
                    else
                    {
                        success = false;
                        messgae = resultPage._Message;
                        return Ok(new BaseResponse<List<Users>>(success, messgae, data));
                    }
                }
                return BadRequest(new BaseResponse<List<Users>>(success, "Lấy tất cả dữ liệu thành công!", data));
            }
            catch (Exception ex)
            {
                messgae = ex.Message;
                return StatusCode(500, new BaseResponse<List<Users>>(success, messgae, data));
            }
        }

        [HttpGet("getAllUsersByRoleInProject/{idProject}/{idUser}")]
        public async Task<IActionResult> getAllUsersByRoleInProject(int idProject, int idUser)
        {
            var success = false;
            var messgae = "";
            var data = new List<Users>();
            try
            {
                var getProject = await _context.Projects.Where(s => s.IsDeleted == false && s.Id == idProject).FirstOrDefaultAsync();
                if (getProject != null)
                {
                    var memberProject = await _context.Member_Projects.Where(mp => mp.idProject == idProject && mp.isDeleted == false).Select(s => s.member).ToListAsync();
                    data = await _context.Users.Where(u => !memberProject.Contains(u.id)).ToListAsync();

                    var userGroup = await _context.UserGroups.Where(u => u.idUser.Equals(idUser) && u.isDeleted == false).ToArrayAsync();
                    if (userGroup.Count() > 0)
                    {
                        foreach (var item in userGroup)
                        {
                            if (item.idGroup.Equals(3))
                            {
                                data = data.Where(u => u.id != getProject.UserCreated).ToList();
                            }
                            if (item.idGroup.Equals(4))
                            {
                                data = data.Where(u => u.id != getProject.UserCreated && u.id != getProject.Leader).ToList();
                            }
                        }
                    }
                }
                messgae = "Lấy tất cả user thành công";
                return Ok(new BaseResponse<List<Users>>(success, messgae, data));
            }
            catch (Exception ex)
            {
                messgae = ex.Message;
                return StatusCode(500, new BaseResponse<List<Users>>(success, messgae, data));
            }
        }


    }
}

