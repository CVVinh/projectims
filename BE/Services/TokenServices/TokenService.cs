using BE.Data.Contexts;
using BE.Data.Models;
using BE.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BE.Services.TokenServices
{
    public class TokenService:ITokenService
    {
        private readonly AppDbContext _context;
        private readonly JwtSetting _appSettings;
        private readonly IConfiguration _iconfiguration;

        public TokenService(AppDbContext context, IOptionsMonitor<JwtSetting> optionsMonitor, IConfiguration iconfiguration)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
            _iconfiguration = iconfiguration;
        }

        public string GenerateToken(Users userS)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.Secret);

            Claim[] getClaims()
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Email, userS.email));
                claims.Add(new Claim("id", userS.id.ToString()));
                claims.Add(new Claim("userCode", userS.userCode));
                claims.Add(new Claim("fullName", userS.FullName));
                claims.Add(new Claim("branchUser", userS.idBranch.ToString()));
                claims.Add(new Claim("IdGroup", userS.IdGroup.ToString()));
                claims.Add(new Claim("idUserGitLab", userS.idUserGitLab.ToString()));
                claims.Add(new Claim("tokenUserGitLab", userS.tokenGitLab.ToString()));

                if (userS.avatarLink != null)
                {
                    claims.Add(new Claim("avatarLink", userS.avatarLink.ToString()));
                }

                if (getPermission_Group_AccessModule(userS.id) != null)
                {
                    foreach (var item in getPermission_Group_AccessModule(userS.id))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item));
                    }
                }

                if (getPermission_By_Group_Int(userS.id) != null)
                {
                    foreach (var item in getPermission_By_Group_Int(userS.id))
                    {
                        claims.Add(new Claim("listGroup", item.ToString()));
                    }
                }

                if (getAction_Group_Module(userS.id) != null)
                {
                    foreach (var item in getAction_Group_Module(userS.id))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.ToString()));
                    }
                }

              
                if (getPermission_By_Group_Name(userS.id) != null)
                {
                    foreach (var item in getPermission_By_Group_Name(userS.id))
                    {
                        claims.Add(new Claim("listNameGroup", item.ToString()));
                    }
                }

                return claims.ToArray();
            }

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(getClaims()),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature),
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            return jwtTokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpriredToken(string token)
        {
            var jwtSetting = Encoding.UTF8.GetBytes(_iconfiguration.GetSection("JwtSetting:Secret").Value);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSetting),
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.Secret);
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        /*private string GetRole(Users user)
        {
            var role = "Anonymous";
            foreach (eRole erole in (eRole[])Enum.GetValues(typeof(eRole)))
            {
                if (user.IdGroup == (int)erole)
                {
                    role = erole.ToString();
                }
            }
            return role;
        }*/
        public ClaimToken Decode(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenS = jwtSecurityToken as JwtSecurityToken;
            var userCode = tokenS.Claims.First(claim => claim.Type == "userCode").Value;
            var id = tokenS.Claims.First(claim => claim.Type == "id").Value;
            var group = tokenS.Claims.First(claim => claim.Type == "IdGroup").Value;
            var key = tokenS.Claims.First(claim => claim.Type == "Key").Value;
            var response = new ClaimToken
            {
                userCode = userCode,
                id = Convert.ToInt32(id),
                group = Convert.ToInt32(group)
            };
            return response;
        }

        // bat theo use_menu add edit delete export = 1
        private List<string> getPermission_Use_Menu(int idUser)
        {
            var query = from a in _context.Permission_Use_Menus
                        join b in _context.modules
                        on a.idModule equals b.id
                        where a.isDeleted == false && b.isDeleted == 0 && a.IdUser==idUser
                        select new {a, b};
            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var permission in query)
                {
                    data.Add("modules: "+permission.b.nameModule +" "+ "add: "+(permission.a.Add!=0? 1: 0));

                    data.Add("modules: " + permission.b.nameModule + " " + "update: " + (permission.a.Update != 0 ? 1 : 0));

                    data.Add("modules: " + permission.b.nameModule + " " + "delete: " + (permission.a.Delete != 0 ? 1 : 0));

                    data.Add("modules: " + permission.b.nameModule + " " + "deleteMulti: " + (permission.a.DeleteMulti != 0 ? 1 : 0));

                    data.Add("modules: " + permission.b.nameModule + " " + "confirm: " + (permission.a.Confirm != 0 ? 1 : 0));

                    data.Add("modules: " + permission.b.nameModule + " " + "confirmMulti: " + (permission.a.ConfirmMulti != 0 ? 1 : 0));

                    data.Add("modules: " + permission.b.nameModule + " " + "refuse: " + (permission.a.Refuse != 0 ? 1 : 0));

                    data.Add("modules: " + permission.b.nameModule + " " + "addMember: " + (permission.a.AddMember != 0 ? 1 : 0));

                    data.Add("modules: " + permission.b.nameModule + " " + "export: " + (permission.a.Export != 0 ? 1 : 0));
                }
                return data.Distinct().ToList();
            }
            return null!;
        }


        private List<string> getAction_Group(int idUser)
        {
            var query = from u in _context.Users
                        join g in _context.UserGroups on u.id equals g.idUser
                        join d in _context.Groups on g.idGroup equals d.Id
                        where u.isDeleted == 0 && g.isDeleted == false && d.IsDeleted == 0 && u.id == idUser
                        select d.NameGroup.ToLower();

            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var permission in query)
                {
                    data.Add(permission);
                }
                return data.Distinct().ToList();
            }
            return null!;
        }



        private List<string> getPermission_By_Group_String(int idUser)
        {
            var query = from u in _context.Users
                        join g in _context.UserGroups on u.id equals g.idUser
                        join d in _context.Groups on g.idGroup equals d.Id
                        where u.isDeleted == 0 && g.isDeleted == false && d.IsDeleted == 0 && u.id == idUser
                        select d.NameGroup.ToLower();

            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var permission in query)
                {
                   data.Add(permission);
                }
                return data.Distinct().ToList();
            }
            return null!;
        }

        // bat la group nao idgroup
        private List<int> getPermission_By_Group_Int(int idUser)
        {
            var query = from u in _context.Users
                        join g in _context.UserGroups on u.id equals g.idUser
                        join d in _context.Groups on g.idGroup equals d.Id
                        where u.isDeleted == 0 && g.isDeleted == false && d.IsDeleted == 0 && u.id == idUser
                        select d.Id;

            if (query.Count() != 0)
            {
                List<int> data = new List<int>();
                foreach (var permission in query)
                {
                    data.Add(permission);
                }
                return data.Distinct().ToList();
            }
            return null!;
        }

        public static string RemoveVietnameseSigns(string str)
        {
            string[] vietnameseSigns = new string[] { "aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ", "ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ" };
            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                {
                    str = Regex.Replace(str, vietnameseSigns[i][j].ToString(), vietnameseSigns[0][i - 1].ToString());
                }
            }
            return str;
        }

        private List<string> getAction_Group_Module(int idUser)
        {
            var query = (from u in _context.Users
                        join ug in _context.UserGroups on u.id equals ug.idUser
                        join g in _context.Groups on ug.idGroup equals g.Id
                        join gma in _context.GroupModuleActions on ug.idGroup equals gma.idGroup
                        join m in _context.modules on gma.idModule equals m.id
                        join a in _context.ActionModules on gma.idAction equals a.id
                        where u.id == idUser && gma.isDeleted == false && ug.isDeleted == false
                        select new
                        {
                            g.NameGroup,
                            m.nameModule,
                            a.name,
                        }).OrderBy(x=>x.NameGroup).ThenBy(x=>x.nameModule);

            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var permission in query)
                {
                    string action = RemoveVietnameseSigns(permission.name.Trim().Replace(" ", "").ToLower());
                    if (action == "them")
                    {
                        action = "add";
                    }
                    if (action == "sua")
                    {
                        action = "update";
                    }
                    if (action == "xoa")
                    {
                        action = "delete";
                    }
                    if (action == "xoanhieu")
                    {
                        action = "deleteMulti";
                    }
                    if (action == "xacnhan")
                    {
                        action = "confirm";
                    }
                    if (action == "xacnhannhieu")
                    {
                        action = "confirmMulti";
                    }
                    if (action == "tuchoi")
                    {
                        action = "refuse";
                    }
                    if (action == "themthanhvien")
                    {
                        action = "addMember";
                    }
                    if (action == "xuatfile")
                    {
                        action = "export";
                    }
                    if (action == "binhluan")
                    {
                        action = "comment";
                    }

                    data.Add("module:" +permission.nameModule +" action:"+ action);
                }
                return data.Distinct().ToList();
            }
            return null!;
        }



        // bat lam duoc nhung name group
        private List<string> getPermission_By_Group_Name(int idUser)
        {
            var query = from u in _context.Users
                        join g in _context.UserGroups on u.id equals g.idUser
                        join d in _context.Groups on g.idGroup equals d.Id
                        where u.isDeleted == 0 && g.isDeleted == false && d.IsDeleted == 0 && u.id == idUser
                        select d.NameGroup;

            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var permission in query)
                {
                    data.Add(permission);
                }
                return data.Distinct().ToList();
            }
            return null!;
        }




        // Bat co quyen hay khong
        private List<string> getPermission_Group_AccessModule(int idUser)
        {
            var query = (from ug in _context.UserGroups
                         join pg in _context.Permission_Groups on ug.idGroup equals pg.IdGroup
                         join md in _context.modules on pg.IdModule equals md.id
                         where ug.isDeleted == false && pg.IsDeleted == false && pg.Access == true && md.isDeleted == 0 && ug.idUser == idUser
                         select new { pg, md}).Distinct().ToList();

            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var item in query)
                {
                    data.Add("permission_group: " + item.pg.Access +
                        " module: " + item.md.nameModule);

                }
                return data.Distinct().ToList();
            }
            return null!;
        }

    }
}
