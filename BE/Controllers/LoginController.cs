using BE.Data.Contexts;
using BE.Data.Dtos.MailDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Dtos.UsersDTO;
using BE.Services.MailServices;
using BE.Services.TokenServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly MailService _mailService;
        public LoginController(AppDbContext context,TokenService tokenService, MailService mailService)
        {
            _context = context;
            _tokenService = tokenService;
            _mailService = mailService; 
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.userCode.ToLower() == loginDto.UserName.ToLower());
            if (user == null || user.isDeleted == 1)
            {
                return NotFound("Tài khoản không tồn tại !");
            }
            if (user.workStatus != 1)
            {
                return BadRequest("Người dùng đã bị khóa!");
            }
            else
            {
                bool isPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.userPassword);
                if (isPassword)
                {
                    var accessToken = _tokenService.GenerateToken(user);
                    var refreshToken = _tokenService.GenerateRefreshToken();
                    user.refreshToken = refreshToken;
                    user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    await _context.SaveChangesAsync();
                    return Ok(new ApiResponseDto
                    {
                        Success = true,
                        Message = "Xác thực thành công...",
                        Username = loginDto.UserName,
                        Token = accessToken,
                    });
                }
                return BadRequest("Sai mật khẩu !");
            }
        }

        [HttpPost]
        [Route("ResetPass")]
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


    }
}
